using Ramsha.Security;

namespace Ramsha.AspNetCore.Authentication.Jwt;

public class AspNetCoreJwtAuthenticationModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context.DependsOn<SecurityModule>();
    }

}
