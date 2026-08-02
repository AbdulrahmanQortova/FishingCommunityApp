namespace FishingCommunity.Application.Features.Admin.Users.Queries.GetAllUsers;

public class AdminUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public IList<string> Roles { get; set; } = new List<string>();
    public string Status { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedDate { get; set; }
}