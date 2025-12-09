using Ardalis.Result;
using MediatR;

namespace AuthenticationService.Features.SignIn;

public sealed record SignInCommand(string Email, string Name, string Password, string[] Roles)
    : IRequest<Result>;
