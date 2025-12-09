using Ardalis.Result;
using Contracts.Events;
using FluentValidation;
using MassTransit;
using MassTransit.Caching.Internals;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Enums;
using OrderService.Infrastructure;
using System.Data;

namespace OrderService.Features.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandHandler(
    ServiceContext context,
    IPublishEndpoint publishEndpoint,
    IValidator<UpdateOrderStatusCommand> validator,
    ILogger<UpdateOrderStatusCommandHandler> logger) : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Updating order {id} status", request.Id);

        var order = await context
            .Orders
            .FindAsync([request.Id], cancellationToken);

        if (order is null)
        {
            logger.LogError("Order {id} not found", request.Id);
            return Result.NotFound("Order not found");
        }

        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            logger.LogError("Validation errors occurred\n{errors}",
                validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            return Result.Invalid(
                validationResult
                .Errors
                .Select(e => new ValidationError
                {
                    Identifier = e.PropertyName,
                    ErrorMessage = e.ErrorMessage,
                    ErrorCode = e.ErrorCode
                })
                .ToArray()
            );
        }

        if (!Enum.TryParse<Status>(request.NewStatus, ignoreCase: true, out var newStatus))
        {
            logger.LogError("Invalid status provided. Can not parse to enum");
            return Result.Invalid(new ValidationError("Invalid status"));
        }

        if (!IsValidStatusChange(order.Status, newStatus))
        {
            logger.LogError("Invalid status provided. Can not change from {1} to {2}",
                order.Status.ToString(), newStatus.ToString());
            return Result.Invalid(new ValidationError($"Can not change from {order.Status} to {newStatus}"));
        }

        try
        {
            var oldStatus = order.Status.ToString();
            order.Status = newStatus;
            order.UpdatedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new OrderStatusChangedEvent(
                order.Id,
                oldStatus,
                order.Status.ToString(),
                order.UpdatedAtUtc
                ),
                cancellationToken);

            return Result.Success();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.LogError(ex, "Order status has already been changed");
            return Result.Conflict("Order status has already been changed. Refresh your data");
        }
    }

    private bool IsValidStatusChange(Status oldStatus, Status newStatus)
    {
        return (oldStatus, newStatus) switch
        {
            (Status.Created, Status.InWork)   => true,
            (Status.InWork, Status.Completed) => true,
            (Status.Created, Status.Canceled) => true,
            (Status.InWork, Status.Canceled)  => true,
            _ => false
        };
    }
}
