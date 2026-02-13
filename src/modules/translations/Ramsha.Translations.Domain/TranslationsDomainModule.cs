using Microsoft.Extensions.DependencyInjection;
using Ramsha.Common.Domain;
using Ramsha.Localization;
using Ramsha.Translations.Shared;

namespace Ramsha.Translations.Domain;

public class TranslationsDomainModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<TranslationsSharedModule>()
        .DependsOn<CommonDomainModule>();
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services.AddRamshaDomainManager<TranslationsManager>();

        context.Services.Configure<RamshaLocalizationOptions>(options =>
        {
            options.ResourcesStores.Add<TranslationsResourcesStore>();
        });
    }
}
