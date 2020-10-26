using RetailIM.Model;
using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using RetailIM.Model.Mappers;
using RetailIM.Services.Validations;
using Shared.ROP;
using System;
using System.Threading.Tasks;

namespace RetailIM.Services.Orders
{
    public interface IUpdateOrderDeliveryDependencies
    {
        Task<OrderEntity?> GetOrderFromId(Guid orderId);
        Task UpdateDeliveryAddress(Guid orderId, DeliveryEntity delivery);
        Task Commit();
    }

    public class UpdateOrderDelivery
    {
        private readonly IUpdateOrderDeliveryDependencies _dependencies;

        public UpdateOrderDelivery(IUpdateOrderDeliveryDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public async Task<Result<UpdateOrderDeliveryResponseDto>> Execute(OrderDelivery order)
        {
            return await ValidateNewAddress(order)
                  .Bind(_ => CheckOrderExists(order.OrderId))
                  .Bind(x => UpdateAddress(x, order.Delivery))
                  .Bind(ConfirmChanges)
                  .MapAsync(ToResponseDto);

            UpdateOrderDeliveryResponseDto ToResponseDto(bool success)
            {
                return new UpdateOrderDeliveryResponseDto() { Success = success };
            }
        }

        private Task<Result<Unit>> ValidateNewAddress(OrderDelivery order)
        {
            return order.Delivery.Validate()
                .Success()
                .Ignore()
                .Async();
        }

        private async Task<Result<Guid>> CheckOrderExists(Guid orderId)
        {
            OrderEntity? order = await _dependencies.GetOrderFromId(orderId);
            if (order == null)
                return Result.Failure<Guid>("The order you are looking for does not exist");

            return orderId;
        }

        private async Task<Result<Unit>> UpdateAddress(Guid deliveryId, DeliveryDto delivery)
        {
            await _dependencies.UpdateDeliveryAddress(deliveryId, delivery.ToEntity());

            return Result.Unit;
        }

        private async Task<Result<bool>> ConfirmChanges(Unit _)
        {
            await _dependencies.Commit();

            return true;
        }
    }
}
