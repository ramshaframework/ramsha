using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.Caching;
using Ramsha.Localization;
using Ramsha.Security;
using Ramsha.Settings;

namespace Ramsha.Common.Domain;

public class CommonDomainModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
         .DependsOn<CachingModule>()
        .DependsOn<LocalizationModule>()
        .DependsOn<SecurityModule>()
        .DependsOn<SettingsModule>();

    }
    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);

        context.PrepareOptions<TestDomainOptions>(options =>
        {
            options.Name = "domain";
        });
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);


        var testOptions = context.Services.ExecutePreparedOptions<TestDomainOptions>();




        context.Services.AddTransient<IConnectionStringResolver, DefaultConnectionStringResolver>();

        var config = context.Configuration;
        context.Services.Configure<ConnectionStringsOptions>(options =>
        {
            config.GetSection("ConnectionStrings").Bind(options.ConnectionStrings);
            options.Initialize();
        });
        context.Services.Configure<GlobalQueryFilterOptions>(config);

        context.Services.AddCommonDomainServices();
    }
}