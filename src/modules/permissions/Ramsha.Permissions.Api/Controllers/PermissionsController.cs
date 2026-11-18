
using Microsoft.AspNetCore.Mvc;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Permissions.Contracts;
using Ramsha.Permissions.Shared;

namespace Ramsha.Permissions.Api;

public class PermissionsController(IPermissionsService permissionsService) : RamshaApiController
{
    [HttpPost("{permissionName}")]
    public async Task<IActionResult> AssignAsync(string permissionName, string providerName, string providerKey)
    {
        return RamshaResult(await permissionsService.AssignAsync(permissionName, providerName, providerKey));
    }
    [HttpGet]
    public async Task<ActionResult<List<PermissionInfo>>> GetAllAsync(string providerName, string providerKey)
    {
        return RamshaResult(await permissionsService.GetAllAsync(providerName, providerKey));
    }
    [HttpGet("{permissionName}")]
    public async Task<ActionResult<PermissionInfo>> GetAsync(string permissionName, string providerName, string providerKey)
    {
        return RamshaResult(await permissionsService.GetAsync(permissionName, providerName, providerKey));

    }
    [HttpDelete("{permissionName}")]
    public async Task<IActionResult> RevokeAsync(string permissionName, string providerName, string providerKey)
    => RamshaResult(await permissionsService.RevokeAsync(permissionName, providerName, providerKey));


    [HttpGet("definitions")]
    public async Task<ActionResult<IReadOnlyList<PermissionDefinitionDto>>> GetDefinitions(CancellationToken cancellationToken = default)
    => Ok(await permissionsService.GetDefinitionsAsync(cancellationToken));
}
