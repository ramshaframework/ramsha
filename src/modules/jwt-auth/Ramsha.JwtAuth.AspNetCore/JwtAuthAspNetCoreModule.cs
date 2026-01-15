using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ramsha.AspNetCore.Authentication.Jwt;
using Ramsha.Identity.AspNetCore;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Shared;
using Ramsha.JwtAuth.Domain;
using Ramsha.JwtAuth.Shared;

namespace Ramsha.JwtAuth.AspNetCore;

public class JwtAuthAspNetCoreModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<AspNetCoreJwtAuthenticationModule>()
        .DependsOn<IdentityAspNetCoreModule>();
    }


    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        var types = context.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();

        var jwtValidationOptionsAction = context.Services.GetPrepareConfigureActions<RamshaJwTValidationOptions>();
        context.Services.Configure<RamshaJwTValidationOptions>(jwtValidationOptionsAction.Configure);

        var jwtValidationOptions = jwtValidationOptionsAction.Configure();

        var identityTypesOptions = context.Services.ExecutePreparedOptions<RamshaIdentityTypesOptions>();

        context.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        context.Services.AddScoped(
            typeof(IJwtAuthAuthenticator<,,>)
            .MakeGenericType(
                types.GetOrBase<RamshaJwtAuthenticateRequest>(),
                types.GetOrBase<RamshaJwtRefreshRequest>(),
                types.GetOrBase<RamshaJwtAuthResponse>()
                ),
            typeof(JwtAuthAuthenticator<,,,,>)
            .MakeGenericType(
                identityTypesOptions.UserType,
                identityTypesOptions.KeyType,
                types.GetOrBase<RamshaJwtAuthenticateRequest>(),
                types.GetOrBase<RamshaJwtRefreshRequest>(),
                types.GetOrBase<RamshaJwtAuthResponse>()
            )
             );



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
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(RamshaResults.Unauthenticated(), new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    });
                    return context.Response.WriteAsync(result);
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";

                    var result = JsonSerializer.Serialize(RamshaResults.Forbidden(), new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles
                    });
                    return context.Response.WriteAsync(result);
                },
                OnTokenValidated = async context =>
                {
                    dynamic signInManager = context.HttpContext.RequestServices.GetRequiredService(typeof(SignInManager<>).MakeGenericType(identityTypesOptions.UserType));
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
