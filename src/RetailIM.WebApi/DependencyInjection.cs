using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RetailIM.WebApi.Dispatcher.Orders;
using Shared.Common.Serialization;
using Shared.MessageBus;

namespace RetailIM.WebApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddRabitMQ(configuration)
                .AddCommon();
            //.BuildQueues();
        }

        private static IServiceCollection AddRabitMQ(this IServiceCollection di, IConfiguration configuration)
        {
            IConfigurationSection rabbitMqConfig = configuration.GetSection("RabbitMq");
            return di
                .AddSingleton(x => BuildRabbitMQSettings());

            RabbitMQSettings BuildRabbitMQSettings()
            {
                return new RabbitMQSettings(rabbitMqConfig.GetValue<string>("hostname"),
                    rabbitMqConfig.GetValue<string>("user"),
                    rabbitMqConfig.GetValue<string>("password"));

            }
        }
        private static IServiceCollection AddCommon(this IServiceCollection di)
        {
            return di
                   .AddSingleton<ISerializer, Serializer>();
        }

        private static IServiceCollection BuildQueues(this IServiceCollection di)
        {
            return di
                .AddTransient<OrderCreationProducerQueue>()
                .AddTransient<DeleteOrderProducerQueue>()
                .AddTransient<PaginatedOrderProducerQueue>()
                .AddTransient<GetOrderProducerQueue>()
                .AddTransient<UpdateOrderDeliveryProducerQueue>()
                .AddTransient<UpdateOrderProductsProducerQueue>()
                .AddTransient<OrderQueues>();
        }
    }
}
