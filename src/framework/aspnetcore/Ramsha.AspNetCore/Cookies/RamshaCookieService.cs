using Microsoft.AspNetCore.Http;

namespace Ramsha.AspNetCore;

public class RamshaCookieService(IHttpContextAccessor httpContextAccessor) : IRamshaCookieService
{
    public void SetCookieValue(string key, string value, CookieOptions? options = null)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Append(key, value, options ?? new());
    }

    public void RemoveCookie(string key)
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
    }

    public string? GetCookieValue(string key)
    {
        return httpContextAccessor.HttpContext?.Request.Cookies[key];
    }
}

