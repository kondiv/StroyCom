using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Contracts;
using OrderService.Infrastructure;

namespace OrderService.Features.ListOrders;

public sealed class ListOrdersRequestHandler(
    ServiceContext context,
    IValidator<ListOrdersRequest> validator,
    ILogger<ListOrdersRequestHandler> logger) : IRequestHandler<ListOrdersRequest, Result<IReadOnlyCollection<OrderResponse>>>
{
    public async Task<Result<IReadOnlyCollection<OrderResponse>>> Handle(ListOrdersRequest request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Listing orders for user {uId}", request.UserId);

        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            logger.LogError("Validation errors occurred\n{errors}",
                validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));

            return Result<IReadOnlyCollection<OrderResponse>>.Invalid(
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

        var orders = await context
            .Orders
            .Where(o => o.UserId == request.UserId)
            .Select(o => new OrderResponse
            {
                Id = o.Id,
                Position = o.Position,
                Status = o.Status.ToString(),
                TotalSum = o.TotalSum,
                UserId = o.UserId,
            })
            .OrderBy(o => o.Id)
            .Skip((request.Page - 1) * request.MaxPageSize)
            .Take(request.MaxPageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        logger.LogInformation("Listing complete");

        return Result<IReadOnlyCollection<OrderResponse>>.Success(orders);
    }
}
