using RetailIM.Model;
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
    public interface IUpdateOrderItemsDependencies
    {
        Task<OrderEntity?> GetOrderFromId(Guid orderId);
        Task<ReadOnlyCollection<OrderProductEntity>> GetProductsForOrderId(Guid orderId);
        Task<ReadOnlyCollection<ProductEntity>> GetProductsById(List<Guid> productsIds);
        Task DeleteProductsFromOrder(Guid orderId);
        Task UnallocateProducts(List<(Guid, int)> products);
        Task InsertOrderProducts(List<(Guid, int)> productQuantity, Guid orderId);
        Task AllocateProducts(List<(Guid, int)> productQuantity);
        Task Commit();
    }

    public class UpdateOrderItems
    {
        private readonly IUpdateOrderItemsDependencies _dependencies;

        public UpdateOrderItems(IUpdateOrderItemsDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<UpdateProductsResponseDto>> Execute(OrderItems order)
        {

            return await CheckOrderExists(order.OrderId)
                .Bind(GetOrderInformation)
                .Bind(x => ValidateProducts(x, EnsureItems(order)))
                .Bind(CleanupOldItems)
                .Bind(UnallocateProducts)
                .Bind(InsertOrderProducts)
                .Bind(AllocateProductsInItemTable)
                .Ignore()
                .Bind(ConfirmChanges)
                .MapAsync(ToResponseDto);

            UpdateProductsResponseDto ToResponseDto(bool success)
            {
                return new UpdateProductsResponseDto() { Success = success };
            }

            OrderItems EnsureItems(OrderItems order)
            {
                return new OrderItems(order.OrderId, order.ProductsQty.EnsureUniqueItems());
            }
        }

        private async Task<Result<Guid>> CheckOrderExists(Guid orderId)
        {
            OrderEntity? order = await _dependencies.GetOrderFromId(orderId);
            if (order == null)
                return Result.Failure<Guid>("The order you are looking for does not exist");

            return orderId;
        }

        private async Task<Result<ReadOnlyCollection<OrderProductEntity>>> GetOrderInformation(Guid orderId)
        {
            return await _dependencies.GetProductsForOrderId(orderId);
        }

        private async Task<Result<UpdateOrderProductsValues>> ValidateProducts(ReadOnlyCollection<OrderProductEntity> orderProducts, OrderItems order)
        {
            ReadOnlyCollection<ProductEntity> products = await _dependencies.GetProductsById(order.ProductsQty.Select(a => a.ProductId).ToList());

            return order.ProductsQty
                    .Select(ValidateUpdateProduct)
                    .Traverse()
                    .Bind(BuildUpdate);

            Result<ProductQuantityDto> ValidateUpdateProduct(ProductQuantityDto productQty)
            {
                return productQty.ValidateUpdate(orderProducts, products);
            }

            Result<UpdateOrderProductsValues> BuildUpdate(List<ProductQuantityDto> newItems)
            {
                return new UpdateOrderProductsValues(order.OrderId,
                    newItems.AsReadOnly(),
                    orderProducts.Select(a => a.ToProductQuantityDto()).ToList().AsReadOnly());
            }

        }

        private async Task<Result<UpdateOrderProductsValues>> CleanupOldItems(UpdateOrderProductsValues updateOrderItems)
        {
            await _dependencies.DeleteProductsFromOrder(updateOrderItems.OrderId);

            return updateOrderItems;
        }

        private async Task<Result<UpdateOrderProductsValues>> UnallocateProducts(UpdateOrderProductsValues updateOrderItems)
        {
            await _dependencies.UnallocateProducts(updateOrderItems.CleanupItems.Select(a => (a.ProductId, a.Quantity)).ToList());
            return updateOrderItems;
        }

        private async Task<Result<UpdateOrderProductsValues>> InsertOrderProducts(UpdateOrderProductsValues updateOrderItems)
        {
            await _dependencies.InsertOrderProducts(updateOrderItems.NewItems
                .Select(a => (a.ProductId, a.Quantity)).ToList(), updateOrderItems.OrderId);

            return updateOrderItems;
        }

        private async Task<Result<UpdateOrderProductsValues>> AllocateProductsInItemTable(UpdateOrderProductsValues updateOrderItems)
        {
            await _dependencies.AllocateProducts(updateOrderItems.NewItems
                .Select(a => (a.ProductId, a.Quantity)).ToList());

            return updateOrderItems;
        }

        private async Task<Result<bool>> ConfirmChanges(Unit _)
        {
            await _dependencies.Commit();
            return true;
        }


        private class UpdateOrderProductsValues
        {
            public readonly Guid OrderId;
            public readonly ReadOnlyCollection<ProductQuantityDto> NewItems;
            public readonly ReadOnlyCollection<ProductQuantityDto> CleanupItems;

            public UpdateOrderProductsValues(Guid orderId, ReadOnlyCollection<ProductQuantityDto> newItems, ReadOnlyCollection<ProductQuantityDto> cleanupItems)
            {
                OrderId = orderId;
                NewItems = newItems;
                CleanupItems = cleanupItems;
            }
        }
    }
}
