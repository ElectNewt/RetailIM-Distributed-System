using RetailIM.Data.Repository;
using RetailIM.Model.Entities;
using RetailIM.Services.Orders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace RetailIM.ServicesDependencies.Orders
{
    public class RetrieveSingleOrderServiceDependencies : IRetrieveSingleOrderDependencies
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IDeliveryRepository _deliveryRepo;
        private readonly IOrderProductsRepository _orderProductRepo;
        private readonly IProductRepository _productRepo;

        public RetrieveSingleOrderServiceDependencies(IOrderRepository orderRepo, IDeliveryRepository deliveryRepo, IOrderProductsRepository orderProductRepo,
            IProductRepository productRepo)
        {
            _orderRepo = orderRepo;
            _deliveryRepo = deliveryRepo;
            _orderProductRepo = orderProductRepo;
            _productRepo = productRepo;
        }
        public async Task<DeliveryEntity> GetDeliveryFromOrderId(Guid orderId)
            => await _deliveryRepo.GetDeliveryFromOrderId(orderId);

        public async Task<OrderEntity?> GetOrderFromId(Guid orderId)
            => await _orderRepo.GetSingle(orderId);

        public async Task<ReadOnlyCollection<ProductEntity>> GetProductsById(List<Guid> productsIds)
            => await _productRepo.GetByIds(productsIds);

        public async Task<ReadOnlyCollection<OrderProductEntity>> GetProductsForOrderId(Guid orderId)
            => await _orderProductRepo.GetByOrderId(orderId);
    }
}
