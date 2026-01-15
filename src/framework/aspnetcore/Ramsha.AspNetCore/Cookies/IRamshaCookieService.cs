using Microsoft.AspNetCore.Http;

namespace Ramsha.AspNetCore;

public interface IRamshaCookieService
{
    void SetCookieValue(string key, string value, CookieOptions? options = null);
    string? GetCookieValue(string key);
    void RemoveCookie(string key);
}

