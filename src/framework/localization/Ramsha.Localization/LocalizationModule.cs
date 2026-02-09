using Microsoft.Extensions.DependencyInjection;

using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;

public class LocalizationModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context.DependsOn<LocalizationAbstractionsModule>();
    }
    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services.AddRamshaLocalizationServices();

        context.Services.AddTransient<ILocalizationResourceProvider, JsonLocalizationResourceProvider>();
    }
}
