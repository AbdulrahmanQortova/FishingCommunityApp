namespace FishingCommunity.Shared.Wrappers;

public class Result
{
    public bool Succeeded { get; protected set; }
    public string[] Errors { get; protected set; } = Array.Empty<string>();
    public string? Message { get; protected set; }

    protected Result(bool succeeded, string? message = null, IEnumerable<string>? errors = null)
    {
        Succeeded = succeeded;
        Message = message;
        Errors = errors?.ToArray() ?? Array.Empty<string>();
    }

    public static Result Success(string? message = null)
        => new(true, message);

    public static Result Failure(string error)
        => new(false, null, new[] { error });

    public static Result Failure(IEnumerable<string> errors)
        => new(false, null, errors);
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    protected Result(bool succeeded, T? data, string? message = null, IEnumerable<string>? errors = null)
        : base(succeeded, message, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data, string? message = null)
        => new(true, data, message);

    public static new Result<T> Failure(string error)
        => new(false, default, null, new[] { error });

    public static new Result<T> Failure(IEnumerable<string> errors)
        => new(false, default, null, errors);
}