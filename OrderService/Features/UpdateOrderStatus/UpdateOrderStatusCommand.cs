using Ardalis.Result;
using MediatR;

namespace OrderService.Features.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(Guid Id, string NewStatus) : IRequest<Result>;
