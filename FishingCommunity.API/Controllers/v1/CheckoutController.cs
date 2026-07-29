using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Shop.Checkout.Commands.Checkout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/checkout")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public CheckoutController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CheckoutCommand
        {
            UserId = _currentUserService.UserId!.Value,
            ShippingAddressId = request.ShippingAddressId,
            CouponCode = request.CouponCode
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}