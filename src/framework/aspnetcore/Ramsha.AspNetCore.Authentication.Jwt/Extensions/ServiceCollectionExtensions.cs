
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddRamshaJwt(this AuthenticationBuilder builder)
       => builder.AddRamshaJwt(JwtBearerDefaults.AuthenticationScheme, _ => { });

    public static AuthenticationBuilder AddRamshaJwt(this AuthenticationBuilder builder, Action<JwtBearerOptions> configureOptions)
        => builder.AddRamshaJwt(JwtBearerDefaults.AuthenticationScheme, configureOptions);

    public static AuthenticationBuilder AddRamshaJwt(this AuthenticationBuilder builder, string authenticationScheme, Action<JwtBearerOptions> configureOptions)
        => builder.AddRamshaJwt(authenticationScheme, "Bearer", configureOptions);

    public static AuthenticationBuilder AddRamshaJwt(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<JwtBearerOptions> configureOptions)
    {
        return builder.AddJwtBearer(authenticationScheme, displayName, options =>
        {
            configureOptions?.Invoke(options);

            options.Events ??= new JwtBearerEvents();
            var previousOnChallenge = options.Events.OnChallenge;
            options.Events.OnChallenge = async eventContext =>
            {
                await previousOnChallenge(eventContext);
            };
        });
    }
}
