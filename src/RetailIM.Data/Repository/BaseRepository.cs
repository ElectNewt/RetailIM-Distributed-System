using System.Threading.Tasks;
using WebPersonal.Shared.Data.Db;

namespace RetailIM.Data.Repository
{
    public abstract class BaseRepository
    {
        protected abstract string TableName { get; }

        protected readonly TransactionalWrapper _conexionWrapper;

        public BaseRepository(TransactionalWrapper conexion)
        {
            _conexionWrapper = conexion;
        }

        public async Task CommitTransaction()
        {
            await _conexionWrapper.CommitTransactionAsync();
        }
    }
}
