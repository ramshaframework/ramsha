
using Ramsha.Authorization;
using Ramsha.Common.Application;
using Ramsha.Permissions.Contracts;
using Ramsha.Permissions.Domain;
using Ramsha.Permissions.Shared;

namespace Ramsha.Permissions.Application;

public class PermissionsService(
    IPermissionManager permissionManager,
IPermissionDefinitionManager definitionManager) : RamshaService, IPermissionsService
{
    public async Task<IRamshaResult> AssignAsync(string permissionName, string providerName, string providerKey)
    {
        return await permissionManager.AssignAsync(permissionName, providerName, providerKey);
    }

    public async Task<RamshaResult<List<PermissionInfo>>> GetAllAsync(string providerName, string providerKey)
    {
        return await permissionManager.GetAllAsync(providerName, providerKey);
    }

    public async Task<RamshaResult<PermissionInfo>> GetAsync(string permissionName, string providerName, string providerKey)
    {
        return await permissionManager.GetAsync(permissionName, providerName, providerKey);

    }

    public async Task<IReadOnlyList<PermissionDefinitionDto>> GetDefinitionsAsync(CancellationToken cancellationToken = default)
    {
        var defs = await definitionManager.GetPermissionsAsync();
        return defs.Select(x => new PermissionDefinitionDto(x.Name, x.GroupName, x.Description, x.Providers))
        .ToList();
    }

    public async Task<IRamshaResult> RevokeAsync(string permissionName, string providerName, string providerKey)
    {
        return await permissionManager.RevokeAsync(permissionName, providerName, providerKey);

    }
}
