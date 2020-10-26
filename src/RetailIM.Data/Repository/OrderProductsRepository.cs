using Dapper;
using RetailIM.Data.Repository.Queries;
using RetailIM.Model.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WebPersonal.Shared.Data.Db;

namespace RetailIM.Data.Repository
{
    public interface IOrderProductsRepository
    {
        Task InsertOrderProducts(Guid orderProductId, List<(Guid, int)> orderProducts);
        Task DeleteProductsFromOrder(Guid orderid);
        Task<ReadOnlyCollection<OrderProductEntity>> GetByOrderId(Guid orderId);
        Task CommitTransaction();
    }

    public class OrderProductsRepository : BaseRepository, IOrderProductsRepository
    {
        protected override string TableName => TableNames.OrderProduct;

        public OrderProductsRepository(TransactionalWrapper conexion) : base(conexion)
        {
        }



        public async Task InsertOrderProducts(Guid orderProductId, List<(Guid, int)> orderProducts)
        {
            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            foreach (var order in orderProducts)
            {
                var sql = $"insert into {TableName} ({nameof(OrderProductEntity.OrderId).ToLower()}, {nameof(OrderProductEntity.ProductId).ToLower()}, {nameof(OrderProductEntity.Quantity).ToLower()}) " +
                    $"VALUES( @{nameof(OrderProductEntity.OrderId)}, @{nameof(OrderProductEntity.ProductId)} ,@{nameof(OrderProductEntity.Quantity)});";
                await connection.ExecuteAsync(sql, new
                {
                    OrderId = orderProductId,
                    ProductId = order.Item1,
                    Quantity = order.Item2
                });
            }
        }

        public async Task DeleteProductsFromOrder(Guid orderid)
        {
            string sql = $"delete from {TableName} Where {nameof(OrderProductEntity.OrderId).ToLower()} = @{nameof(OrderProductEntity.OrderId)};";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            await connection.ExecuteAsync(sql, new
            {
                OrderId = orderid
            });
        }

        public async Task<ReadOnlyCollection<OrderProductEntity>> GetByOrderId(Guid orderId)
        {
            string sql = $"select * from {TableName} Where {nameof(OrderProductEntity.OrderId).ToLower()} = @{nameof(OrderProductEntity.OrderId).ToLower()}";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            var result = await connection.QueryAsync<OrderProductEntity>(sql, new { OrderId = orderId });

            return result.ToList().AsReadOnly();
        }
    }
}