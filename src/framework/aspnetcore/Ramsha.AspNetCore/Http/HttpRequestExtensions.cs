
using Microsoft.AspNetCore.Http;

namespace Ramsha.AspNetCore.Http;

public static class HttpRequestExtensions
{
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
