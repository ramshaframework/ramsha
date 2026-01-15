using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.AspNetCore;
using Ramsha.Identity.Domain;

namespace Ramsha.Identity.AspNetCore;

public class IdentityAspNetCoreModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<IdentityDomainModule>()
        .DependsOn<AspNetCoreModule>();
    }

    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);

        context.PrepareOptions<RamshaIdentityOptions>(options =>
        {
            options.ConfigureIdentity(builder =>
            {
                builder.AddDefaultTokenProviders()
              .AddSignInManager();
            });

        });

    }


    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services
               .AddAuthentication(o =>
               {
                   o.DefaultScheme = IdentityConstants.ApplicationScheme;
                   o.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
               }).AddIdentityCookies();

    }
}
