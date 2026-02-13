using Ramsha.Translations.Api;
using Ramsha.Translations.Application;
using Ramsha.Translations.Persistence;

namespace Ramsha.Translations;

public class TranslationsModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<TranslationsApplicationModule>()
        .DependsOn<TranslationsApiModule>()
        .DependsOn<TranslationsPersistenceModule>();

    }
}
