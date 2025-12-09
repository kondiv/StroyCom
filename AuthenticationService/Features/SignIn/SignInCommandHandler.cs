using Ardalis.Result;
using AuthenticationService.Abstractions;
using AuthenticationService.Domain.Entities;
using AuthenticationService.Infrastructure;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AuthenticationService.Features.SignIn;

public sealed class SignInCommandHandler(
    ServiceContext context,
    IValidator<SignInCommand> validator,
    IPasswordHasher passwordHasher,
    ILogger<SignInCommandHandler> logger) : IRequestHandler<SignInCommand, Result>
{
    public async Task<Result> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        using var _ = logger.BeginScope("Registering new user {email}", request.Email);

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
                    ErrorCode = e.ErrorCode,
                    ErrorMessage = e.ErrorMessage,
                    Identifier = e.PropertyName
                })
            );
        }

        var uniqueEmail = !await context
            .Users
            .AnyAsync(u => u.Email == request.Email, cancellationToken);

        if (!uniqueEmail)
        {
            logger.LogError("Email already taken. Email: {email}", request.Email);
            return Result.Conflict("Email already taken");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Name = request.Name,
            PasswordHash = passwordHasher.Hash(request.Password),
            Roles = request.Roles.ToList()
        };

        try
        {
            context.Users.Add(user);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("User signed in successfully");
            return Result.Success();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is NpgsqlException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            logger.LogError("Email already taken. Email {e}", request.Email);
            return Result.Conflict("Email already taken");
        }
    }
}
