using Microsoft.AspNetCore.Authorization;
using PrinterApp.Services.Interfaces;
using System.Security.Claims;

namespace PrinterApp.Web.PermissionAuthorizationHandler;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionCode { get; }
    public string RoleName { get; }

    public PermissionRequirement(string permissionCode, string roleName)
    {
        PermissionCode = permissionCode;
        RoleName = roleName;
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUserPermissionService _userPermissionService;

    public PermissionAuthorizationHandler(IUserPermissionService userPermissionService)
    {
        _userPermissionService = userPermissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return;
        }

        // تحقق إذا المستخدم Admin
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        // تحقق من الصلاحية والدور
        var hasPermission = await _userPermissionService.UserHasPermissionRoleAsync(
            userId, requirement.PermissionCode, requirement.RoleName);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}