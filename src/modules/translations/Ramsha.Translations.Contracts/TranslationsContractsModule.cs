using Ramsha.Common.Contracts;
using Ramsha.Translations.Shared;

namespace Ramsha.Translations.Contracts;

public class TranslationsContractsModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<TranslationsSharedModule>()
        .DependsOn<CommonContractsModule>();
    }
}