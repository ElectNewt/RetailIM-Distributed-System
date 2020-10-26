using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RetailIM.Data.Repository;
using System.Data.Common;
using WebPersonal.Shared.Data.Db;

namespace RetailIM.IntegrationTest.Data.Repositories
{
    public class TestBaseDependencies
    {

        protected IServiceCollection BuildDependencies()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddScoped<DbConnection>(x
                => new MySqlConnection("Server=127.0.0.1;Port=4306;Database=retailim;Uid=personaluser;password=personalpass;Allow User Variables=True;"))
                .AddScoped<TransactionalWrapper>()
                .AddScoped<IOrderRepository, OrderRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IOrderProductsRepository, OrderProductsRepository>()
                .AddScoped<IDeliveryRepository, DeliveryRepository>();

            return services;
        }

    }
}
