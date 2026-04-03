

using Microsoft.EntityFrameworkCore;
using Ramsha.EntityFrameworkCore;
using Ramsha.Identity.Persistence;
using Ramsha.Identity.Domain;
using Ramsha.Permissions.Persistence;
using Ramsha.SettingsManagement.Persistence;
using SimpleAppDemo.Identity;
using SimpleAppDemo.LocalizationModule;
using Ramsha.Translations.Persistence;
using Ramsha.Common.Domain;


public class Org : Entity<int>
{
    public string Name { get; set; }
}
public class Country : Entity<int>
{
    public Org Org { get; set; }
    public int OrgId { get; set; }
    public string Name { get; set; }
}

public class PriceDiscount : Entity<int>
{
    public int PriceId { get; set; }
    public decimal Value { get; set; }
}

public class Price : Entity<int>
{
    public int InventoryId { get; set; }
    public decimal Value { get; set; }
    public List<PriceDiscount> Discounts { get; set; }
}
public class Inventory : Entity<int>
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public List<Price> Prices { get; set; }
    public Country Country { get; set; }
    public int CountryId { get; set; }


}
public class Product : Entity<int>
{
    public string Name { get; set; }
    public List<Inventory> Inventories { get; set; } = [];

}


public class AppDbContext(DbContextOptions<AppDbContext> options)
: RamshaEFDbContext<AppDbContext>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Org> Orgs { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
        .ConfigureIdentity<AppUser, RamshaIdentityRole<int>, int>()
        .ConfigurePermissions()
        .ConfigureSettingsManagement()
        .ConfigureTranslations();
    }
}