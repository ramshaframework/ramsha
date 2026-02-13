using Microsoft.Extensions.DependencyInjection;
using Ramsha.Common.Application;
using Ramsha.LocalMessaging.Abstractions;
using Ramsha.Translations.Contracts;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Application;

public class TranslationsApplicationModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<TranslationsContractsModule>()
        .DependsOn<TranslationsDomainModule>()
        .DependsOn<CommonApplicationModule>();
    }

    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);

        context.PrepareOptions<LocalMessagingOptions>(options =>
        {
            options.AddMessagesFromAssembly<TranslationsApplicationModule>();
        });
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        context.Services.AddRamshaService<ITranslationsService, TranslationsService>();
        context.Services.AddRamshaService<ILanguagesService, LanguagesService>();
    }
}
