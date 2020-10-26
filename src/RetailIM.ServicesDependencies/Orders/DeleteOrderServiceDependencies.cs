using RetailIM.Data.Repository;
using RetailIM.Model.Entities;
using RetailIM.Services.Orders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailIM.ServicesDependencies.Orders
{
    public class DeleteOrderServiceDependencies : IDeleteOrderDependencies
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IDeliveryRepository _deliveryRepo;
        private readonly IOrderProductsRepository _orderProductRepo;
        private readonly IProductRepository _productRepo;

        public DeleteOrderServiceDependencies(IOrderRepository orderRepo, IDeliveryRepository deliveryRepo, IOrderProductsRepository orderProductRepo,
            IProductRepository productRepo)
        {
            _orderRepo = orderRepo;
            _deliveryRepo = deliveryRepo;
            _orderProductRepo = orderProductRepo;
            _productRepo = productRepo;
        }

        public async Task<OrderEntity?> GetOrderFromId(Guid orderId)
            => await _orderRepo.GetSingle(orderId);

        public async Task DeleteDeliveryFromOrder(Guid orderId)
            => await _deliveryRepo.DeleteFromOrderId(orderId);

        public async Task DeleteOrder(Guid orderId)
            => await _orderRepo.DeleteFromId(orderId);

        public async Task DeleteProductsFromOrder(Guid orderId)
            => await _orderRepo.DeleteFromId(orderId);

        public async Task<ReadOnlyCollection<OrderProductEntity>> GetProductsForOrderId(Guid orderId)
            => await _orderProductRepo.GetByOrderId(orderId);

        public async Task UnallocateProducts(List<(Guid, int)> products)
            => await _productRepo.UpdateProductsUnallocation(products);

        public async Task Commit()
        {
            await _orderRepo.CommitTransaction();
            await _deliveryRepo.CommitTransaction();
            await _productRepo.CommitTransaction();
        }
    }
}
