using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Shop.Stores.Commands.ApproveStore;
using FishingCommunity.Application.Features.Shop.Stores.Commands.CreateStore;
using FishingCommunity.Application.Features.Shop.Stores.Commands.UpdateStore;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/stores")]
public class StoresController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public StoresController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Roles = Roles.StoreOwner)]
    public async Task<IActionResult> Create([FromBody] CreateStoreRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateStoreCommand
        {
            OwnerId = _currentUserService.UserId!.Value,
            Name = request.Name,
            Description = request.Description
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{storeId:guid}")]
    [Authorize(Roles = Roles.StoreOwner)]
    public async Task<IActionResult> Update(Guid storeId, [FromBody] UpdateStoreRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateStoreCommand
        {
            StoreId = storeId,
            RequestingUserId = _currentUserService.UserId!.Value,
            Name = request.Name,
            Description = request.Description
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{storeId:guid}/approve")]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> Approve(Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ApproveStoreCommand { StoreId = storeId }, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}