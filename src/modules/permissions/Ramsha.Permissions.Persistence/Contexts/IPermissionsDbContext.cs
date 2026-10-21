
using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.Permissions.Domain;

namespace Ramsha.Permissions.Persistence;

[ConnectionString(PermissionsDbContextConstants.ConnectionStringName)]
public interface IPermissionsDbContext : IRamshaEFDbContext
{
    DbSet<Permission> Permissions { get; }
}
