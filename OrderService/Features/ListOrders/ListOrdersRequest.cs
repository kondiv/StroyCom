using Ardalis.Result;
using MediatR;
using OrderService.Contracts;

namespace OrderService.Features.ListOrders;

public sealed record ListOrdersRequest(Guid UserId, int Page, int MaxPageSize) 
    : IRequest<Result<IReadOnlyCollection<OrderResponse>>>;
