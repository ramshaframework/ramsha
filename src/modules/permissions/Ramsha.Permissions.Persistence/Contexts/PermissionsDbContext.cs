
using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.Permissions.Domain;

namespace Ramsha.Permissions.Persistence;

[ConnectionString(PermissionsDbContextConstants.ConnectionStringName)]
public sealed class PermissionsDbContext(DbContextOptions<PermissionsDbContext> options)
: RamshaEFDbContext<PermissionsDbContext>(options), IPermissionsDbContext
{
    public DbSet<Permission> Permissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigurePermissions();
    }
}
