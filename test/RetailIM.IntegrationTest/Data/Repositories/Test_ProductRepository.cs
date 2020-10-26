using Microsoft.Extensions.DependencyInjection;
using RetailIM.Data.Repository;
using RetailIM.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetailIM.IntegrationTest.Data.Repositories
{
    public class Test_ProductRepository : TestBaseDependencies
    {
        [Fact]
        public async Task TestInsert_ThenGet_ThenDelete()
        {
            IServiceCollection services = BuildDependencies();
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                ProductEntity product1 = new ProductEntity(Guid.NewGuid(), "product1", 100, 10);
                ProductEntity product2 = new ProductEntity(Guid.NewGuid(), "product2", 100, 90);
                var productRepo = serviceProvider.GetRequiredService<IProductRepository>();
                await productRepo.InsertSingle(product1);
                await productRepo.InsertSingle(product2);

                List<(Guid, int)> allocation = new List<(Guid, int)>()
                {
                    (product1.ProductId, 5)
                };
                List<(Guid, int)> unallocation = new List<(Guid, int)>()
                {
                    (product2.ProductId, 5)
                };
                await productRepo.UpdateProductsAllocation(allocation);
                await productRepo.UpdateProductsUnallocation(unallocation);

                ReadOnlyCollection<ProductEntity> products = await productRepo
                    .GetByIds(new List<Guid> { product1.ProductId, product2.ProductId });


                Assert.Equal(2, products.Count);
                ProductEntity p1 = products.First(a => a.ProductId == product1.ProductId);
                ProductEntity p2 = products.First(a => a.ProductId == product2.ProductId);

                Assert.Equal(15, p1.AllocatedStock);
                Assert.Equal(85, p2.AllocatedStock);

                await productRepo.DeleteAllProducts();


            }
        }
    }
}
