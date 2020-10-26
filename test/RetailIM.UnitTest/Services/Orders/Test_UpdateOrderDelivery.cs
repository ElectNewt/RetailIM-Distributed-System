using Moq;
using RetailIM.Model;
using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using RetailIM.Model.Mappers;
using RetailIM.Services.Orders;
using Shared.ROP;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RetailIM.UnitTest.Services.Orders
{
    public class Test_UpdateOrderDelivery
    {
        public class TestState
        {
            public readonly Mock<IUpdateOrderDeliveryDependencies> Dependencies;
            public readonly UpdateOrderDelivery Subject;
            public readonly OrderDelivery DefaultOrderDelivery;

            public TestState()
            {
                Guid orderId = Guid.NewGuid();
                Guid originalDeliveryId = Guid.NewGuid();
                DeliveryDto newDelivery = new DeliveryDto("IE", "Dub", "street1");

                Mock<IUpdateOrderDeliveryDependencies> dependencies = new Mock<IUpdateOrderDeliveryDependencies>();

                dependencies.Setup(a => a.GetOrderFromId(orderId))
                    .Returns(Task.FromResult(new OrderEntity(orderId, DateTime.UtcNow)));

                dependencies.Setup(a => a.UpdateDeliveryAddress(orderId, newDelivery.ToEntity()))
                    .Returns(Task.CompletedTask);

                Dependencies = dependencies;
                Subject = new UpdateOrderDelivery(dependencies.Object);
                DefaultOrderDelivery = new OrderDelivery(orderId, newDelivery);

            }

            public void VerifyDependencies()
            {
                Dependencies.Verify(a => a.GetOrderFromId(It.IsAny<Guid>()));
                Dependencies.Verify(a => a.UpdateDeliveryAddress(It.IsAny<Guid>(), It.IsAny<DeliveryEntity>()));
            }

        }

        [Fact]
        public async Task Test_UpdateOrderDelivery_WhenAllCorrect_Then_True()
        {
            TestState state = new TestState();

            Result<UpdateOrderDeliveryResponseDto> result = await state.Subject.Execute(state.DefaultOrderDelivery);

            Assert.True(result.Success);

            Assert.True(result.Value.Success);

            state.VerifyDependencies();
        }

        [Fact]
        public async Task Test_CreationJob_WhenStockIsWrong_ThenError()
        {
            TestState state = new TestState();

            state.Dependencies.Setup(a => a.GetOrderFromId(It.IsAny<Guid>()))
                    .Returns(Task.FromResult((OrderEntity)null));

            Result<UpdateOrderDeliveryResponseDto> result = await state.Subject.Execute(state.DefaultOrderDelivery);

            Assert.False(result.Success);
            Assert.Single(result.Errors);
        }
    }
}
