using Microsoft.Extensions.DependencyInjection;
using Ramsha.EntityFrameworkCore;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Persistence;

public class TranslationsPersistenceModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<TranslationsDomainModule>()
        .DependsOn<EntityFrameworkCoreModule>();
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        context.Services.AddRamshaDbContext<ITranslationsDbContext, TranslationsDbContext>(options =>
        {
            options.AddRepository<Translation, ITranslationRepository, EfCoreTranslationRepository>();
        });
    }
}