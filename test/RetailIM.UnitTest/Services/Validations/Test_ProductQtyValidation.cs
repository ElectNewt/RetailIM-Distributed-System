using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using RetailIM.Services.Validations;
using Shared.ROP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace RetailIM.UnitTest.Services.Validations
{
    public class Test_ProductQtyValidation
    {
        [Fact]
        public void Test_EnsureUniqueItems()
        {
            ProductQuantityDto product1 = new ProductQuantityDto() { ProductId = Guid.NewGuid(), Quantity = 10 };
            ProductQuantityDto product2 = new ProductQuantityDto() { ProductId = Guid.NewGuid(), Quantity = 10 };
            ProductQuantityDto product3 = new ProductQuantityDto() { ProductId = product1.ProductId, Quantity = 10 };

            List<ProductQuantityDto> OriginalListProduct = new List<ProductQuantityDto>() { product1, product2, product3 };

            var result = OriginalListProduct.EnsureUniqueItems();

            Assert.Equal(2, result.Count());
            Assert.Equal(20, result.FirstOrDefault(a => a.ProductId == product1.ProductId).Quantity);
            Assert.Equal(10, result.FirstOrDefault(a => a.ProductId == product2.ProductId).Quantity);
        }

        [Fact]
        public void Test_ValidateInsertion_ItemExist_andEnoughStock()
        {
            Guid product1Id = Guid.NewGuid();
            ProductQuantityDto product1 = new ProductQuantityDto() { ProductId = product1Id, Quantity = 10 };
            ReadOnlyCollection<ProductEntity> dbproducts = BuildDatabaseProducts(product1Id, 100, 10);

            Result<ProductQuantityDto> result = product1.ValidateInsertion(dbproducts);

            Assert.True(result.Success);
            Assert.Equal(product1, result.Value);

        }

        [Fact]
        public void Test_ValidateInsertion_ItemExist_ButNotEnoughStock()
        {
            Guid product1Id = Guid.NewGuid();
            ProductQuantityDto product1 = new ProductQuantityDto() { ProductId = product1Id, Quantity = 10 };
            ReadOnlyCollection<ProductEntity> dbproducts = BuildDatabaseProducts(product1Id, 100, 91);

            Result<ProductQuantityDto> result = product1.ValidateInsertion(dbproducts);

            Assert.False(result.Success);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Test_ValidateInsertion_ItemDoesNotExist()
        {
            Guid product1Id = Guid.NewGuid();
            ProductQuantityDto product1 = new ProductQuantityDto() { ProductId = product1Id, Quantity = 10 };
            ReadOnlyCollection<ProductEntity> dbproducts = BuildDatabaseProducts(Guid.NewGuid(), 100, 91);

            Result<ProductQuantityDto> result = product1.ValidateInsertion(dbproducts);

            Assert.False(result.Success);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void Test_Validateupdate_ItemDoesNotExist()
        {
            Guid product1Id = Guid.NewGuid();
            ProductQuantityDto product1 = new ProductQuantityDto() { ProductId = product1Id, Quantity = 10 };
            ReadOnlyCollection<ProductEntity> dbproducts = BuildDatabaseProducts(Guid.NewGuid(), 100, 0);
            ReadOnlyCollection<OrderProductEntity> currentOrder = GetCurrentOrderProducts();

            Result<ProductQuantityDto> result = product1.ValidateUpdate(currentOrder, dbproducts);

            Assert.False(result.Success);
            Assert.Single(result.Errors);

            ReadOnlyCollection<OrderProductEntity> GetCurrentOrderProducts()
            {
                OrderProductEntity p1 = new OrderProductEntity(Guid.NewGuid(), product1Id, 3);
                return new List<OrderProductEntity> { p1, }.AsReadOnly();
            }
        }

        [Fact]
        public void Test_Validateupdate_ItemExist_andEnoughStock()
        {
            Guid product1Id = Guid.NewGuid();
            ProductQuantityDto product1 = new ProductQuantityDto() { ProductId = product1Id, Quantity = 10 };
            ReadOnlyCollection<ProductEntity> dbproducts = BuildDatabaseProducts(product1Id, 100, 0);
            ReadOnlyCollection<OrderProductEntity> currentOrder = GetCurrentOrderProducts();

            Result<ProductQuantityDto> result = product1.ValidateUpdate(currentOrder, dbproducts);

            Assert.True(result.Success);
            Assert.Equal(product1, result.Value);

            ReadOnlyCollection<OrderProductEntity> GetCurrentOrderProducts()
            {
                OrderProductEntity p1 = new OrderProductEntity(product1Id, product1Id, 3);
                return new List<OrderProductEntity> { p1, }.AsReadOnly();
            }
        }


        [Fact]
        public void Test_Validateupdate_ItemExist_andEnoughStock_noItemAlreadyInOrder()
        {
            Guid product1Id = Guid.NewGuid();
            ProductQuantityDto product1 = new ProductQuantityDto() { ProductId = product1Id, Quantity = 10 };
            ReadOnlyCollection<ProductEntity> dbproducts = BuildDatabaseProducts(product1Id, 100, 0);
            ReadOnlyCollection<OrderProductEntity> currentOrder = GetCurrentOrderProducts();

            Result<ProductQuantityDto> result = product1.ValidateUpdate(currentOrder, dbproducts);

            Assert.True(result.Success);
            Assert.Equal(product1, result.Value);

            ReadOnlyCollection<OrderProductEntity> GetCurrentOrderProducts()
            {
                OrderProductEntity p1 = new OrderProductEntity(Guid.NewGuid(), product1Id, 3);
                return new List<OrderProductEntity> { p1, }.AsReadOnly();
            }
        }

        [Fact]
        public void Test_Validateupdate_ItemExist_ButNoStock()
        {
            Guid product1Id = Guid.NewGuid();
            ProductQuantityDto product1 = new ProductQuantityDto() { ProductId = product1Id, Quantity = 10 };
            ReadOnlyCollection<ProductEntity> dbproducts = BuildDatabaseProducts(product1Id, 100, 100);
            ReadOnlyCollection<OrderProductEntity> currentOrder = GetCurrentOrderProducts();

            Result<ProductQuantityDto> result = product1.ValidateUpdate(currentOrder, dbproducts);

            Assert.False(result.Success);

            ReadOnlyCollection<OrderProductEntity> GetCurrentOrderProducts()
            {
                OrderProductEntity p1 = new OrderProductEntity(Guid.NewGuid(), product1Id, 3);
                return new List<OrderProductEntity> { p1, }.AsReadOnly();
            }
        }


        private ReadOnlyCollection<ProductEntity> BuildDatabaseProducts(Guid existingProduct, int stock, int allocatedStock)
        {
            ProductEntity p1 = new ProductEntity(existingProduct, "p1", stock, allocatedStock);
            ProductEntity p2 = new ProductEntity(Guid.NewGuid(), "p1", 100, 0);
            return new List<ProductEntity> { p1, p2 }.AsReadOnly();
        }
    }
}
