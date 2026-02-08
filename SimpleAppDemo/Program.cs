using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ramsha;
using Ramsha.Common.Domain;
using Ramsha.Identity.Application;
using Ramsha.Identity.Domain;
using Ramsha.Localization;
using Ramsha.Localization.Abstractions;
using SimpleAppDemo.Controllers;
using SimpleAppDemo.Identity;
using SimpleAppDemo.Resources;

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

builder.Services.Configure<RamshaLocalizationOptions>(options =>
{
    options.Resources.Add<AppResource>()
    .SetPath("App");

    options.Resources.Add<TestController>()
    .Extend<AppResource>();
    options.SupportedLanguages = [new("ar", "ar")];
});

var app = builder.Build();

app.UseRamsha();


app.MapDelete("test", () =>
{
    return "this is test endpoint";
});

app.Run();
