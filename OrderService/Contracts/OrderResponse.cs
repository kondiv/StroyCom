using OrderService.Domain.Models;

namespace OrderService.Contracts;

public sealed class OrderResponse
{
    public required Guid Id { get; init; }

    public required Guid UserId { get; init; }
    
    public required Position Position { get; init; }
    
    public required string Status { get; init; }
    
    public required decimal TotalSum { get; init; }
}
