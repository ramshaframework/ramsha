using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ramsha;
using Ramsha.Common.Domain;
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





app.MapDelete("test", () =>
{
    return "this is test endpoint";
});

app.UseRamsha();

app.Run();
