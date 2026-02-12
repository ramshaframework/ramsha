

using Microsoft.EntityFrameworkCore;
using Ramsha.EntityFrameworkCore;
using Ramsha.Identity.Persistence;
using Ramsha.Identity.Domain;
using Ramsha.Permissions.Persistence;
using Ramsha.SettingsManagement.Persistence;
using SimpleAppDemo.Identity;
using SimpleAppDemo.LocalizationModule;


public class AppDbContext(DbContextOptions<AppDbContext> options)
: RamshaEFDbContext<AppDbContext>(options)
{
    public DbSet<LocalizationText> LocalizationTexts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
        .ConfigureIdentity<AppUser, RamshaIdentityRole<int>, int>()
        .ConfigurePermissions()
        .ConfigureSettingsManagement();
    }
}