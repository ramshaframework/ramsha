
namespace Ramsha.Account.Contracts;

public interface IRamshaAccountService<TRegisterDto> : IRamshaAccountServiceBase
where TRegisterDto : RamshaRegisterDto, new()
{
    Task<IRamshaResult> RegisterAsync(TRegisterDto registerDto);
    Task<IRamshaResult> ConfirmEmailAsync(string email, string token);
    Task<IRamshaResult> ResetPasswordAsync(string username, string token, string newPassword);
}


public interface IRamshaAccountServiceBase
{

}