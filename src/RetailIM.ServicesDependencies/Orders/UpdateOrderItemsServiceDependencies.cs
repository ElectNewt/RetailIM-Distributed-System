using RetailIM.Data.Repository;
using RetailIM.Model.Entities;
using RetailIM.Services.Orders;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace RetailIM.ServicesDependencies.Orders
{
    public class UpdateOrderItemsServiceDependencies : IUpdateOrderItemsDependencies
    {
        private readonly IOrderProductsRepository _orderProductRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;

        public UpdateOrderItemsServiceDependencies(IOrderProductsRepository orderProductRepo, IProductRepository productRepo,
            IOrderRepository orderRepo)
        {
            _orderProductRepo = orderProductRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        public async Task<OrderEntity?> GetOrderFromId(Guid orderId)
            => await _orderRepo.GetSingle(orderId);

        public async Task AllocateProducts(List<(Guid, int)> productQuantity)
            => await _productRepo.UpdateProductsAllocation(productQuantity);

        public async Task DeleteProductsFromOrder(Guid orderId)
            => await _orderProductRepo.DeleteProductsFromOrder(orderId);

        public async Task<ReadOnlyCollection<ProductEntity>> GetProductsById(List<Guid> productsIds)
            => await _productRepo.GetByIds(productsIds);

        public async Task<ReadOnlyCollection<OrderProductEntity>> GetProductsForOrderId(Guid orderId)
            => await _orderProductRepo.GetByOrderId(orderId);

        public async Task InsertOrderProducts(List<(Guid, int)> productQuantity, Guid orderId)
            => await _orderProductRepo.InsertOrderProducts(orderId, productQuantity);

        public async Task UnallocateProducts(List<(Guid, int)> products)
            => await _productRepo.UpdateProductsUnallocation(products);

        public async Task Commit()
        {
            await _orderProductRepo.CommitTransaction();
            await _productRepo.CommitTransaction();
        }
    }
}
