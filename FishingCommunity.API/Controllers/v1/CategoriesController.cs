using Asp.Versioning;
using FishingCommunity.Application.Features.Shop.Categories.Commands.CreateCategory;
using FishingCommunity.Application.Features.Shop.Categories.Queries.GetAllCategories;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllCategoriesQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}