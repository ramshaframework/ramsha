
namespace Ramsha.Account.Auth.ApiAuth;

public class RamshaLoginRequest
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
    public bool RememberMe { get; set; }
}
