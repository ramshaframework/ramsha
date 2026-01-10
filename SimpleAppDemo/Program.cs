using Microsoft.EntityFrameworkCore;
using Ramsha;
using Ramsha.Caching;
using Ramsha.Identity.Domain;
using SimpleAppDemo;

var builder = WebApplication.CreateBuilder(args);

var ramsha = builder.Services.AddRamsha(ramsha =>
{
    ramsha
    .AddIdentityModule()
    .AddAccountModule()
    .AddSettingsManagementModule()
    .AddPermissionsModule()
    .AddEFSqlServerModule()
    .AddCachingModule();
});

builder.Services.AddRamshaDbContext<AppDbContext>();

var app = builder.Build();


app.MapPost("users", async (int count, RamshaIdentityUserManager<RamshaIdentityUser> userManager) =>
{
    int succeededCount = 0;
    var users = DataGenerator.GenerateUserList(count, true);
    foreach (var user in users)
    {
        var result = await userManager.CreateAsync(user, "qawsed");
        if (result.Succeeded)
        {
            succeededCount += 1;
        }
    }

    return Results.Ok(succeededCount);
});


app.MapGet("users", async (IIdentityUserRepository<RamshaIdentityUser, Guid> userRepository) =>
{
    return await GetUsers(userRepository);
});


app.MapGet("cached-users", async (IRamshaCache cache, IIdentityUserRepository<RamshaIdentityUser, Guid> userRepository) =>
{
    return await cache.GetOrCreateAsync("users",
    async (ct) => await GetUsers(userRepository)
,
    new RamshaCacheEntryOptions
    {
        Expiration = TimeSpan.FromMinutes(2),
        LocalCacheExpiration = TimeSpan.FromMinutes(2),
    });
});

app.MapDelete("remove-users-cache", async (IRamshaCache cache) =>
{
    await cache.RemoveAsync("users");
});

app.UseRamsha();

app.Run();



static async Task<List<RamshaIdentityUser>> GetUsers(IIdentityUserRepository<RamshaIdentityUser, Guid> repository)
{
    return await repository.GetListAsync(
        x => x.UserName == x.UserName
         && x.EmailConfirmed == false
         && x.PasswordHash != null
          && !x.UserName.StartsWith("0076555359")
         && !x.NormalizedUserName.StartsWith("0076555359")
          && !x.NormalizedEmail.StartsWith("0076555359")
          && !x.Email.StartsWith("0076555359"),
        [x => x.Roles, x => x.Claims, c => c.Logins]);
}
