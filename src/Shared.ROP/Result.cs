using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.ROP
{
    /// <summary>
    /// This code is based on my own codebase https://github.com/ElectNewt/EjemploRop
    /// At the same time is based in Railway oriented programming https://fsharpforfunandprofit.com/rop/ 
    /// </summary>
    public static class Result
    {
        public static readonly Unit Unit = Unit.Value;
        private static readonly Task<Result<Unit>> _completedUnitAsync = Task.FromResult(Success());

        public static Result<T> Success<T>(this T value) => new Result<T>(value);

        public static Result<T> Failure<T>(ImmutableArray<string> errors) => new Result<T>(errors);

        public static Result<T> Failure<T>(string error) => new Result<T>(ImmutableArray.Create(error));

        public static Result<Unit> Success() => new Result<Unit>(Unit);

        public static Result<Unit> Failure(ImmutableArray<string> errors) => new Result<Unit>(errors);

        public static Result<Unit> Failure(IEnumerable<string> errors) => new Result<Unit>(ImmutableArray.Create(errors.ToArray()));

        public static Result<Unit> Failure(string error) => new Result<Unit>(ImmutableArray.Create(error));


        public static Task<Result<T>> Async<T>(this Result<T> r) => Task.FromResult(r);

        public static Task<Result<Unit>> Async(this Result<Unit> r) => _completedUnitAsync;
    }
}
