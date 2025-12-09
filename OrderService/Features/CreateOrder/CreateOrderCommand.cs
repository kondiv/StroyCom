using Ardalis.Result;
using MediatR;
using OrderService.Domain.Models;

namespace OrderService.Features.CreateOrder;

public sealed record CreateOrderCommand(Guid UserId, Position Position, decimal TotalSum)
    : IRequest<Result>;
