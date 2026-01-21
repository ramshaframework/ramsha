using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Domain;

namespace Ramsha.Account.Auth.WebAuth;

[Area("account")]
[ControllerName("Account")]
public class AccountController<TUser, TId, TLoginViewModel>(
    SignInManager<TUser> signInManager,
    ILogger<AccountController<TUser, TId, TLoginViewModel>> logger
) : RamshaController
where TId : IEquatable<TId>
where TUser : RamshaIdentityUserBase<TId>
where TLoginViewModel : RamshaLoginViewModel, new()

{
    [HttpGet("login")]
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        await HttpContext.SignOutAsync(
            IdentityConstants.ExternalScheme);

        var vm = new TLoginViewModel
        {
            ReturnUrl = returnUrl,
            ExternalLogins =
                (await signInManager
                    .GetExternalAuthenticationSchemesAsync())
                    .ToList()
        };

        return View(vm);
    }


    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(TLoginViewModel model)
    {
        model.ReturnUrl ??= Url.Content("~/");

        model.ExternalLogins =
            (await signInManager
                .GetExternalAuthenticationSchemesAsync())
                .ToList();

        if (!ModelState.IsValid)
            return View(model);

        var result = await signInManager.PasswordSignInAsync(
            model.UserName,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            logger.LogInformation("User logged in.");
            return LocalRedirect(model.ReturnUrl);
        }

        if (result.RequiresTwoFactor)
        {
            return RedirectToAction(
                "LoginWith2fa",
                new
                {
                    returnUrl = model.ReturnUrl,
                    rememberMe = model.RememberMe
                });
        }

        if (result.IsLockedOut)
        {
            logger.LogWarning("User account locked out.");
            return RedirectToAction("Lockout");
        }

        ModelState.AddModelError(
            string.Empty,
            "Invalid login attempt.");

        return View(model);
    }
}
