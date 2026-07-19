namespace CareLink.Application.Common
{
    public class Result
    {
        public bool Succeeded { get; }
        public string? Error { get; }
        public IReadOnlyList<string> Errors { get; }

        protected Result(bool succeeded, string? error, IReadOnlyList<string>? errors = null)
        {
            Succeeded = succeeded;
            Error = error;
            Errors = errors ?? (error is null ? Array.Empty<string>() : new[] { error });
        }

        public static Result Success() => new(true, null);
        public static Result Failure(string error) => new(false, error);
        public static Result Failure(IReadOnlyList<string> errors) => new(false, errors.FirstOrDefault(), errors);
    }

    public class Result<T> : Result
    {
        public T? Data { get; }

        private Result(bool succeeded, T? data, string? error, IReadOnlyList<string>? errors = null)
            : base(succeeded, error, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data) => new(true, data, null);
        public static new Result<T> Failure(string error) => new(false, default, error);
        public static new Result<T> Failure(IReadOnlyList<string> errors) => new(false, default, errors.FirstOrDefault(), errors);
    }
}