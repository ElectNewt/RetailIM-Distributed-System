using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using RetailIM.Model.Mappers;
using Shared.ROP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetailIM.Services.Orders
{

    public interface IRetrievePaginatedOrdersDependencies
    {
        Task<ReadOnlyCollection<OrderEntity>> GetOrders(int page, int itemsPerPage);
        Task<DeliveryEntity> GetDeliveryFromOrderId(Guid orderId);
        Task<ReadOnlyCollection<OrderProductEntity>> GetProductsForOrderId(Guid orderId);
        Task<ReadOnlyCollection<ProductEntity>> GetProductsById(List<Guid> productsIds);
    }

    public class RetrievePaginatedOrders
    {
        public readonly IRetrievePaginatedOrdersDependencies _dependencies;

        public RetrievePaginatedOrders(IRetrievePaginatedOrdersDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<PaginatedOrdersResponseDto>> Execute(int page)
        {
            int itemsPerPage = 10;
            return await GetPaginatedOrders(page, itemsPerPage)
                .Bind(ProcessOrders)
                .MapAsync(ToResponseDto);

            PaginatedOrdersResponseDto ToResponseDto(List<OrderDto> orders)
            {
                return new PaginatedOrdersResponseDto() { Orders = orders };
            }
        }

        public async Task<Result<ReadOnlyCollection<OrderEntity>>> GetPaginatedOrders(int page, int itemsPerPage)
        {
            return await _dependencies.GetOrders(page, itemsPerPage);
        }

        private async Task<Result<List<OrderDto>>> ProcessOrders(ReadOnlyCollection<OrderEntity> orders)
        {
            List<OrderDto> orderDtoList = new List<OrderDto>();
            List<Result<OrderDto>> ordersLis = new List<Result<OrderDto>>();
            foreach (var order in orders)
            {
                var t = await GetOrderInformation(order);
                ordersLis.Add(t);
            }

            return ordersLis
                .Traverse();

            async Task<Result<OrderDto>> GetOrderInformation(OrderEntity order)
            {
                return await GetDeliveryInformation(order.OrderId)
                    .Combine(_ => GetProductsInOrder(order.OrderId))
                    .Bind(x => BuildOrderDto(x, order));
            }
        }

        private async Task<Result<DeliveryEntity>> GetDeliveryInformation(Guid orderId)
        {
            return await _dependencies.GetDeliveryFromOrderId(orderId);
        }

        private async Task<ReadOnlyCollection<ProductValues>> GetProductsInOrder(Guid orderId)
        {
            ReadOnlyCollection<OrderProductEntity> productsInOrder = await _dependencies.GetProductsForOrderId(orderId);
            ReadOnlyCollection<ProductEntity> prductsInformation = await GetProductsInformation(productsInOrder);

            return productsInOrder.Select(BuildSingleOrderProductValues).ToList().AsReadOnly();

            async Task<ReadOnlyCollection<ProductEntity>> GetProductsInformation(ReadOnlyCollection<OrderProductEntity> producstInOrder)
            {
                return await _dependencies.GetProductsById(producstInOrder.Select(a => a.ProductId).ToList());
            }

            ProductValues BuildSingleOrderProductValues(OrderProductEntity productEntity)
            {
                ProductEntity productInfo = prductsInformation.First(a => a.ProductId == productEntity.ProductId);
                return new ProductValues(productEntity.ProductId, productInfo.Name, productEntity.Quantity);
            }
        }

        private Task<Result<OrderDto>> BuildOrderDto((DeliveryEntity delivery, ReadOnlyCollection<ProductValues> products) arg, OrderEntity order)
        {
            return new OrderDto(order.OrderId,
                order.CreationTimeUtc,
                arg.delivery.ToDto(),
                arg.products.Select(a => a.ToProductDto()).ToList())
                .Success().Async();
        }

        private class ProductValues
        {
            private readonly Guid ProductId;
            private readonly string Name;
            private readonly int Qty;

            public ProductValues(Guid productId, string name, int qty)
            {
                ProductId = productId;
                Name = name;
                Qty = qty;
            }

            public ProductInformationDto ToProductDto()
            {
                return new ProductInformationDto(ProductId, Name, Qty);
            }
        }

    }
}
