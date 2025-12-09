using Ardalis.Result.AspNetCore;
using AuthenticationService.Contracts;
using AuthenticationService.Features.LogIn;
using AuthenticationService.Features.SignIn;
using AuthenticationService.Infrastructure.Security;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthenticationService.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IMediator mediator, IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    [HttpPost("sign-in")]
    public async Task<ActionResult> SignInAsync(SignInRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new SignInCommand(request.Email, request.Name, request.Password, request.Roles);

        var signInResult = await mediator.Send(command, cancellationToken);

        return signInResult.ToActionResult(this);
    }

    [HttpPost("log-in")]
    public async Task<ActionResult<string>> LogInAsync(LogInRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new LogInCommand(request.Email, request.Password);

        var logInResult = await mediator.Send(command, cancellationToken);
        
        if (logInResult.IsSuccess)
        {
            HttpContext.Response.Cookies.Append("access_token", logInResult.Value, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenLifetimeMinutes)
            });
        }

        return logInResult.ToActionResult(this);
    }
}
