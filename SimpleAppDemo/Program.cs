
using Ramsha;
using Ramsha.Common.Domain;
using Ramsha.Files;
using Ramsha.Identity.Application;
using Ramsha.Identity.Domain;
using Ramsha.Localization;
using Ramsha.LocalMessaging.Abstractions;
using SimpleAppDemo;
using SimpleAppDemo.Identity;
using SimpleAppDemo.Resources;

//oy2bs4zhdpy7l5b5qooqgbds26kw3ekelmrtn3d3h6fnha

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
    // .AddModule<DistributedMessagingModule>()



    ramsha.PrepareOptions<RamshaTypeReplacementOptions>(options =>
    {
        options.ReplaceIdentityEntities<AppUser, RamshaIdentityRole<int>, int>();
        options.ReplaceIdentityUserService<AppUserService>();
    });
});

// builder.Services.ConfigureWolverine(options =>
// {
//     options.Discovery.IncludeAssembly(typeof(Program).Assembly);
// });

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


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var orgRepository = services.GetRequiredService<IRepository<Org>>();

    try
    {
        List<Org> orgs = [
            new Org{
                Name = "test3"
            },
            new Org{
                Name = "test4"
            }
        ];

        await orgRepository.AddRangeAsync(orgs, true);
    }
    catch (Exception ex)
    {
        logger.LogError(ex.Message);
    }
}


app.Run();

