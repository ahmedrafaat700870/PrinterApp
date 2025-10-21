using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PrinterApp.Web.PermissionAuthorizationHandler;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

    public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _fallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        // Format: Permission.PERMISSION_CODE.RoleName
        // Example: Permission.USERS.Manage
        if (policyName.StartsWith("Permission.", StringComparison.OrdinalIgnoreCase))
        {
            var parts = policyName.Substring("Permission.".Length).Split('.');

            if (parts.Length == 2)
            {
                var permissionCode = parts[0];
                var roleName = parts[1];

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(permissionCode, roleName));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }
        }

        return _fallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}