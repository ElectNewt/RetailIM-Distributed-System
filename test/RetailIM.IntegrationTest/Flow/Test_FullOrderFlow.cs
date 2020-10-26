using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RetailIM.Data.Repository;
using RetailIM.Model.Dto;
using RetailIM.OrderMS.BackgroundWorkers.Consumers;
using RetailIM.Services.Orders;
using RetailIM.ServicesDependencies.Orders;
using RetailIM.WebApi.Controllers;
using Shared.Common.Serialization;
using Shared.MessageBus;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebPersonal.Shared.Data.Db;
using Xunit;

namespace RetailIM.IntegrationTest.Flow
{
    public class Test_FullOrderFlow
    {
        [Fact]
        public async Task Test_Full_Order_Workflow()
        {
            Guid productId = new Guid("331866d3-25f7-425f-9c75-90f21f5a606c");
            OrderCreationDto orderCreation = new OrderCreationDto()
            {
                Delivery = new DeliveryDto()
                {
                    City = "C1",
                    Country = "country1",
                    Street = "street"
                },
                Products = new List<ProductQuantityDto>()
                {
                    new ProductQuantityDto(){ProductId = productId, Quantity = 1 }
                }
            };
            ServiceProvider serviceProvider = BuildServiceProvider();
            OrderController controller = serviceProvider
                   .GetService<OrderController>();

            //Create the order
            OrderCreationResponseDto orderCreationResponse;
            using (OrderCreationConsumer orderCreationConsumer = serviceProvider.GetService<OrderCreationConsumer>())
            {
                await orderCreationConsumer.StartAsync(CancellationToken.None);
                OperationResultDto<OrderCreationResponseDto> result = await controller.Create(orderCreation);
                orderCreationResponse = result.Result;
            }

            Assert.NotEqual(Guid.NewGuid(), orderCreationResponse.OrderId);


            //Update order Delivery
            UpdateOrderDeliveryResponseDto deliveryResponse;
            DeliveryDto updatedDelivery = new DeliveryDto()
            {
                City = "city",
                Street = "St",
                Country = "cou"
            };
            using (UpdateOrderDeliveryConsumer updateOrderconsumer = serviceProvider.GetService<UpdateOrderDeliveryConsumer>())
            {
                await updateOrderconsumer.StartAsync(CancellationToken.None);
                OperationResultDto<UpdateOrderDeliveryResponseDto> result = await controller.UpdateDelivery(orderCreationResponse.OrderId, updatedDelivery);
                deliveryResponse = result.Result;
            }
            Assert.True(deliveryResponse.Success);

            //Update product qty
            List<ProductQuantityDto> productQuantityDtos = new List<ProductQuantityDto>()
            {
                new ProductQuantityDto(){ProductId = productId, Quantity = 5}
            };

            UpdateProductsResponseDto productUpdateresponse;
            using (UpdateOrderProductsConsumer updateOrderProductsConsumer = serviceProvider.GetService<UpdateOrderProductsConsumer>())
            {
                await updateOrderProductsConsumer.StartAsync(CancellationToken.None);
                OperationResultDto<UpdateProductsResponseDto> result = await controller.UpdateProducts(orderCreationResponse.OrderId, productQuantityDtos);
                productUpdateresponse = result.Result;
            }
            Assert.True(productUpdateresponse.Success);

            ///List all orders
            int numberOfPaginatedOrders;
            using (PaginatedOrdersConsumer paginatedOrdersConsumer = serviceProvider.GetService<PaginatedOrdersConsumer>())
            {
                await paginatedOrdersConsumer.StartAsync(CancellationToken.None);
                OperationResultDto<PaginatedOrdersResponseDto> result = await controller.GetPaginated(1);
                numberOfPaginatedOrders = result.Result.Orders.Count;
            }
            Assert.Equal(1, numberOfPaginatedOrders);

            //Get The order
            OrderDto getOrderDto;
            using (GetOrderConsumer getOrderConsumer = serviceProvider.GetService<GetOrderConsumer>())
            {
                await getOrderConsumer.StartAsync(CancellationToken.None);
                OperationResultDto<GetOrderResponseDto> result = await controller.Get(orderCreationResponse.OrderId);
                getOrderDto = result.Result.Order;
            }
            Assert.Single(getOrderDto.Products);
            Assert.Equal(5, getOrderDto.Products.First().Quantity);
            Assert.Equal(updatedDelivery.City, getOrderDto.DeliveryDto.City);
            Assert.Equal(updatedDelivery.Country, getOrderDto.DeliveryDto.Country);
            Assert.Equal(updatedDelivery.Street, getOrderDto.DeliveryDto.Street);


            //Delete the order
            bool deleteOder;
            using (DeleteOrderConsumer deleteOrderConsumer = serviceProvider.GetService<DeleteOrderConsumer>())
            {
                await deleteOrderConsumer.StartAsync(CancellationToken.None);
                OperationResultDto<DeleteOrderResponseDto> result = await controller.Delete(orderCreationResponse.OrderId);
                deleteOder = result.Result.Success;
            }
            Assert.True(deleteOder);



        }

        private ServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(x => BuildRabbitMQSettings())
                .AddSingleton<ISerializer, Serializer>()

                .AddScoped<DbConnection>(x
                => new MySqlConnection("Server=127.0.0.1;Port=4306;Database=retailim;Uid=personaluser;password=personalpass;Allow User Variables=True;"))
                .AddScoped<TransactionalWrapper>()
                .AddScoped<IOrderRepository, OrderRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IOrderProductsRepository, OrderProductsRepository>()
                .AddScoped<IDeliveryRepository, DeliveryRepository>()
                 .AddTransient<ICreateOrderDependencies, CreateOrderServiceDependencies>()
                .AddTransient<IDeleteOrderDependencies, DeleteOrderServiceDependencies>()
                .AddTransient<IRetrievePaginatedOrdersDependencies, RetrievePaginatedOrdersServiceDependencies>()
                .AddTransient<IRetrieveSingleOrderDependencies, RetrieveSingleOrderServiceDependencies>()
                .AddTransient<IUpdateOrderDeliveryDependencies, UpdateOrderDeliveryServiceDependencies>()
                .AddTransient<IUpdateOrderItemsDependencies, UpdateOrderItemsServiceDependencies>()
                .AddTransient<CreateOrder>()
                    .AddTransient<DeleteOrder>()
                    .AddTransient<RetrievePaginatedOrders>()
                    .AddTransient<RetrieveSingleOrder>()
                    .AddTransient<UpdateOrderDelivery>()
                    .AddTransient<UpdateOrderItems>()

                    .AddScoped<OrderController>()
                    .AddScoped<OrderCreationConsumer>()
                    .AddScoped<UpdateOrderDeliveryConsumer>()
                    .AddScoped<UpdateOrderProductsConsumer>()
                    .AddScoped<PaginatedOrdersConsumer>()
                    .AddScoped<GetOrderConsumer>()
                    .AddScoped<DeleteOrderConsumer>()

                .BuildServiceProvider();

            RabbitMQSettings BuildRabbitMQSettings()
            {
                return new RabbitMQSettings("localhost",
                    "admin",
                    "admin");
            }

        }
    }
}
