
using Ramsha.Account.Contracts;
using Ramsha.Common.Application;
using Ramsha.Identity.Domain;

namespace Ramsha.Account.Application;

public class RamshaAccountService<TUser, TRegisterDto>(RamshaIdentityUserManager<TUser> userManager)
: RamshaAccountService<TUser, RamshaIdentityRole, Guid, RamshaIdentityUserRole<Guid>, RamshaIdentityRoleClaim<Guid>, RamshaIdentityUserClaim<Guid>, RamshaIdentityUserLogin<Guid>, RamshaIdentityUserToken<Guid>, TRegisterDto>
(userManager)
where TUser : RamshaIdentityUser, new()
where TRegisterDto : RamshaRegisterDto, new()
{

}
public class RamshaAccountService<TUser, TRole, TId, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TUserToken, TRegisterDto>
(RamshaIdentityUserManager<TUser, TRole, TId, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TUserToken> userManager)
 : RamshaService, IRamshaAccountService<TRegisterDto>
     where TId : IEquatable<TId>
     where TUser : RamshaIdentityUser<TId, TUserClaim, TUserRole, TUserLogin, TUserToken>, new()
where TUserClaim : RamshaIdentityUserClaim<TId>, new()
where TUserRole : RamshaIdentityUserRole<TId>, new()
where TUserLogin : RamshaIdentityUserLogin<TId>, new()
where TUserToken : RamshaIdentityUserToken<TId>, new()
where TRole : RamshaIdentityRole<TId, TUserRole, TRoleClaim>, new()
where TRoleClaim : RamshaIdentityRoleClaim<TId>, new()
where TRegisterDto : RamshaRegisterDto, new()
{

    public async Task<IRamshaResult> RegisterAsync(TRegisterDto registerDto)
    {
        var user = MapCreate(registerDto);
        var createResult = await userManager.CreateAsync(user, registerDto.Password);
        if (!createResult.Succeeded)
        {
            return createResult.MapToRamshaError();
        }

        await userManager.AddToBaseRoles(user);

        return Success(user.Id.ToString());
    }


    protected virtual TUser MapCreate(TRegisterDto registerDto)
    {
        return new TUser()
        {
            Id = GenerateId(),
            UserName = registerDto.Username,
            Email = registerDto.Email
        };

    }

    protected virtual TId GenerateId()
    {
        object id = Guid.NewGuid();

        if (typeof(TId) == typeof(Guid))
            return (TId)id;
        if (typeof(TId) == typeof(string))
            return (TId)(object)id.ToString();

        return default;
    }

    public async Task<IRamshaResult> ResetPasswordAsync(string username, string token, string newPassword)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
       {
           var user = await userManager.FindByNameAsync(username);
           if (user is null)
               return NotFound(message: "no user found");

           var result = await userManager.ResetPasswordAsync(user, token, newPassword);
           if (!result.Succeeded) return result.MapToRamshaError();

           return Success();
       });
    }

    public async Task<IRamshaResult> ConfirmEmailAsync(string email, string token)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
       {
           var user = await userManager.FindByEmailAsync(email);
           if (user is null)
               return NotFound(message: "no user found");

           var result = await userManager.ConfirmEmailAsync(user, token);
           if (!result.Succeeded) return result.MapToRamshaError();

           return Success();
       });
    }


}
