using Microsoft.AspNetCore.Mvc;
using Ramsha.Account.Contracts;
using Ramsha.AspNetCore.Mvc;

namespace Ramsha.Account.Api;

[Area("account")]
[ControllerName("account")]
public class RamshaAccountController<TRegisterDto>(IRamshaAccountService<TRegisterDto> accountService)
: RamshaApiController
where TRegisterDto : RamshaRegisterDto, new()
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(TRegisterDto registerDto)
     => await UnitOfWork(async () => RamshaResult(await accountService.RegisterAsync(registerDto)));

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(string userName, string token, string newPassword)
     => RamshaResult(await accountService.ResetPasswordAsync(userName, token, newPassword));

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(string email, string token)
     => RamshaResult(await accountService.ConfirmEmailAsync(email, token));


}
