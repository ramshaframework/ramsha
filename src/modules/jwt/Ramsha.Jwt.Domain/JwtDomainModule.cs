using Ramsha.Common.Domain;

namespace Ramsha.Jwt.Domain;

public class JwtDomainModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<CommonDomainModule>();
    }
}