using System;
using System.Collections.Immutable;


namespace Shared.ROP
{
    public struct Result<T>
    {
        public readonly T Value;

        public static implicit operator Result<T>(T value) => new Result<T>(value);

        public static implicit operator Result<T>(ImmutableArray<string> errors) => new Result<T>(errors);

        public readonly ImmutableArray<string> Errors;
        public bool Success => Errors.Length == 0;

        public Result(T value)
        {
            Value = value;
            Errors = ImmutableArray<string>.Empty;
        }

        public Result(ImmutableArray<string> errors)
        {
            if (errors.Length == 0)
            {
                throw new InvalidOperationException("You should indicate at least one error");
            }

            Value = default!;
            Errors = errors;
        }
    }
}