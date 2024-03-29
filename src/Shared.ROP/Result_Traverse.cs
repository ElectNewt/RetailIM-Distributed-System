﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Shared.ROP
{
    public static class Result_Traverse
    {

        /// <summary>
        /// Convierte List<Result<T>> a Result<List<T>>
        /// </summary>
        public static Result<List<T>> Traverse<T>(this IEnumerable<Result<T>> results)
        {
            try
            {
                List<string> errors = new List<string>();
                List<T> output = new List<T>();

                foreach (var r in results)
                {
                    if (r.Success)
                    {
                        output.Add(r.Value);
                    }
                    else
                    {
                        errors.AddRange(r.Errors);
                    }
                }

                return errors.Count > 0
                    ? Result.Failure<List<T>>(errors.ToImmutableArray())
                    : Result.Success(output);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Convierte IEnumerable<Task<Result<T>>> a Task<Result<List<T>>>
        /// </summary>
        public static async Task<Result<List<T>>> Traverse<T>(this IEnumerable<Task<Result<T>>> results)
        {
            try
            {
                List<Result<T>> res = new List<Result<T>>();
                foreach (var task in results)
                {
                    res.Add(await task);
                }

                return res.Traverse();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Convierte IEnumerable<Result<T>> a Result<T>>
        /// </summary>
        public static Result<Unit> TraverseUnit(this IEnumerable<Result<Unit>> results)
        {
            try
            {
                return results.Traverse().Map(MapToSingle);
            }
            catch (Exception)
            {
                throw;
            }

            Unit MapToSingle(List<Unit> _) => Unit.Value;
        }
    }
}
