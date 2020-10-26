using Microsoft.Extensions.DependencyInjection;
using RetailIM.Data.Repository;
using RetailIM.Model.Entities;
using System;
using System.Threading.Tasks;
using Xunit;


namespace RetailIM.IntegrationTest.Data.Repositories
{
    public class Test_DeliveryRepository : TestBaseDependencies
    {


        [Fact]
        public async Task TestInsert_ThenGet_ThenDelete()
        {
            IServiceCollection services = BuildDependencies();
            using (ServiceProvider serviceProvider = services.BuildServiceProvider())
            {
                var deliveryRepo = serviceProvider.GetRequiredService<IDeliveryRepository>();
                Guid orderId = Guid.NewGuid();
                DeliveryEntity deliveryEntity = new DeliveryEntity(orderId, "IE", "Dub", "Street");
                await deliveryRepo.InsertSingle(deliveryEntity, orderId);
                DeliveryEntity del = await deliveryRepo.GetDeliveryFromOrderId(orderId);
                Assert.Equal(deliveryEntity.City, del.City);

                DeliveryEntity updatedDelivery = new DeliveryEntity(orderId, "IE", "Dub", "Street2");
                await deliveryRepo.UpdateOrderAddress(orderId, updatedDelivery);
                DeliveryEntity del2 = await deliveryRepo.GetDeliveryFromOrderId(orderId);
                Assert.Equal(updatedDelivery.City, del2.City);

                await deliveryRepo.DeleteFromOrderId(orderId);

                await deliveryRepo.CommitTransaction();
            }
        }

    }
}
