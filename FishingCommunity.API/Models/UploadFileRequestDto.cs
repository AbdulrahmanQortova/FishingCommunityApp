using Microsoft.AspNetCore.Http;

namespace FishingCommunity.API.Models;

public class UploadFileRequestDto
{
    public IFormFile? File { get; set; }
    public string Folder { get; set; } = "general";
}