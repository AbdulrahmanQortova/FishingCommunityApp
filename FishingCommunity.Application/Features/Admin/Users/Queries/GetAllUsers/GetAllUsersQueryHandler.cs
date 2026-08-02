using FishingCommunity.Application.Common.Interfaces;
using FishingCommunity.Shared.Wrappers;
using MediatR;

namespace FishingCommunity.Application.Features.Admin.Users.Queries.GetAllUsers;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PaginatedList<AdminUserDto>>>
{
    private readonly IIdentityService _identityService;

    public GetAllUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<PaginatedList<AdminUserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _identityService.GetUsersAsync(
            request.PageNumber, request.PageSize, request.SearchTerm, request.Role, cancellationToken);

        var dtos = users.Select(u => new AdminUserDto
        {
            Id = u.Id,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Roles = u.Roles,
            Status = u.Status,
            IsEmailVerified = u.IsEmailVerified,
            CreatedDate = u.CreatedDate
        }).ToList();

        var paginatedResult = new PaginatedList<AdminUserDto>(dtos, totalCount, request.PageNumber, request.PageSize);

        return Result<PaginatedList<AdminUserDto>>.Success(paginatedResult);
    }
}