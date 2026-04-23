using Microsoft.AspNetCore.Authorization;

namespace app.Application.Authorization;

public class RoleRequirement : IAuthorizationRequirement
{
    public int RoleId { get; }

    public RoleRequirement(int roleId)
    {
        RoleId = roleId;
    }
}
