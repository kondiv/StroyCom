namespace Contracts.Events;

public sealed record OrderCreatedEvent(
    Guid OrderId, 
    Guid UserId, 
    string Item,
    int Quantity, 
    decimal TotalSum, 
    DateTime CreateAtUtc);
