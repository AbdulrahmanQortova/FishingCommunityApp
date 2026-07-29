using Asp.Versioning;
using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Application.Features.Shop.Products.Commands.AddProductReview;
using FishingCommunity.Application.Features.Shop.Products.Commands.CreateProduct;
using FishingCommunity.Application.Features.Shop.Products.Commands.UpdateProduct;
using FishingCommunity.Application.Features.Shop.Products.Commands.UpdateStock;
using FishingCommunity.Application.Features.Shop.Products.Queries.GetProducts;
using FishingCommunity.Application.Features.Shop.Stores.Commands.UpdateStore;
using FishingCommunity.Shared.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FishingCommunity.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;

    public ProductsController(ISender sender, ICurrentUserService currentUserService)
    {
        _sender = sender;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = Roles.StoreOwner)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequestDto request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand
        {
            StoreId = request.StoreId,
            RequestingUserId = _currentUserService.UserId!.Value,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPut("{productId:guid}")]
    [Authorize(Roles = Roles.StoreOwner)]
    public async Task<IActionResult> Update(Guid productId, [FromBody] UpdateProductRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand
        {
            ProductId = productId,
            RequestingUserId = _currentUserService.UserId!.Value,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{productId:guid}/stock")]
    [Authorize(Roles = Roles.StoreOwner)]
    public async Task<IActionResult> UpdateStock(Guid productId, [FromBody] UpdateStockRequestDto request, CancellationToken cancellationToken)
    {
        var command = new UpdateStockCommand
        {
            ProductId = productId,
            RequestingUserId = _currentUserService.UserId!.Value,
            QuantityToAdd = request.QuantityToAdd
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }

    [HttpPost("{productId:guid}/reviews")]
    [Authorize]
    public async Task<IActionResult> AddReview(Guid productId, [FromBody] AddProductReviewRequestDto request, CancellationToken cancellationToken)
    {
        var command = new AddProductReviewCommand
        {
            ProductId = productId,
            UserId = _currentUserService.UserId!.Value,
            Rating = request.Rating,
            Comment = request.Comment
        };

        var result = await _sender.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result) : BadRequest(result);
    }
}