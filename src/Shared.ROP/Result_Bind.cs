using System;
using System.Threading.Tasks;

namespace Shared.ROP
{
    public static class Result_Bind
    {
        /// <summary>
        /// Bind method allows to "link" previous responses with new methods passing them as parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="result"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static async Task<Result<U>> Bind<T, U>(this Task<Result<T>> result, Func<T, Task<Result<U>>> method)
        {
            try
            {
                var r = await result;
                return r.Success
                    ? await method(r.Value)
                    : Result.Failure<U>(r.Errors);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Result<U> Bind<T, U>(this Result<T> r, Func<T, Result<U>> method)
        {
            try
            {
                return r.Success
                    ? method(r.Value)
                    : Result.Failure<U>(r.Errors);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
