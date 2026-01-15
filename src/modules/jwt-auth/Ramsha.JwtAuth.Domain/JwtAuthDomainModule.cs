using Ramsha.Common.Domain;
using Ramsha.JwtAuth.Shared;

namespace Ramsha.JwtAuth.Domain;

public class JwtAuthDomainModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<CommonDomainModule>()
        .DependsOn<JwtAuthSharedModule>();
    }
}