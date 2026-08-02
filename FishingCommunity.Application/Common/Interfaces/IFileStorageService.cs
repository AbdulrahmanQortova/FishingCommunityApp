namespace FishingCommunity.Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Saves a file and returns a publicly accessible URL to it.
    /// </summary>
    /// <param name="fileStream">The file content stream.</param>
    /// <param name="fileName">Original file name (used to derive the extension).</param>
    /// <param name="folder">Logical folder/category — e.g. "posts", "products", "payment-proofs".</param>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder, CancellationToken cancellationToken = default);

    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
}