using Ardalis.Result;
using MediatR;

namespace OrderService.Features.DeleteOrder;

public sealed record DeleteOrderCommand(Guid Id) : IRequest<Result>;
