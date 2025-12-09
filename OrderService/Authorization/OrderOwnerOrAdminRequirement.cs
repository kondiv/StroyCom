using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure;
using System.Security.Claims;

namespace OrderService.Authorization;

public sealed class OrderOwnerOrAdminRequirement : IAuthorizationRequirement
{
}

public sealed class OrderOwnerOrAdminRequirementHandler(
    ServiceContext dbContext,
    IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<OrderOwnerOrAdminRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OrderOwnerOrAdminRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if(httpContext is null)
        {
            return;
        }

        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return;
        }

        var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;
        if (userRole is not null && userRole.Equals("admin", StringComparison.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
            return;
        }

        if (!httpContext.Request.RouteValues.TryGetValue("id", out var orderIdObj) ||
            !Guid.TryParse(orderIdObj?.ToString(), out var orderId))
        {
            return;
        }

        bool isOwner = await dbContext
            .Orders
            .AnyAsync(o => o.Id == orderId && o.UserId == userId);

        if (isOwner)
        {
            context.Succeed(requirement);
        }
    }
}
