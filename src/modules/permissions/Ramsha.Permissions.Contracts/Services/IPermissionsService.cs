using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ramsha.Permissions.Shared;

namespace Ramsha.Permissions.Contracts;


public interface IPermissionsService
{
    Task<RamshaResult<PermissionInfo>> GetAsync(string permissionName, string providerName, string providerKey);
    Task<RamshaResult<List<PermissionInfo>>> GetAllAsync(string providerName, string providerKey);
    Task<IRamshaResult> AssignAsync(string permissionName, string providerName, string providerKey);
    Task<IRamshaResult> RevokeAsync(string permissionName, string providerName, string providerKey);
    Task<IReadOnlyList<PermissionDefinitionDto>> GetDefinitionsAsync(CancellationToken cancellationToken = default);

}
