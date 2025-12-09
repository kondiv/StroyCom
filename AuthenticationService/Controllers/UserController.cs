using Ardalis.Result.AspNetCore;
using AuthenticationService.Features.CheckExistence;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationService.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UserController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}:exists")]
    public async Task<ActionResult> CheckExistenceAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var request = new CheckExistenceRequest(id);

        var checkExistenceResult = await mediator.Send(request, cancellationToken);

        return checkExistenceResult.ToActionResult(this);
    }
}
