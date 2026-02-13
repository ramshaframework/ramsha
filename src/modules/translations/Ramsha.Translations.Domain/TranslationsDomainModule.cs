using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        context.Services.AddRamshaDomainManager<LanguageManager>();

        context.Services.Configure<RamshaLocalizationOptions>(options =>
        {
            options.ResourcesStores.Add<TranslationsResourcesStore>();
        });

        context.Services.Replace(
            ServiceDescriptor.Transient<ILocalizationLanguagesProvider, TranslationsLanguagesProvider>()
            );

        context.Services.Configure<RamshaHooksOptions>(options =>
        {
            options.InitHookContributors.Add<TranslationsDomainInitHookContributor>();
        });
    }
}
