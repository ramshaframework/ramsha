using Microsoft.Extensions.DependencyInjection;
using Ramsha.AspNetCore.Mvc;
using Ramsha.JwtAuth.AspNetCore;

namespace Ramsha.JwtAuth.Api;

public class JwtAuthApiModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<JwtAuthAspNetCoreModule>()
        .DependsOn<AspNetCoreMvcModule>();
    }

    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);
        context.PrepareOptions<IMvcBuilder>(builder =>
        {
            builder.AddJwtAuthGenericControllers();
        });
    }
}
