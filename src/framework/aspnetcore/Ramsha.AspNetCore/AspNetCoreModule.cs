using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ramsha.AspNetCore.Security.Claims;
using Ramsha.Common.Domain;
using Ramsha.Security.Claims;


namespace Ramsha.AspNetCore;

public class AspNetCoreModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context.DependsOn<CommonDomainModule>();
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services.AddRamshaAspNetCoreService();
    }
}






