using Microsoft.Extensions.DependencyInjection;
using RetailIM.Data.Repository;
using RetailIM.Model.Entities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace RetailIM.IntegrationTest.Data.Repositories
{
    public class Test_OrderRepository : TestBaseDependencies
    {
        [Fact]
        public async Task TestInsert_ThenGet_ThenDelete()
        {
            IServiceCollection services = BuildDependencies();
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var orderRepo = serviceProvider.GetRequiredService<IOrderRepository>();

                Guid order1 = await orderRepo.InsertSingle();
                Guid order2 = await orderRepo.InsertSingle();

                OrderEntity orderEntity = await orderRepo.GetSingle(order1);
                Assert.Equal(order1, orderEntity.OrderId);

                await orderRepo.DeleteFromId(order1);
                ReadOnlyCollection<OrderEntity> ordersInSystem = await orderRepo.GetPaginated(1, 5);
                Assert.Single(ordersInSystem);
                Assert.Equal(order2, ordersInSystem.First().OrderId);
                await orderRepo.DeleteFromId(order2);

                await orderRepo.CommitTransaction();
            }
        }
    }
}
