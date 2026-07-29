using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Shop.ShippingAddresses.Commands.AddShippingAddress;
using FishingCommunity.Application.Features.Shop.ShippingAddresses.Queries.GetMyAddresses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/shipping-addresses")]
[Authorize]
public class ShippingAddressesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public ShippingAddressesController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAddresses(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetMyAddressesQuery { UserId = _currentUserService.UserId!.Value }, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddShippingAddressRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AddShippingAddressCommand
        {
            UserId = _currentUserService.UserId!.Value,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            Country = request.Country,
            PostalCode = request.PostalCode,
            SetAsDefault = request.SetAsDefault
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}