using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RetailIM.Data.Repository;
using RetailIM.OrderMS.BackgroundWorkers.Consumers;
using RetailIM.Services.Orders;
using RetailIM.ServicesDependencies.Orders;
using Shared.Common.Serialization;
using Shared.MessageBus;
using System.Data.Common;
using WebPersonal.Shared.Data.Db;

namespace RetailIM.OrderMS
{

    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddRabitMQ(configuration)
                .AddDbConnectors(configuration)
                .AddRepositories()
                .AddServiceDependencies()
                .AddServices()
                .AddCommon()
                .AddBackgroundWorkers()
                ;
        }

        private static IServiceCollection AddRabitMQ(this IServiceCollection di, IConfiguration configuration)
        {
            IConfigurationSection rabbitMqConfig = configuration.GetSection("RabbitMq");
            return di
                .AddTransient(x => BuildRabbitMQSettings());

            RabbitMQSettings BuildRabbitMQSettings()
            {
                return new RabbitMQSettings(rabbitMqConfig.GetValue<string>("hostname"),
                    rabbitMqConfig.GetValue<string>("user"),
                    rabbitMqConfig.GetValue<string>("password"));

            }
        }
        private static IServiceCollection AddDbConnectors(this IServiceCollection di, IConfiguration configuration)
        {
            return di
                .AddTransient<DbConnection>(x => new MySqlConnection(configuration.GetValue<string>("connectionStringMysql")))
                .AddTransient<TransactionalWrapper>();
        }

        private static IServiceCollection AddRepositories(this IServiceCollection di)
        {
            return di
                .AddTransient<IOrderRepository, OrderRepository>()
                .AddTransient<IProductRepository, ProductRepository>()
                .AddTransient<IOrderProductsRepository, OrderProductsRepository>()
                .AddTransient<IDeliveryRepository, DeliveryRepository>();
        }

        private static IServiceCollection AddServiceDependencies(this IServiceCollection di)
        {
            return di
                .AddTransient<ICreateOrderDependencies, CreateOrderServiceDependencies>()
                .AddTransient<IDeleteOrderDependencies, DeleteOrderServiceDependencies>()
                .AddTransient<IRetrievePaginatedOrdersDependencies, RetrievePaginatedOrdersServiceDependencies>()
                .AddTransient<IRetrieveSingleOrderDependencies, RetrieveSingleOrderServiceDependencies>()
                .AddTransient<IUpdateOrderDeliveryDependencies, UpdateOrderDeliveryServiceDependencies>()
                .AddTransient<IUpdateOrderItemsDependencies, UpdateOrderItemsServiceDependencies>();
        }

        private static IServiceCollection AddServices(this IServiceCollection di)
        {
            return di
                    .AddTransient<CreateOrder>()
                    .AddTransient<DeleteOrder>()
                    .AddTransient<RetrievePaginatedOrders>()
                    .AddTransient<RetrieveSingleOrder>()
                    .AddTransient<UpdateOrderDelivery>()
                    .AddTransient<UpdateOrderItems>();
        }

        private static IServiceCollection AddCommon(this IServiceCollection di)
        {
            return di
                   .AddTransient<ISerializer, Serializer>();
        }

        private static IServiceCollection AddBackgroundWorkers(this IServiceCollection di)
        {
            return di
                .AddHostedService<OrderCreationConsumer>()
                .AddHostedService<DeleteOrderConsumer>()
                .AddHostedService<PaginatedOrdersConsumer>()
                .AddHostedService<GetOrderConsumer>()
                .AddHostedService<UpdateOrderDeliveryConsumer>()
                .AddHostedService<UpdateOrderProductsConsumer>();
        }
    }
}
