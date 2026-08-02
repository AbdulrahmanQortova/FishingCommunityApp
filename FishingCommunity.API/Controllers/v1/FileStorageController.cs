using Asp.Versioning;
using FishingCommunity.API.Models;
using FishingCommunity.Application.Features.FileStorage.Commands.UploadFile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/files")]
[Authorize]
public class FileStorageController : ControllerBase
{
    private readonly ISender _sender;

    public FileStorageController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] UploadFileRequestDto request, CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest(new { message = "No file was provided." });
        }

        await using var stream = request.File.OpenReadStream();

        var command = new UploadFileCommand
        {
            FileStream = stream,
            FileName = request.File.FileName,
            Folder = request.Folder,
            FileSizeBytes = request.File.Length,
            ContentType = request.File.ContentType
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}