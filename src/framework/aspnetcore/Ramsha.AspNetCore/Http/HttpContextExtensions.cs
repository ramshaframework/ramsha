
using Microsoft.AspNetCore.Http;

namespace Ramsha.AspNetCore;

public static class HttpContextExtensions
{

    public static void SetCookieValue(this HttpContext httpContext, string key, string value, CookieOptions? options = null)
    {
        httpContext?.Response.Cookies.Append(key, value, options ?? new());
    }

    public static void RemoveCookie(this HttpContext httpContext, string key)
    {
        httpContext?.Response.Cookies.Delete(key);
    }

    public static string? GetCookieValue(this HttpContext httpContext, string key)
    {
        return httpContext?.Request.Cookies[key];
    }

    public static bool IsApiRequest(this HttpRequest request)
    {
        if (request.Headers.TryGetValue("Accept", out var acceptHeaders))
        {
            if (acceptHeaders.Any(h => h.Contains("application/json", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        if (!string.IsNullOrEmpty(request.ContentType) && request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (request.Headers.TryGetValue("X-Requested-With", out var xRequestedWith))
        {
            if (xRequestedWith.Any(h => h.Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }
}
