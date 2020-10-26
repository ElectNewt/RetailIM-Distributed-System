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
    public interface IOrderRepository
    {
        Task<Guid> InsertSingle();
        Task<OrderEntity?> GetSingle(Guid orderId);
        Task DeleteFromId(Guid orderId);
        Task<ReadOnlyCollection<OrderEntity>> GetPaginated(int page, int itemsPerPage);
        Task CommitTransaction();
    }

    public class OrderRepository : BaseRepository, IOrderRepository
    {
        protected override string TableName => TableNames.Order;

        public OrderRepository(TransactionalWrapper conexion) : base(conexion)
        {
        }


        public async Task<Guid> InsertSingle()
        {
            Guid OrderId = Guid.NewGuid();


            string sql = $"insert into {TableName} ({nameof(OrderEntity.OrderId).ToLower()}, {nameof(OrderEntity.CreationTimeUtc).ToLower()})" +
                $" VALUES (@{nameof(OrderEntity.OrderId)}, @{nameof(OrderEntity.CreationTimeUtc)});";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            _ = await connection.ExecuteAsync(sql, new
            {
                OrderId,
                CreationTimeUtc = DateTime.UtcNow
            });

            return OrderId;
        }

        public async Task<OrderEntity?> GetSingle(Guid orderId)
        {
            string sql = $"select * from {TableName} Where {nameof(OrderEntity.OrderId).ToLower()} = @{nameof(OrderEntity.OrderId).ToLower()};";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            OrderEntity? result = await connection.QueryFirstOrDefaultAsync<OrderEntity>(sql, new { OrderId = orderId });

            return result;
        }

        public async Task DeleteFromId(Guid orderId)
        {
            string sql = $"delete from {TableName} Where {nameof(OrderEntity.OrderId).ToLower()} = @{nameof(OrderEntity.OrderId).ToLower()};";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            await connection.ExecuteAsync(sql, new { OrderId = orderId });
        }

        public async Task<ReadOnlyCollection<OrderEntity>> GetPaginated(int page, int itemsPerPage)
        {
            string sql = $"select * from {TableName} " +
                $"order by {nameof(OrderEntity.CreationTimeUtc).ToLower()} desc " +
                $"LIMIT @limit OFFSET @offset; ";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            IEnumerable<OrderEntity> result = await connection.QueryAsync<OrderEntity>(sql, new
            {
                limit = itemsPerPage,
                offset = (page - 1) * itemsPerPage,
            });

            return result.ToList().AsReadOnly();
        }
    }
}
