using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using Shared.ROP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RetailIM.Services.Orders
{
    public interface IDeleteOrderDependencies
    {
        Task<OrderEntity?> GetOrderFromId(Guid orderId);
        Task<ReadOnlyCollection<OrderProductEntity>> GetProductsForOrderId(Guid orderId);
        Task UnallocateProducts(List<(Guid, int)> products);
        Task DeleteProductsFromOrder(Guid orderId);
        Task DeleteDeliveryFromOrder(Guid orderId);
        Task DeleteOrder(Guid orderId);
        Task Commit();
    }

    public class DeleteOrder
    {
        private readonly IDeleteOrderDependencies _dependencies;
        public DeleteOrder(IDeleteOrderDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<DeleteOrderResponseDto>> Execute(Guid orderId)
        {
            return await CheckOrderExists(orderId)
                .Bind(GetOrderInformation)
                .Bind(UnallocateProducts)
                .Bind(_ => DeleteProductsInOrder(orderId))
                .Bind(DeleteDelivery)
                .Bind(DeleteOrderInfromation)
                .Ignore()
                .Bind(ConfirmChanges)
                .MapAsync(ToResponseDto);

            DeleteOrderResponseDto ToResponseDto(bool success)
            {
                return new DeleteOrderResponseDto() { Success = success };
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

        private async Task<Result<Unit>> UnallocateProducts(ReadOnlyCollection<OrderProductEntity> orderProducts)
        {
            await _dependencies.UnallocateProducts(orderProducts.Select(a => (a.ProductId, a.Quantity)).ToList());
            return Result.Unit;
        }
        private async Task<Result<Guid>> DeleteProductsInOrder(Guid orderId)
        {
            await _dependencies.DeleteProductsFromOrder(orderId);
            return orderId;
        }

        private async Task<Result<Guid>> DeleteDelivery(Guid orderId)
        {
            await _dependencies.DeleteDeliveryFromOrder(orderId);
            return orderId;
        }
        private async Task<Result<Guid>> DeleteOrderInfromation(Guid orderId)
        {
            await _dependencies.DeleteOrder(orderId);
            return orderId;
        }

        private async Task<Result<bool>> ConfirmChanges(Unit _)
        {
            await _dependencies.Commit();
            return true;
        }

    }
}
