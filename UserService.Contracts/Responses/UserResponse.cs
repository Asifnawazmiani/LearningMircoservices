namespace UserService.Contracts.Responses;

/// <summary>
/// Response containing user information.
/// </summary>
public record UserResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string MobileNo { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsEmailVerified { get; init; }
    public bool IsPhoneVerified { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public List<string> Roles { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
