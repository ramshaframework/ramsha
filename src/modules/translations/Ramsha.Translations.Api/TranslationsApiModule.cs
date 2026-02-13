using Ramsha.AspNetCore.Mvc;
using Ramsha.Translations.Contracts;

namespace Ramsha.Translations.Api;

public class TranslationsApiModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<TranslationsContractsModule>()
        .DependsOn<AspNetCoreMvcModule>();
    }
}
