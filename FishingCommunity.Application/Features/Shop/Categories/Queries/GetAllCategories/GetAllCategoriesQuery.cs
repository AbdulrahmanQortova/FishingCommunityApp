using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Shop.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<Result<List<CategoryDto>>>
{
}