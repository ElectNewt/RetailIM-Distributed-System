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
    public class Test_RetrievePaginatedOrders
    {

        public class TestState
        {
            public readonly RetrievePaginatedOrders Subject;
            public readonly Mock<IRetrievePaginatedOrdersDependencies> Dependencies;
            public readonly Guid OrderId;

            public TestState()
            {
                Mock<IRetrievePaginatedOrdersDependencies> dependencies = new Mock<IRetrievePaginatedOrdersDependencies>();

                Guid orderId = Guid.NewGuid();

                dependencies.Setup(a => a.GetOrders(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(Task.FromResult(new List<OrderEntity>() { new OrderEntity(orderId, DateTime.UtcNow) }.AsReadOnly()));

                dependencies.Setup(a => a.GetDeliveryFromOrderId(orderId))
                    .Returns(Task.FromResult(new DeliveryEntity(Guid.NewGuid(), "IE", "dub", "street1")));

                Guid productId = Guid.NewGuid();

                dependencies.Setup(a => a.GetProductsForOrderId(orderId))
                    .Returns(Task.FromResult(new List<OrderProductEntity>() { new OrderProductEntity(orderId, productId, 5) }
                    .AsReadOnly()));

                dependencies.Setup(a => a.GetProductsById(new List<Guid>() { productId }))
                    .Returns(Task.FromResult(new List<ProductEntity>() { new ProductEntity(productId, "ProdName", 124, 10) }
                    .AsReadOnly()));

                RetrievePaginatedOrders paginatedOrders = new RetrievePaginatedOrders(dependencies.Object);
                Subject = paginatedOrders;
                Dependencies = dependencies;
                OrderId = orderId;

            }

            public void VerifyDependencies()
            {
                Dependencies.Verify(a => a.GetOrders(It.IsAny<int>(), It.IsAny<int>()));
                Dependencies.Verify(a => a.GetDeliveryFromOrderId(OrderId));
                Dependencies.Verify(a => a.GetProductsForOrderId(OrderId));
                Dependencies.Verify(a => a.GetProductsById(It.IsAny<List<Guid>>()));
            }
        }

        [Fact]
        public async Task Test_RetrievePaginatedOrders_WhenAllCorrect_Then_FullOrderList()
        {
            TestState state = new TestState();

            Result<PaginatedOrdersResponseDto> result = await state.Subject.Execute(1);

            Assert.True(result.Success);
            Assert.Single(result.Value.Orders);
            OrderDto order = result.Value.Orders.First();

            Assert.Equal(state.OrderId, order.OrderId);
            Assert.Equal("street1", order.DeliveryDto.Street);
            Assert.Equal("ProdName", order.Products.First().Name);
            Assert.Equal(5, order.Products.First().Quantity);

            state.VerifyDependencies();
        }

    }
}
