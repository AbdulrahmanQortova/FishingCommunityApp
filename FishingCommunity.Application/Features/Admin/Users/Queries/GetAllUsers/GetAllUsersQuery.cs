using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<Result<PaginatedList<AdminUserDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; } // Searches name/email
    public string? Role { get; set; } // Optional filter
}