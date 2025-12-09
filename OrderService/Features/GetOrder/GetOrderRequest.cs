using Ardalis.Result;
using MediatR;
using OrderService.Contracts;

namespace OrderService.Features.GetOrder;

public sealed record GetOrderRequest(Guid Id) : IRequest<Result<OrderResponse>>;
