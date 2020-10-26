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
    public class Test_DeleteOrder
    {
        public class TestState
        {
            public readonly DeleteOrder Subject;
            public readonly Mock<IDeleteOrderDependencies> Dependencies;
            public readonly Guid OrderId;

            public TestState()
            {
                Guid orderId = Guid.NewGuid();
                Guid productId = Guid.NewGuid();


                OrderProductEntity orderProduct = new OrderProductEntity(orderId, productId, 12);
                Mock<IDeleteOrderDependencies> dependencies = new Mock<IDeleteOrderDependencies>();
                dependencies.Setup(a => a.GetOrderFromId(orderId))
                    .Returns(Task.FromResult(new OrderEntity(orderId, DateTime.UtcNow)));
                dependencies.Setup(a => a.GetProductsForOrderId(orderId))
                    .Returns(Task.FromResult(new List<OrderProductEntity> { orderProduct }.AsReadOnly()));
                dependencies.Setup(a => a.UnallocateProducts(It.IsAny<List<(Guid, int)>>())).Returns(Task.CompletedTask);
                dependencies.Setup(a => a.DeleteProductsFromOrder(orderId)).Returns(Task.CompletedTask);
                dependencies.Setup(a => a.DeleteDeliveryFromOrder(orderId)).Returns(Task.CompletedTask);
                dependencies.Setup(a => a.DeleteOrder(orderId)).Returns(Task.CompletedTask);
                dependencies.Setup(a => a.Commit()).Returns(Task.CompletedTask);

                DeleteOrder deleteOrder = new DeleteOrder(dependencies.Object);
                OrderId = orderId;
                Dependencies = dependencies;
                Subject = deleteOrder;
            }

            public void VerifyDependencies()
            {
                Dependencies.Verify(a => a.GetOrderFromId(OrderId));
                Dependencies.Verify(a => a.GetProductsForOrderId(OrderId));
                Dependencies.Verify(a => a.UnallocateProducts(It.IsAny<List<(Guid, int)>>()));
                Dependencies.Verify(a => a.DeleteProductsFromOrder(OrderId));
                Dependencies.Verify(a => a.DeleteDeliveryFromOrder(OrderId));
                Dependencies.Verify(a => a.DeleteOrder(OrderId));
                Dependencies.Verify(a => a.Commit());
            }

        }

        [Fact]
        public async Task Test_DeleteOrder_WhenAllCorrect_Then_OrderId()
        {
            TestState state = new TestState();

            Result<DeleteOrderResponseDto> result = await state.Subject.Execute(state.OrderId);

            Assert.True(result.Success);
            Assert.True(result.Value.Success);
            state.VerifyDependencies();
        }
    }
}
