using Ardalis.Result.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Contracts;
using OrderService.Features.CreateOrder;
using OrderService.Features.DeleteOrder;
using OrderService.Features.GetOrder;
using OrderService.Features.ListOrders;
using OrderService.Features.UpdateOrderStatus;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("api/v1/orders/")]
public class OrderController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> CreateAsync(CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if(!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var command = new CreateOrderCommand(userId, request.Position, request.TotalSum);

        var createResult = await mediator.Send(command, cancellationToken);

        return createResult.ToActionResult(this);
    }

    [Authorize(Policy = "OrderOwnerOrAdmin")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var request = new GetOrderRequest(id);

        var getResult = await mediator.Send(request, cancellationToken);

        return getResult.ToActionResult(this);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrderResponse>>> ListAsync(
        [FromQuery] int page = 1,
        [FromQuery] int maxPageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var request = new ListOrdersRequest(userId, page, maxPageSize);

        var listResult = await mediator.Send(request, cancellationToken);

        return listResult.ToActionResult(this);
    }

    [Authorize(Roles = "admin")]
    [HttpPost("{id:guid}/status:change")]
    public async Task<ActionResult> ChangeStatusAsync([FromRoute] Guid id,
        [FromQuery] string newStatus,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateOrderStatusCommand(id, newStatus);

        var updateResult = await mediator.Send(command, cancellationToken);

        return updateResult.ToActionResult(this);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteOrderCommand(id);

        var deleteResult = await mediator.Send(command, cancellationToken);

        return deleteResult.ToActionResult(this);
    }
}
