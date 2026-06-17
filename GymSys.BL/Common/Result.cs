using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Common
{
    public sealed record Result(bool success, string? error = null, ResultKind kind = ResultKind.OK)
    {
        public static Result OK() => new(true);
        public static Result Fail(string error, ResultKind kind = ResultKind.CONFLICT) => new(false, error, kind);
        public static Result NotFound(string error = "Not Found!") => new(false, error, ResultKind.NOT_FOUND);
        public static Result Validation(string error) => new(false, error, ResultKind.VALIDATION_FAILED);
    }

    public sealed record Result<T>(bool success, T? value, string? error = null, ResultKind king = ResultKind.OK)
    {
        public static Result<T> OK(T value) => new(true, value);
        public static Result<T> Fail(string error, ResultKind king = ResultKind.CONFLICT) => new(false, default, error, king);
        public static Result<T> NotFound(string error = "Not Found!") => new(false, default, error, ResultKind.NOT_FOUND);
        public static Result<T> Validation(string error) => new(false, default, error, ResultKind.VALIDATION_FAILED);
    }
}
