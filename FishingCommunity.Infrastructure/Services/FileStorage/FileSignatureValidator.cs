namespace FishingCommunity.Infrastructure.Services.FileStorage;

/// <summary>
/// Validates file content against known "magic byte" signatures, so a spoofed
/// Content-Type header (or a renamed .exe) can't slip through as an allowed image/audio file.
/// </summary>
public static class FileSignatureValidator
{
    private static readonly Dictionary<string, List<byte[]>> Signatures = new()
    {
        ["jpeg"] = new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } },
        ["png"] = new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
        ["webp"] = new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 } }, // "RIFF" header (WebP container)
        ["mp3"] = new List<byte[]>
        {
            new byte[] { 0x49, 0x44, 0x33 },       // "ID3" tag
            new byte[] { 0xFF, 0xFB },              // MPEG frame sync (no ID3 tag)
        },
        ["ogg"] = new List<byte[]> { new byte[] { 0x4F, 0x67, 0x67, 0x53 } }, // "OggS"
    };

    public static async Task<bool> IsAllowedFileTypeAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        stream.Position = 0;

        var buffer = new byte[16]; // Enough bytes to cover all signatures above.
        var bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        stream.Position = 0; // Reset for the actual save operation afterward.

        if (bytesRead == 0) return false;

        foreach (var signatureList in Signatures.Values)
        {
            foreach (var signature in signatureList)
            {
                if (bytesRead >= signature.Length && buffer.Take(signature.Length).SequenceEqual(signature))
                {
                    return true;
                }
            }
        }

        return false;
    }
}