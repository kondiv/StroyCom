using Ardalis.Result;
using MediatR;
using OrderService.Infrastructure;

namespace OrderService.Features.DeleteOrder;

public sealed class DeleteOrderCommandHandler(
    ServiceContext context,
    ILogger<DeleteOrderCommandHandler> logger) : IRequestHandler<DeleteOrderCommand, Result>
{
    public async Task<Result> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Deleting order {id}", request.Id);

        var order = await context
            .Orders
            .FindAsync([request.Id], cancellationToken);

        if (order is null)
        {
            logger.LogError("Order {id} not found", request.Id);
            return Result.NotFound();
        }

        context.Orders.Remove(order);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Order {id} successfully deleted", request.Id);

        return Result.Success();
    }
}
