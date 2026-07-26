using System.Security.Cryptography;

namespace FishingCommunity.Shared.Utilities;

public static class RandomCodeGenerator
{
    private const string Digits = "0123456789";
    private const string Alphanumeric = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    public static string GenerateNumericCode(int length = 6)
        => GenerateFromCharset(Digits, length);

    public static string GenerateAlphanumericCode(int length = 8)
        => GenerateFromCharset(Alphanumeric, length);

    public static string GenerateSecureToken(int byteLength = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteLength);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    private static string GenerateFromCharset(string charset, int length)
    {
        var result = new char[length];
        for (var i = 0; i < length; i++)
        {
            var index = RandomNumberGenerator.GetInt32(charset.Length);
            result[i] = charset[index];
        }
        return new string(result);
    }
}