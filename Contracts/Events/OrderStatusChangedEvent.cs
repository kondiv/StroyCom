namespace Contracts.Events;

public sealed record OrderStatusChangedEvent(Guid OrderId, string OldStatus, string NewStatus, DateTime UpdatedAtUtc);
