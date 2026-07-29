using FishingCommunity.Domain.Entities.Shop;
using FishingCommunity.Domain.Exceptions;
using FishingCommunity.Domain.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.Application.Features.Shop.Products.Commands.AddProductReview;

public class AddProductReviewCommandHandler : IRequestHandler<AddProductReviewCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddProductReviewCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddProductReviewCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().Query()
            .Where(p => p.Id == request.ProductId)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        // Note: unlike TripReview, we're not requiring a verified purchase here for
        // simplicity — could add that constraint later by checking OrderItems.
        var review = product.AddReview(request.UserId, request.Rating, request.Comment);

        _unitOfWork.Repository<Product>().Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(review.Id, "Review added successfully.");
    }
}