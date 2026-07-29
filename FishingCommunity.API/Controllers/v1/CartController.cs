using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Shop.Cart.Commands.AddToCart;
using FishingCommunity.Application.Features.Shop.Cart.Commands.RemoveCartItem;
using FishingCommunity.Application.Features.Shop.Cart.Commands.UpdateCartItem;
using FishingCommunity.Application.Features.Shop.Cart.Queries.GetMyCart;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public CartController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyCart(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyCartQuery { UserId = _currentUserService.UserId!.Value }, cancellationToken);
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToCartRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AddToCartCommand
        {
            UserId = _currentUserService.UserId!.Value,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("items/{productId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid productId, [FromBody] UpdateCartItemRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateCartItemCommand
        {
            UserId = _currentUserService.UserId!.Value,
            ProductId = productId,
            Quantity = request.Quantity
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid productId, CancellationToken cancellationToken)
    {
        var command = new RemoveCartItemCommand
        {
            UserId = _currentUserService.UserId!.Value,
            ProductId = productId
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}