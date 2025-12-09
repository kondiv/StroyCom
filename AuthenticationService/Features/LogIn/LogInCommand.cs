using Ardalis.Result;
using MediatR;

namespace AuthenticationService.Features.LogIn;

public sealed record LogInCommand(string Email, string Password) : IRequest<Result<string>>;
