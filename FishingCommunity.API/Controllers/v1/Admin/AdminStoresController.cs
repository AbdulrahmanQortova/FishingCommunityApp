using Asp.Versioning;
using FishingCommunity.Application.Features.Admin.Stores.Commands.SuspendStore;
using FishingCommunity.Application.Features.Admin.Stores.Queries.GetAllStores;
using FishingCommunity.Application.Features.Shop.Stores.Commands.ApproveStore;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1.Admin;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/stores")]
[Authorize(Roles = Roles.Administrator)]
public class AdminStoresController : ControllerBase
{
    private readonly ISender _sender;

    public AdminStoresController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetAllStoresQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{storeId:guid}/approve")]
    public async Task<IActionResult> Approve(Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new ApproveStoreCommand { StoreId = storeId }, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{storeId:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid storeId, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new SuspendStoreCommand { StoreId = storeId }, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}