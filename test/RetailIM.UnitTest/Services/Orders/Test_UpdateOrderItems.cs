using Moq;
using RetailIM.Model;
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
    public class Test_UpdateOrderItems
    {

        public class TestState
        {
            public readonly Mock<IUpdateOrderItemsDependencies> Dependencies;
            public readonly UpdateOrderItems Subject;
            public readonly Guid OrderId;
            public readonly Guid ProductInOrder;

            public TestState()
            {
                Mock<IUpdateOrderItemsDependencies> dependencies = new Mock<IUpdateOrderItemsDependencies>();
                Guid orderId = Guid.NewGuid();

                Guid productId = Guid.NewGuid();

                dependencies.Setup(a => a.GetOrderFromId(orderId))
                    .Returns(Task.FromResult(new OrderEntity(orderId, DateTime.UtcNow)));

                dependencies.Setup(a => a.GetProductsForOrderId(orderId))
                    .Returns(Task.FromResult(new List<OrderProductEntity>() { new OrderProductEntity(orderId, productId, 3) }
                    .AsReadOnly()));

                dependencies.Setup(a => a.GetProductsById(It.IsAny<List<Guid>>()))
                    .Returns(Task.FromResult(new List<ProductEntity>() { new ProductEntity(productId, "name", 100, 25) }
                    .AsReadOnly()));

                dependencies.Setup(a => a.DeleteProductsFromOrder(orderId))
                    .Returns(Task.CompletedTask);

                dependencies.Setup(a => a.UnallocateProducts(It.IsAny<List<(Guid, int)>>()))
                    .Returns(Task.CompletedTask);

                dependencies.Setup(a => a.InsertOrderProducts(It.IsAny<List<(Guid, int)>>(), orderId))
                    .Returns(Task.CompletedTask);

                dependencies.Setup(a => a.AllocateProducts(It.IsAny<List<(Guid, int)>>()))
                    .Returns(Task.CompletedTask);

                dependencies.Setup(a => a.Commit())
                    .Returns(Task.CompletedTask);

                UpdateOrderItems orderItems = new UpdateOrderItems(dependencies.Object);
                Subject = orderItems;
                OrderId = orderId;
                Dependencies = dependencies;
                ProductInOrder = productId;
            }

            public void VerifyDependencies()
            {
                Dependencies.Verify(a => a.GetOrderFromId(OrderId));
                Dependencies.Verify(a => a.GetProductsForOrderId(OrderId));
                Dependencies.Verify(a => a.GetProductsById(It.IsAny<List<Guid>>()));
                Dependencies.Verify(a => a.DeleteProductsFromOrder(OrderId));
                Dependencies.Verify(a => a.UnallocateProducts(It.IsAny<List<(Guid, int)>>()));
                Dependencies.Verify(a => a.InsertOrderProducts(It.IsAny<List<(Guid, int)>>(), OrderId));
                Dependencies.Verify(a => a.AllocateProducts(It.IsAny<List<(Guid, int)>>()));
                Dependencies.Verify(a => a.Commit());
            }

        }

        [Fact]
        public async Task Test_UpdateOrderItems_WhenAllCorrect_Then_True()
        {
            TestState state = new TestState();

            OrderItems order = new OrderItems(
                state.OrderId,
                new List<ProductQuantityDto>() {
                    new ProductQuantityDto(){ProductId = state.ProductInOrder, Quantity = 10 }
                }
            );

            Result<UpdateProductsResponseDto> result = await state.Subject.Execute(order);

            Assert.True(result.Success);
            Assert.True(result.Value.Success);
            state.VerifyDependencies();

        }

        [Fact]
        public async Task Test_UpdateOrderItems_WhenMoreItems_ThanInStock_then_Error()
        {
            TestState state = new TestState();

            OrderItems order = new OrderItems(
                state.OrderId,
                new List<ProductQuantityDto>() {
                    new ProductQuantityDto(){ProductId = state.ProductInOrder, Quantity = 150 }
                }
            );

            Result<UpdateProductsResponseDto> result = await state.Subject.Execute(order);

            Assert.False(result.Success);
            Assert.Single(result.Errors);

        }

    }
}
