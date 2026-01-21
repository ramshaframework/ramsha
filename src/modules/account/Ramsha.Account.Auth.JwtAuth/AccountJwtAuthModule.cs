using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Shared;
using Ramsha.Jwt.Domain;

namespace Ramsha.Account.Auth.JwtAuth;

public class AccountJwtAuthModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context.DependsOn<JwtDomainModule>();
    }

    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);
        context.PrepareOptions<IMvcBuilder>(builder =>
        {
            builder.AddJwtAuthGenericControllers();
        });
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        var types = context.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();

        var jwtValidationOptionsAction = context.Services.GetPrepareConfigureActions<RamshaJwTValidationOptions>();
        context.Services.Configure<RamshaJwTValidationOptions>(jwtValidationOptionsAction.Configure);

        var jwtValidationOptions = jwtValidationOptionsAction.Configure();

        var typesOptions = context.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();

        context.Services.AddScoped<RefreshTokenGenerator>();


        context.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddRamshaJwt(o =>
        {
            o.RequireHttpsMetadata = false;
            o.SaveToken = false;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = jwtValidationOptions.Issuer,
                ValidAudience = jwtValidationOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtValidationOptions.SecurityKey))
            };
            o.Events = new JwtBearerEvents()
            {
                OnTokenValidated = async context =>
                {
                    dynamic signInManager = context.HttpContext.RequestServices.GetRequiredService(typeof(SignInManager<>).MakeGenericType(typesOptions.GetUserTypeOrBase()));
                    var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                    if (claimsIdentity.Claims?.Any() != true)
                        context.Fail("This token has no claims.");

                    var securityStamp = claimsIdentity.FindFirst("AspNet.Identity.SecurityStamp");
                    if (securityStamp is null)
                        context.Fail("This token has no secuirty stamp");

                    var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                    if (validatedUser == null)
                        context.Fail("Token secuirty stamp is not valid.");
                },

            };
        });
    }



}
