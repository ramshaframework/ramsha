using Microsoft.Extensions.DependencyInjection;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.AspNetCore;

namespace Ramsha.Account.Auth.ApiAuth;

public class AccountApiAuthModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<IdentityAspNetCoreModule>()
        .DependsOn<AspNetCoreMvcModule>();
    }

    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);
        context.PrepareOptions<IMvcBuilder>(builder =>
        {
            builder.AddAccountApiAuthGenericControllers();
        });
    }
}
