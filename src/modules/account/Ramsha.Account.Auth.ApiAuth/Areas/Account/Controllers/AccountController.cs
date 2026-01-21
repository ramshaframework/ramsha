using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Auth.Shared;
using Ramsha.Identity.Domain;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Ramsha.Account.Auth.ApiAuth;


[Area("account")]
[ControllerName("account")]
public class AccountController<TUser, TId, TLoginRequest, TLoginResponse>(
    SignInManager<TUser> signInManager
)
: RamshaApiController
where TId : IEquatable<TId>
where TUser : RamshaIdentityUserBase<TId>
where TLoginRequest : RamshaLoginRequest, new()
where TLoginResponse : RamshaLoginResponse, new()
{
    [HttpPost("login")]
    public virtual async Task<ActionResult<TLoginResponse>> Login(TLoginRequest request)
    {
        await BeforeLogin(request);

        var signInResult = await signInManager.PasswordSignInAsync(
        request.UserName,
        request.Password,
        request.RememberMe,
        true
        );

        return RamshaResult(await ReturnFromLoginResult(request, signInResult, new TLoginResponse()));
    }

    [HttpPost("logout")]
    public virtual async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return RamshaResult(RamshaResults.NoContent);
    }


    protected virtual Task BeforeLogin(TLoginRequest request)
    {
        return Task.CompletedTask;
    }

    protected virtual Task<RamshaResult<TLoginResponse>> ReturnFromLoginResult(TLoginRequest request, SignInResult result, TLoginResponse response)
    {
        if (result.RequiresTwoFactor)
        {
            return Task.FromResult<RamshaResult<TLoginResponse>>(
                RamshaResults.Invalid(RamshaAccountAuthErrorsCodes.RequiresTwoFactor)
                );
        }

        if (result.IsLockedOut)
        {
            return Task.FromResult<RamshaResult<TLoginResponse>>(
                RamshaResults.Invalid(RamshaAccountAuthErrorsCodes.IsLockedOut)
                );
        }

        if (result.IsNotAllowed)
        {
            return Task.FromResult<RamshaResult<TLoginResponse>>(
               RamshaResults.Invalid(RamshaAccountAuthErrorsCodes.IsNotAllowed)
               );
        }

        if (!result.Succeeded)
        {
            return Task.FromResult<RamshaResult<TLoginResponse>>(
              RamshaResults.Invalid(RamshaAccountAuthErrorsCodes.Default)
              );
        }

        response.Username = request.UserName;
        return Task.FromResult<RamshaResult<TLoginResponse>>(response);
    }
}
