using app.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace app.Application.Authorization;

public class RoleHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(JwtClaimNames.RoleId);
        if (roleClaim is not null && roleClaim.Value == requirement.RoleId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
