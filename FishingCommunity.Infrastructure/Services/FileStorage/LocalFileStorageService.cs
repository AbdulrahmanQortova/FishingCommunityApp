using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace FishingCommunity.Infrastructure.Services.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly FileStorageSettings _settings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LocalFileStorageService(IOptions<FileStorageSettings> settings, IHttpContextAccessor httpContextAccessor)
    {
        _settings = settings.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default)
    {
        // Security: verify the file's actual content matches an allowed type by checking
        // its magic bytes — Content-Type headers can be spoofed by the client, but the
        // first few bytes of a real image/audio file follow a fixed, well-known signature.
        if (!await FileSignatureValidator.IsAllowedFileTypeAsync(fileStream, cancellationToken))
        {
            throw new InvalidOperationException("File content does not match an allowed image or audio format.");
        }

        // Sanitize the folder name and generate a collision-proof file name —
        // never trust the original file name directly (path traversal risk, e.g. "../../etc").
        var safeFolder = SanitizeFolderName(folder);
        var extension = Path.GetExtension(fileName);
        var safeFileName = $"{Guid.NewGuid()}{extension}";

        var relativePath = Path.Combine(safeFolder, safeFileName);
        var fullPath = Path.Combine(_settings.RootPath, relativePath);

        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        fileStream.Position = 0;

        await using var outputStream = new FileStream(fullPath, System.IO.FileMode.Create);
        await fileStream.CopyToAsync(outputStream, cancellationToken);

        return BuildPublicUrl(safeFolder, safeFileName);
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        var relativePath = ExtractRelativePathFromUrl(fileUrl);

        if (relativePath is null)
        {
            return Task.CompletedTask; // Not a URL we recognize/manage — nothing to delete.
        }

        var fullPath = Path.Combine(_settings.RootPath, relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private static string SanitizeFolderName(string folder)
    {
        // Strip anything that isn't a simple alphanumeric/dash/underscore segment,
        // preventing path traversal via a crafted folder name like "../../wwwroot".
        var cleaned = string.Join("_", folder.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return cleaned.Replace("..", "").Trim('/', '\\');
    }

    private string BuildPublicUrl(string folder, string fileName)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = request is not null
            ? $"{request.Scheme}://{request.Host}"
            : _settings.PublicBaseUrl;

        return $"{baseUrl}/{_settings.UrlPathPrefix}/{folder}/{fileName}".Replace("\\", "/");
    }

    private string? ExtractRelativePathFromUrl(string fileUrl)
    {
        var marker = $"/{_settings.UrlPathPrefix}/";
        var index = fileUrl.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        return index >= 0 ? fileUrl[(index + marker.Length)..] : null;
    }
}