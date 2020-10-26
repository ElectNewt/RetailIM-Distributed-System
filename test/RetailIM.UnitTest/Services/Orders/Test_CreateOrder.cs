using Moq;
using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using RetailIM.Services.Orders;
using Shared.ROP;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RetailIM.UnitTest.Services.Orders
{
    public class Test_CreateOrder
    {

        public class TestState
        {
            public readonly Mock<ICreateOrderDependencies> Dependencies;
            public readonly CreateOrder Subject;
            public readonly OrderCreationDto DefaultOrder;
            public readonly Guid Orderid;
            public readonly Guid ProductId;

            public TestState()
            {

                ProductId = Guid.NewGuid();
                DefaultOrder = createDefaultOrder(ProductId);
                Orderid = Guid.NewGuid();
                Mock<ICreateOrderDependencies> dependencies = new Mock<ICreateOrderDependencies>();
                dependencies.Setup(a => a.GetProductsById(It.IsAny<List<Guid>>()))
                    .Returns(Task.FromResult(new List<ProductEntity>() { new ProductEntity(ProductId, "name", 100, 0) }.AsReadOnly()));
                dependencies.Setup(a => a.InsertDelivery(It.IsAny<DeliveryEntity>(), It.IsAny<Guid>()))
                    .Returns(Task.CompletedTask);
                dependencies.Setup(a => a.InsertOrderProducts(It.IsAny<List<(Guid, int)>>(), It.IsAny<Guid>()))
                    .Returns(Task.CompletedTask);
                dependencies.Setup(a => a.InsertOrder())
                    .Returns(Task.FromResult(Orderid));
                dependencies.Setup(a => a.AllocateProducts(It.IsAny<List<(Guid, int)>>()))
                    .Returns(Task.CompletedTask);
                dependencies.Setup(a => a.Commit())
                    .Returns(Task.CompletedTask);


                CreateOrder createOrder = new CreateOrder(dependencies.Object);
                Dependencies = dependencies;
                Subject = createOrder;


                OrderCreationDto createDefaultOrder(Guid productGuid)
                {
                    List<ProductQuantityDto> productQties = new List<ProductQuantityDto>()
                    {
                        new ProductQuantityDto(){ ProductId = productGuid, Quantity = 20 }
                    };
                    DeliveryDto delivery = new DeliveryDto("IE", "Dublin", "Abbey street");

                    return new OrderCreationDto() { Products = productQties, Delivery = delivery };
                }
            }

            public void VerifyDependencies()
            {
                Dependencies.Verify(a => a.GetProductsById(It.IsAny<List<Guid>>()));
                Dependencies.Verify(a => a.InsertDelivery(It.IsAny<DeliveryEntity>(), It.IsAny<Guid>()));
                Dependencies.Verify(a => a.InsertOrderProducts(It.IsAny<List<(Guid, int)>>(), It.IsAny<Guid>()));
                Dependencies.Verify(a => a.InsertOrder());
                Dependencies.Verify(a => a.AllocateProducts(It.IsAny<List<(Guid, int)>>()));
                Dependencies.Verify(a => a.Commit());
            }
        }




        [Fact]
        public async Task Test_CreationJob_WhenAllCorrect_Then_OrderId()
        {
            TestState state = new TestState();

            Result<OrderCreationResponseDto> result = await state.Subject.Execute(state.DefaultOrder);

            Assert.True(result.Success);

            Assert.Equal(state.Orderid, result.Value.OrderId);

            state.VerifyDependencies();
        }


        [Fact]
        public async Task Test_CreationJob_WhenStockIsWrong_ThenError()
        {
            TestState state = new TestState();

            //Change the dependency so it will not be enough stock.
            state.Dependencies.Setup(a => a.GetProductsById(It.IsAny<List<Guid>>()))
                    .Returns(Task.FromResult(new List<ProductEntity>() { new ProductEntity(state.ProductId, "name", 2, 0) }
                    .AsReadOnly()));


            Result<OrderCreationResponseDto> result = await state.Subject.Execute(state.DefaultOrder);

            Assert.False(result.Success);
            Assert.Single(result.Errors);
        }


    }
}
