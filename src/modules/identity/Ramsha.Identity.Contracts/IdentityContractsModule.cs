
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Contracts;

public class IdentityContractsModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<IdentitySharedModule>();
    }
    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);
        context.PrepareOptions<RamshaTypeReplacementOptions>(options => { });
    }
}
