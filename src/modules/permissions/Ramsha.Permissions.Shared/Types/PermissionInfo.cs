using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.Permissions.Shared;


public record PermissionInfo(string Name, bool IsAssigned);
public record PermissionDetail(string Name, bool IsAssigned, List<PermissionProviderInfo>? Providers = null);


