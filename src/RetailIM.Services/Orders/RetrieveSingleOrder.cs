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
    public interface IRetrieveSingleOrderDependencies
    {
        Task<OrderEntity?> GetOrderFromId(Guid orderId);
        Task<DeliveryEntity> GetDeliveryFromOrderId(Guid orderId);
        Task<ReadOnlyCollection<OrderProductEntity>> GetProductsForOrderId(Guid orderId);
        Task<ReadOnlyCollection<ProductEntity>> GetProductsById(List<Guid> productsIds);
    }

    public class RetrieveSingleOrder
    {
        private readonly IRetrieveSingleOrderDependencies _dependencies;

        public RetrieveSingleOrder(IRetrieveSingleOrderDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<GetOrderResponseDto>> Execute(Guid orderId)
        {
            return await GetOrderInformation(orderId)
                .Combine(_ => GetDeliveryInformation(orderId))
                .Combine(_ => GetProductsInOrder(orderId))
                .Bind(BuildOrderDto)
                .MapAsync(ToResponseDto);

            GetOrderResponseDto ToResponseDto(OrderDto order)
            {
                return new GetOrderResponseDto() { Order = order };
            }
        }

        private async Task<Result<OrderEntity>> GetOrderInformation(Guid orderId)
        {
            OrderEntity? order = await _dependencies.GetOrderFromId(orderId);
            if (order == null)
                return Result.Failure<OrderEntity>($"The order you are looking ({orderId}) for does not exist");

            return order;
        }
        private async Task<DeliveryEntity> GetDeliveryInformation(Guid orderId)
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

        private Task<Result<OrderDto>> BuildOrderDto((OrderEntity order, DeliveryEntity delivery, ReadOnlyCollection<ProductValues> products) arg)
        {
            return new OrderDto(arg.order.OrderId,
                arg.order.CreationTimeUtc,
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
