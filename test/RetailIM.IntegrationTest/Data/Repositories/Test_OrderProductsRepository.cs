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
    public class Test_OrderProductsRepository : TestBaseDependencies
    {
        [Fact]
        public async Task TestInsert_ThenGet_ThenDelete()
        {
            IServiceCollection services = BuildDependencies();
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                IOrderProductsRepository orderProductsRepo = serviceProvider.GetRequiredService<IOrderProductsRepository>();
                Guid orderId = Guid.NewGuid();
                List<(Guid, int)> insertedProducts1 = new List<(Guid, int)>()
                {
                    (Guid.NewGuid(), 2),
                    (Guid.NewGuid(), 1)
                };
                await orderProductsRepo.InsertOrderProducts(orderId, insertedProducts1);

                ReadOnlyCollection<OrderProductEntity> products1 = await orderProductsRepo.GetByOrderId(orderId);
                Assert.Equal(2, products1.Count);
                Assert.Equal(insertedProducts1.First().Item2, products1.First(a => a.ProductId == insertedProducts1.First().Item1).Quantity);
                Assert.Equal(insertedProducts1.Last().Item2, products1.First(a => a.ProductId == insertedProducts1.Last().Item1).Quantity);


                await orderProductsRepo.DeleteProductsFromOrder(orderId);

                ReadOnlyCollection<OrderProductEntity> products2 = await orderProductsRepo.GetByOrderId(orderId);
                Assert.Empty(products2);

                await orderProductsRepo.CommitTransaction();



            }
        }
    }
}
