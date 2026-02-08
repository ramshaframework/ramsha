using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ramsha.AspNetCore;


public class RamshaRequestLocalizationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IRamshaRequestLocalizationOptionsProvider optionsProvider)
{
    public const string HttpContextItemName = "__RamshaCultureCookie";

    public async Task InvokeAsync(HttpContext context)
    {
        var middleware = new RequestLocalizationMiddleware(
            next,
            new OptionsWrapper<RequestLocalizationOptions>(
                await optionsProvider.GetOptionsAsync()
            ),
            loggerFactory
        );

        context.Response.OnStarting(() =>
        {
            if (context.Items[HttpContextItemName] == null)
            {
                var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                if (requestCultureFeature?.Provider is QueryStringRequestCultureProvider)
                {
                    context.SetCookieValue
                    (CookieRequestCultureProvider.DefaultCookieName,
                     CookieRequestCultureProvider.MakeCookieValue(requestCultureFeature.RequestCulture),
            new CookieOptions
            {
                IsEssential = true,
                Expires = DateTime.Now.AddYears(2)
            });
                }
            }

            return Task.CompletedTask;
        });

        await middleware.Invoke(context);
    }
}
