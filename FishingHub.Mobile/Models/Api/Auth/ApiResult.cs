namespace FishingHub.Mobile.Models.Api;

public class ApiResult<T>
{
    public T? Data { get; set; }
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();
    public string? Message { get; set; }
}