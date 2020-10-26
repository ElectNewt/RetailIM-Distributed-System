using Dapper;
using RetailIM.Data.Repository.Queries;
using RetailIM.Model.Entities;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using WebPersonal.Shared.Data.Db;

namespace RetailIM.Data.Repository
{
    public interface IDeliveryRepository
    {
        Task InsertSingle(DeliveryEntity delivery, Guid orderId);
        Task UpdateOrderAddress(Guid orderId, DeliveryEntity delivery);
        Task DeleteFromOrderId(Guid orderID);
        Task<DeliveryEntity> GetDeliveryFromOrderId(Guid orderId);
        Task CommitTransaction();
    }

    public class DeliveryRepository : BaseRepository, IDeliveryRepository
    {
        protected override string TableName => TableNames.Delivery;

        public DeliveryRepository(TransactionalWrapper conexion) : base(conexion)
        {
        }

        public async Task InsertSingle(DeliveryEntity delivery, Guid orderId)
        {
            string sql = $"insert into {TableName} ({nameof(DeliveryEntity.DeliveryId).ToLower()}, {nameof(DeliveryEntity.City).ToLower()}," +
                $"{nameof(DeliveryEntity.Country).ToLower()}, {nameof(DeliveryEntity.Street).ToLower()})" +
                $" VALUES (@{nameof(DeliveryEntity.DeliveryId)}, @{nameof(DeliveryEntity.City)}, " +
                $"@{nameof(DeliveryEntity.Country)}, @{nameof(DeliveryEntity.Street)});";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            _ = await connection.ExecuteAsync(sql, new
            {
                DeliveryId = orderId,
                City = delivery.City,
                Country = delivery.Country,
                Street = delivery.Street
            });
        }

        public async Task UpdateOrderAddress(Guid orderId, DeliveryEntity delivery)
        {
            string sql = $"Update {TableName} set " +
                $"{nameof(DeliveryEntity.City).ToLower()} = @{nameof(DeliveryEntity.City)}, " +
                $"{nameof(DeliveryEntity.Country).ToLower()} = @{nameof(DeliveryEntity.Country)}, " +
                $"{nameof(DeliveryEntity.Street).ToLower()} = @{nameof(DeliveryEntity.Street)} " +
                $"Where {nameof(DeliveryEntity.DeliveryId).ToLower()} = @{nameof(DeliveryEntity.DeliveryId)};";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            _ = await connection.ExecuteAsync(sql, new
            {
                DeliveryId = orderId,
                City = delivery.City,
                Country = delivery.Country,
                Street = delivery.Street
            });
        }

        public async Task DeleteFromOrderId(Guid orderID)
        {
            string sql = $"delete from {TableName} Where {nameof(DeliveryEntity.DeliveryId).ToLower()} = @{nameof(DeliveryEntity.DeliveryId)};";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            await connection.ExecuteAsync(sql, new
            {
                DeliveryId = orderID
            });
        }

        public async Task<DeliveryEntity> GetDeliveryFromOrderId(Guid orderId)
        {
            string sql = $"select * from {TableName} Where {nameof(DeliveryEntity.DeliveryId).ToLower()} = @{nameof(DeliveryEntity.DeliveryId).ToLower()}";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            return await connection.QueryFirstOrDefaultAsync<DeliveryEntity>(sql, new { DeliveryId = orderId });
        }
    }
}
