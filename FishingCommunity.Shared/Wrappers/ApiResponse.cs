namespace FishingCommunity.Shared.Wrappers;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }

    public static ApiResponse<T> Success(T data, string? message = null)
        => new() { Succeeded = true, Data = data, Message = message };

    public static ApiResponse<T> Failure(string[] errors, string? message = null)
        => new() { Succeeded = false, Errors = errors, Message = message };
}