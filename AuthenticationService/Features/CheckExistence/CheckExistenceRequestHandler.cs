using Ardalis.Result;
using AuthenticationService.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Features.CheckExistence;

public sealed class CheckExistenceRequestHandler(
    ServiceContext context,
    ILogger<CheckExistenceRequestHandler> logger) : IRequestHandler<CheckExistenceRequest, Result>
{
    public async Task<Result> Handle(CheckExistenceRequest request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Check existence for user {id}", request.UserId);

        bool exist = await context
            .Users
            .AnyAsync(u => u.Id == request.UserId, cancellationToken);

        if (!exist)
        {
            logger.LogInformation("User not found");
            return Result.NotFound();
        }

        logger.LogInformation("User found");
        return Result.Success();
    }
}
