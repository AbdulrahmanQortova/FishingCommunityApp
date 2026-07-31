using Asp.Versioning;
using FishingCommunity.Application.Features.FishingRecords.FishSpecies.Commands.CreateFishSpecies;
using FishingCommunity.Application.Features.FishingRecords.FishSpecies.Queries.GetAllFishSpecies;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/fish-species")]
public class FishSpeciesController : ControllerBase
{
    private readonly ISender _sender;

    public FishSpeciesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllFishSpeciesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> Create([FromBody] CreateFishSpeciesCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}