using Microsoft.Extensions.DependencyInjection;
using Ramsha.EntityFrameworkCore;
using Ramsha.JwtAuth.Domain;

namespace Ramsha.JwtAuth.Persistence;

public class JwtAuthPersistenceModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<JwtAuthDomainModule>()
        .DependsOn<EntityFrameworkCoreModule>();
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services.AddJwtAuthPersistenceServices();
    }
}
