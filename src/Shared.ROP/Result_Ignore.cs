using System;
using System.Threading.Tasks;

namespace Shared.ROP
{
    public static class Result_Ignore
    {
        public static Result<Unit> Ignore<T>(this Result<T> r)
        {
            try
            {
                return r.Success
                    ? Result.Success()
                    : Result.Failure(r.Errors);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<Result<Unit>> Ignore<T>(this Task<Result<T>> r)
        {
            try
            {
                return (await r).Ignore();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
