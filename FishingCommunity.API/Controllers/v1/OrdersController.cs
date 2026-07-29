using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Shop.Orders.Commands.CancelOrder;
using FishingCommunity.Application.Features.Shop.Orders.Queries.GetMyOrders;
using FishingCommunity.Application.Features.Shop.Orders.Queries.GetOrderDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public OrdersController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetMyOrdersQuery { UserId = _currentUserService.UserId!.Value, PageNumber = pageNumber, PageSize = pageSize };
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetDetails(Guid orderId, CancellationToken cancellationToken)
    {
        var query = new GetOrderDetailsQuery { OrderId = orderId, RequestingUserId = _currentUserService.UserId!.Value };
        var result = await _sender.Send(query, cancellationToken);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid orderId, [FromBody] CancelOrderRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CancelOrderCommand
        {
            OrderId = orderId,
            RequestingUserId = _currentUserService.UserId!.Value,
            Reason = request.Reason
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}