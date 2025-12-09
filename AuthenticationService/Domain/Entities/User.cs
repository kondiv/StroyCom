namespace AuthenticationService.Domain.Entities;

public sealed class User
{
    public required Guid Id { get; init; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required string Name { get; set; }

    public required List<string> Roles { get; set; } = [];

    public DateTime CreateAdUtc { get; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
