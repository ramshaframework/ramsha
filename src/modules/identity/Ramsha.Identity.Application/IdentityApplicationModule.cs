using Ramsha.Identity.Contracts;
using Ramsha.Identity.Domain;
using Ramsha.Common.Application;

namespace Ramsha.Identity.Application;

public class IdentityApplicationModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
              .DependsOn<IdentityContractsModule>()
        .DependsOn<IdentityDomainModule>()
        .DependsOn<CommonApplicationModule>();
    }
    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);
    }
    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services.AddRamshaIdentityApplicationServices();
    }
}
