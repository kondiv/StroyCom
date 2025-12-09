using Ardalis.Result;
using AuthenticationService.Abstractions;
using AuthenticationService.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService.Features.LogIn;

public sealed class LogInCommandHandler(
    ServiceContext context,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    ILogger<LogInCommandHandler> logger) : IRequestHandler<LogInCommand, Result<string>>
{
    public async Task<Result<string>> Handle(LogInCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Log in request for user {email}", request.Email);

        var user = await context
            .Users
            .Select(u => new
            {
                u.Id,
                u.Email,
                u.PasswordHash,
                u.Roles
            })
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            logger.LogError("User not found");
            return Result.Unauthorized("Invalid credentials");
        }

        bool validPassword = passwordHasher.Verify(request.Password, user.PasswordHash);

        if (!validPassword)
        {
            logger.LogError("Invalid password");
            return Result.Unauthorized("Invalid credentials");
        }

        var token = tokenProvider.GetAccessToken(user.Id, user.Email, user.Roles);

        logger.LogInformation("User logged in successfully");

        return Result<string>.Success(token);
    }
}
