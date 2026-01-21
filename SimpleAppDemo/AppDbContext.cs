using Microsoft.EntityFrameworkCore;
using Ramsha.EntityFrameworkCore;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Persistence;
using Ramsha.Permissions.Persistence;
using Ramsha.SettingsManagement.Persistence;
using SimpleAppDemo.Identity;

public class AppDbContext(DbContextOptions<AppDbContext> options)
: RamshaEFDbContext<AppDbContext>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
        .ConfigureIdentity<AppUser, RamshaIdentityRole<int>, int>()
        .ConfigurePermissions()
        .ConfigureSettingsManagement();
    }
}
