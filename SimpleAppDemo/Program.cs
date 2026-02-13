
using Ramsha;
using Ramsha.Identity.Application;
using Ramsha.Identity.Domain;
using Ramsha.Localization;
using Ramsha.LocalMessaging.Abstractions;
using SimpleAppDemo;
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
    .AddEFPostgreSql()
    .AddTranslations();


    ramsha.PrepareOptions<RamshaTypeReplacementOptions>(options =>
    {
        options.ReplaceIdentityEntities<AppUser, RamshaIdentityRole<int>, int>();
        options.ReplaceIdentityUserService<AppUserService>();
    });
});

builder.Services.AddRamshaDbContext<AppDbContext>(options => options.AddDefaultRepositories(true));

builder.Services.Configure<RamshaLocalizationOptions>(options =>
{
    options.Resources.Add<AppResource>()
    .SetPath("App");

    options.Resources.Add<AdditionalResource>()
    .Extend<AppResource>().SetPath("Additional");

    options.SupportedLanguages = [new("ar", "ar"), new("en", "en")];

});

var app = builder.Build();

app.UseRamsha();


app.MapDelete("test", () =>
{
    return "this is test endpoint";
});

app.Run();
