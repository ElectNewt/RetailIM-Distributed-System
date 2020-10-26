using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using RetailIM.Model.Mappers;
using RetailIM.Services.Validations;
using Shared.ROP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetailIM.Services.Orders
{
    public interface ICreateOrderDependencies
    {
        Task<ReadOnlyCollection<ProductEntity>> GetProductsById(List<Guid> productsIds);
        Task InsertDelivery(DeliveryEntity delivery, Guid orderId);
        Task InsertOrderProducts(List<(Guid, int)> productQuantity, Guid orderId);
        Task<Guid> InsertOrder();
        Task AllocateProducts(List<(Guid, int)> productQuantity);
        Task Commit();

    }

    public class CreateOrder
    {
        private readonly ICreateOrderDependencies _dependencies;

        public CreateOrder(ICreateOrderDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<OrderCreationResponseDto>> Execute(OrderCreationDto order)
        {
            return await EnsureItems(order)
                .Async()
                .Bind(ValidateOrder)
                .Bind(InsertOrder)
                .Bind(InsertDelivery)
                .Bind(InsertOrderProducts)
                .Bind(AllocateProductsInItemTable)
                .Bind(ConfirmChanges)
                .MapAsync(ToResponseDto);

            OrderCreationResponseDto ToResponseDto(Guid orderId)
            {
                return new OrderCreationResponseDto() { OrderId = orderId };
            }


        }

        private Result<OrderCreationDto> EnsureItems(OrderCreationDto order)
        {
            return new OrderCreationDto() { Delivery = order.Delivery, Products = order.Products.EnsureUniqueItems() };
        }

        private async Task<Result<OrderCreationDto>> ValidateOrder(OrderCreationDto order)
        {

            return await order.Delivery.Validate()
                .Ignore()
                .Async()
                 .Bind(_ => ValidateProducts(order.Products))
                 .MapAsync(_ => order);

            async Task<Result<Unit>> ValidateProducts(List<ProductQuantityDto> productsQty)
            {
                ReadOnlyCollection<ProductEntity> products = await _dependencies.GetProductsById(productsQty.Select(a => a.ProductId).ToList());

                return productsQty
                    .Select(ValidateSingleProduct)
                    .Traverse()
                    .Ignore();

                Result<ProductQuantityDto> ValidateSingleProduct(ProductQuantityDto productQty)
                {
                    return productQty.ValidateInsertion(products);
                }
            }
        }

        private async Task<Result<(OrderCreationDto, Guid)>> InsertDelivery((OrderCreationDto order, Guid orderid) args)
        {
            await _dependencies.InsertDelivery(args.order.Delivery.ToEntity(), args.orderid);
            return args;
        }

        private async Task<Result<(OrderCreationDto, Guid)>> InsertOrderProducts((OrderCreationDto order, Guid orderId) args)
        {
            await _dependencies.InsertOrderProducts(args.order.Products
               .Select(a => (a.ProductId, a.Quantity)).ToList(), args.orderId);

            return args;
        }

        private async Task<Result<(OrderCreationDto, Guid)>> InsertOrder(OrderCreationDto order)
        {
            Guid orderId = await _dependencies.InsertOrder();

            return (order, orderId);
        }

        private async Task<Result<Guid>> AllocateProductsInItemTable((OrderCreationDto order, Guid orderId) args)
        {
            await _dependencies.AllocateProducts(args.order.Products
                .Select(a => (a.ProductId, a.Quantity)).ToList());

            return args.orderId;
        }

        private async Task<Result<Guid>> ConfirmChanges(Guid orderId)
        {
            await _dependencies.Commit();
            return orderId;
        }
    }
}
