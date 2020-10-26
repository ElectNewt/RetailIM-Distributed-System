using RetailIM.Data.Repository;
using RetailIM.Model.Entities;
using RetailIM.Services.Orders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailIM.ServicesDependencies.Orders
{
    public class CreateOrderServiceDependencies : ICreateOrderDependencies
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IDeliveryRepository _deliveryRepo;
        private readonly IOrderProductsRepository _orderProductRepo;
        private readonly IProductRepository _productRepo;

        public CreateOrderServiceDependencies(IOrderRepository orderRepo, IDeliveryRepository deliveryRepo, IOrderProductsRepository orderProductRepo,
            IProductRepository productRepo)
        {
            _orderRepo = orderRepo;
            _deliveryRepo = deliveryRepo;
            _orderProductRepo = orderProductRepo;
            _productRepo = productRepo;
        }

        public async Task<ReadOnlyCollection<ProductEntity>> GetProductsById(List<Guid> productsIds)
            => await _productRepo.GetByIds(productsIds);

        public async Task InsertDelivery(DeliveryEntity delivery, Guid orderId)
            => await _deliveryRepo.InsertSingle(delivery, orderId);

        public async Task<Guid> InsertOrder()
            => await _orderRepo.InsertSingle();

        public async Task InsertOrderProducts(List<(Guid, int)> productQuantity, Guid orderId)
            => await _orderProductRepo.InsertOrderProducts(orderId, productQuantity);

        public async Task AllocateProducts(List<(Guid, int)> productQuantity)
            => await _productRepo.UpdateProductsAllocation(productQuantity);

        public async Task Commit()
        {
            await _productRepo.CommitTransaction();
            await _deliveryRepo.CommitTransaction();
            await _orderProductRepo.CommitTransaction();
            await _orderRepo.CommitTransaction();
        }
    }
}
