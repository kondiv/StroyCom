using OrderService.Domain.Enums;
using OrderService.Domain.Models;

namespace OrderService.Domain.Entities;

public sealed class Order
{
    public required Guid Id { get; init; }

    public required Guid UserId { get; set; }

    public required Position Position { get; set; }

    public required Status Status { get; set; }

    public required decimal TotalSum { get; set; }

    public DateTime CreatedAtUtc { get; } = DateTime.UtcNow;

    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
