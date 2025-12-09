using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts;
using OrderService.Infrastructure;

namespace OrderService.Features.GetOrder;

public sealed class GetOrderRequestHandler(
    ServiceContext context,
    ILogger<GetOrderRequestHandler> logger) : IRequestHandler<GetOrderRequest, Result<OrderResponse>>
{
    public async Task<Result<OrderResponse>> Handle(GetOrderRequest request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Requesting order {id}", request.Id);

        var order = await context
            .Orders
            .Select(o => new OrderResponse
            {
                Id = o.Id,
                Position = o.Position,
                Status = o.Status.ToString(),
                TotalSum = o.TotalSum,
                UserId = o.UserId
            })
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order is null)
        {
            logger.LogError("Order {id} not found", request.Id);
            return Result<OrderResponse>.NotFound();
        }

        return Result<OrderResponse>.Success(order);
    }
}
