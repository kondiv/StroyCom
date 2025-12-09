using Ardalis.Result;
using Contracts.Events;
using FluentValidation;
using MassTransit;
using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Infrastructure;
using OrderService.Infrastructure.HttpClients.Abstractions;
using System.Data;

namespace OrderService.Features.CreateOrder;

public sealed class CreateOrderCommandHandler(
    ServiceContext context,
    IUserService userService,
    IValidator<CreateOrderCommand> validator,
    IPublishEndpoint publishEndpoint,
    ILogger<CreateOrderCommandHandler> logger) : IRequestHandler<CreateOrderCommand, Result>
{
    public async Task<Result> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Creating new order");

        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            logger.LogError("Validation errors occurred.\n{errors}",
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

        var userExists = await userService.UserExists(request.UserId, cancellationToken);

        if (!userExists.IsSuccess)
        {
            logger.LogError("User does not exist");
            return Result.Forbidden("User does not exist");
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Position = request.Position,
            Status = Status.Created,
            TotalSum = request.TotalSum
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new OrderCreatedEvent(
                order.Id,
                order.UserId,
                order.Position.Item,
                order.Position.Quantity,
                order.TotalSum,
                order.CreatedAtUtc),
            cancellationToken);

        logger.LogInformation("Order successfully created");

        return Result.Success();
    }
}
