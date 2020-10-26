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
    public interface IProductRepository
    {
        Task<ProductEntity?> GetById(Guid productid);
        Task<ReadOnlyCollection<ProductEntity>> GetByIds(List<Guid> productIds);
        Task UpdateProductsAllocation(List<(Guid, int)> productsAllocation);
        Task UpdateProductsUnallocation(List<(Guid, int)> productsAllocation);
        Task InsertSingle(ProductEntity product);
        Task DeleteAllProducts();
        Task CommitTransaction();

    }

    public class ProductRepository : BaseRepository, IProductRepository
    {
        protected override string TableName => TableNames.Product;

        public ProductRepository(TransactionalWrapper conexion) : base(conexion)
        {
        }


        public async Task<ProductEntity?> GetById(Guid productid)
        {
            ReadOnlyCollection<ProductEntity> products = await GetByIds(new List<Guid>() { productid });
            return products.FirstOrDefault();
        }

        public async Task<ReadOnlyCollection<ProductEntity>> GetByIds(List<Guid> productIds)
        {
            string sql = $"select * from {TableName} " +
                $"where  {nameof(ProductEntity.ProductId).ToLower()} in @ids;";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            IEnumerable<ProductEntity> result = await connection.QueryAsync<ProductEntity>(sql, new
            {
                ids = productIds.ToArray()
            });

            return result.ToList().AsReadOnly();

        }

        public async Task UpdateProductsAllocation(List<(Guid, int)> productsAllocation)
        {
            //TODO: this logic needs some work, not sure if in real world scenario this can cause issues.
            //do here CurrentAllocation - new allocation.
            DbConnection connection = await _conexionWrapper.GetConnectionAsync();

            foreach ((Guid, int) product in productsAllocation)
            {
                string sql = $"update {TableName} " +
                    $"set {nameof(ProductEntity.AllocatedStock).ToLower()} = {nameof(ProductEntity.AllocatedStock).ToLower()} + @newqty " +
                    $"where {nameof(ProductEntity.ProductId).ToLower()} = @{nameof(ProductEntity.ProductId).ToLower()};";
                _ = await connection.ExecuteAsync(sql, new
                {
                    newqty = product.Item2,
                    ProductId = product.Item1
                });
            }
        }

        public async Task UpdateProductsUnallocation(List<(Guid, int)> productsAllocation)
        {
            DbConnection connection = await _conexionWrapper.GetConnectionAsync();

            foreach ((Guid, int) product in productsAllocation)
            {
                string sql = $"update {TableName} " +
                    $"set {nameof(ProductEntity.AllocatedStock).ToLower()} = {nameof(ProductEntity.AllocatedStock).ToLower()} - @newqty " +
                    $"where {nameof(ProductEntity.ProductId).ToLower()} = @{nameof(ProductEntity.ProductId).ToLower()};";
                _ = await connection.ExecuteAsync(sql, new
                {
                    newqty = product.Item2,
                    ProductId = product.Item1
                });
            }
        }

        //This method atm is only used on tests
        public async Task InsertSingle(ProductEntity product)
        {
            string sql = $"insert into {TableName} " +
                $" ({nameof(ProductEntity.ProductId).ToLower()}, {nameof(ProductEntity.Name).ToLower()}," +
                $"{nameof(ProductEntity.TotalStock).ToLower()}, {nameof(ProductEntity.AllocatedStock).ToLower()}) " +
                $"VALUES (@{nameof(ProductEntity.ProductId)}, @{nameof(ProductEntity.Name)}," +
                $"@{nameof(ProductEntity.TotalStock)}, @{nameof(ProductEntity.AllocatedStock)}) ;";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            await connection.QueryAsync<ProductEntity>(sql, new
            {
                ProductId = product.ProductId,
                Name = product.Name,
                TotalStock = product.TotalStock,
                AllocatedStock = product.AllocatedStock
            });
        }

        //this is only used on tests.
        public async Task DeleteAllProducts()
        {
            string sql = $"delete from {TableName} ;";

            DbConnection connection = await _conexionWrapper.GetConnectionAsync();
            await connection.ExecuteAsync(sql);
        }
    }
}
