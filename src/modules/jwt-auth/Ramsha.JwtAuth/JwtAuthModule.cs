using Ramsha.JwtAuth.Api;
using Ramsha.JwtAuth.Persistence;


namespace Ramsha.JwtAuth;

public class JwtAuthModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<JwtAuthApiModule>()
        .DependsOn<JwtAuthPersistenceModule>();
    }
}
