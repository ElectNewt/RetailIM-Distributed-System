using Moq;
using RetailIM.Model.Dto;
using RetailIM.Model.Entities;
using RetailIM.Services.Orders;
using Shared.ROP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace RetailIM.UnitTest.Services.Orders
{
    public class Test_RetrieveSingleOrder
    {
        public class TestState
        {
            public readonly RetrieveSingleOrder Subject;
            public readonly Mock<IRetrieveSingleOrderDependencies> Dependencies;
            public readonly Guid OrderId;

            public TestState()
            {

                Guid orderId = Guid.NewGuid();

                Mock<IRetrieveSingleOrderDependencies> dependencies = new Mock<IRetrieveSingleOrderDependencies>();

                dependencies.Setup(a => a.GetOrderFromId(orderId))
                    .Returns(Task.FromResult(new OrderEntity(orderId, DateTime.UtcNow)));

                dependencies.Setup(a => a.GetDeliveryFromOrderId(orderId))
                    .Returns(Task.FromResult(new DeliveryEntity(Guid.NewGuid(), "IE", "dub", "street1")));

                Guid productId = Guid.NewGuid();

                dependencies.Setup(a => a.GetProductsForOrderId(orderId))
                    .Returns(Task.FromResult(new List<OrderProductEntity>() { new OrderProductEntity(orderId, productId, 5) }
                    .AsReadOnly()));

                dependencies.Setup(a => a.GetProductsById(new List<Guid>() { productId }))
                    .Returns(Task.FromResult(new List<ProductEntity>() { new ProductEntity(productId, "ProdName", 124, 10) }
                    .AsReadOnly()));


                RetrieveSingleOrder retrieveSingleOrder = new RetrieveSingleOrder(dependencies.Object);
                Subject = retrieveSingleOrder;
                OrderId = orderId;
                Dependencies = dependencies;

            }

            public void VerifyDependencies()
            {
                Dependencies.Verify(a => a.GetOrderFromId(OrderId));
                Dependencies.Verify(a => a.GetDeliveryFromOrderId(OrderId));
                Dependencies.Verify(a => a.GetProductsForOrderId(OrderId));
                Dependencies.Verify(a => a.GetProductsById(It.IsAny<List<Guid>>()));
            }
        }

        [Fact]
        public async Task Test_RetrieveSingleOrder_WhenAllCorrect_Then_FullOrder()
        {
            TestState state = new TestState();

            Result<GetOrderResponseDto> result = await state.Subject.Execute(state.OrderId);

            Assert.True(result.Success);
            OrderDto order = result.Value.Order;

            Assert.Equal(state.OrderId, order.OrderId);
            Assert.Equal("street1", order.DeliveryDto.Street);
            Assert.Equal("ProdName", order.Products.First().Name);
            Assert.Equal(5, order.Products.First().Quantity);

            state.VerifyDependencies();
        }


        [Fact]
        public async Task Test_RetrieveSingleOrder_WhenOrderDoesNotExit_thenError()
        {
            TestState state = new TestState();
            state.Dependencies.Setup(a => a.GetOrderFromId(state.OrderId))
                   .Returns(Task.FromResult((OrderEntity)null));

            Result<GetOrderResponseDto> result = await state.Subject.Execute(state.OrderId);

            Assert.False(result.Success);
            Assert.Single(result.Errors);
        }

    }
}
