using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Ramsha;
using Ramsha.Identity.Application;
using Ramsha.Identity.Domain;
using SimpleAppDemo.Identity;

var builder = WebApplication.CreateBuilder(args);

var ramsha = builder.Services.AddRamsha(ramsha =>
{
    ramsha
    .AddCaching()
    .AddIdentity()
    .AddAccount()
    .AddAccountApiAuth()
    .AddAccountWebAuth()
    .AddSettingsManagement()
    .AddPermissions()
    .AddEFPostgreSql();


    ramsha.PrepareOptions<RamshaTypeReplacementOptions>(options =>
    {
        options.ReplaceIdentityEntities<AppUser, RamshaIdentityRole<int>, int>();
        options.ReplaceIdentityUserService<AppUserService>();
    });
});

builder.Services.AddRamshaDbContext<AppDbContext>();

var app = builder.Build();


app.MapGet("query", (UserManager<AppUser> repository) =>
{
    return repository.Users.ToList();
});

app.MapDelete("test", () =>
{
    return "this is test endpoint";
});

app.UseRamsha();

app.Run();
