using RetailIM.Data.Repository;
using RetailIM.Model.Entities;
using RetailIM.Services.Orders;
using System;
using System.Threading.Tasks;

namespace RetailIM.ServicesDependencies.Orders
{
    public class UpdateOrderDeliveryServiceDependencies : IUpdateOrderDeliveryDependencies
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IDeliveryRepository _deliveryRepo;

        public UpdateOrderDeliveryServiceDependencies(IOrderRepository orderRepo, IDeliveryRepository deliveryRepo)
        {
            _orderRepo = orderRepo;
            _deliveryRepo = deliveryRepo;
        }

        public async Task Commit()
        {
            await _orderRepo.CommitTransaction();
            await _deliveryRepo.CommitTransaction();
        }

        public async Task<OrderEntity?> GetOrderFromId(Guid orderId)
            => await _orderRepo.GetSingle(orderId);

        public async Task UpdateDeliveryAddress(Guid orderId, DeliveryEntity delivery)
            => await _deliveryRepo.UpdateOrderAddress(orderId, delivery);
    }
}
