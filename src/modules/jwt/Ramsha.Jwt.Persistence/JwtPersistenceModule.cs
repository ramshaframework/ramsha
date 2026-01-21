using Microsoft.Extensions.DependencyInjection;
using Ramsha.EntityFrameworkCore;
using Ramsha.Jwt.Domain;

namespace Ramsha.Jwt.Persistence;

public class JwtPersistenceModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<JwtDomainModule>()
        .DependsOn<EntityFrameworkCoreModule>();
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services.AddJwtPersistenceServices();
    }
}
