namespace FishingCommunity.Application.Common.Models;

public class FileStorageSettings
{
    public const string SectionName = "FileStorageSettings";

    public string RootPath { get; set; } = string.Empty; // Physical disk path, e.g. wwwroot/uploads
    public string UrlPathPrefix { get; set; } = "uploads"; // URL segment, e.g. /uploads/posts/xyz.jpg
    public string PublicBaseUrl { get; set; } = string.Empty; // Fallback if no HttpContext available (e.g. background jobs)
}