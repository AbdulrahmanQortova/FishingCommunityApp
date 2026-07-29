using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Shop.Wishlist.Commands.ToggleWishlistItem;
using FishingCommunity.Application.Features.Shop.Wishlist.Queries.GetMyWishlist;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/wishlist")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public WishlistController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyWishlist(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyWishlistQuery { UserId = _currentUserService.UserId!.Value }, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{productId:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid productId, CancellationToken cancellationToken)
    {
        var command = new ToggleWishlistItemCommand { UserId = _currentUserService.UserId!.Value, ProductId = productId };
        var result = await _sender.Send(command, cancellationToken);
        return Ok(result);
    }
}