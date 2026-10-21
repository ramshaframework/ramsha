

using Ramsha.Security;

namespace Ramsha.Authorization;

public class AuthorizationModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context.DependsOn<SecurityModule>();
    }
    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        context.Services.AddAuthorizationServices();

        context.Configure<RamshaPermissionOptions>(options =>
        {
            options.ValueResolvers.Add<UserPermissionValueResolver>();
        });
    }
}
