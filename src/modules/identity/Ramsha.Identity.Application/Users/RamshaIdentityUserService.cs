using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ramsha.Core;
using Ramsha.Identity.Contracts;
using Ramsha.Identity.Shared;
using Ramsha.Identity.Domain;
using Ramsha.Common.Application;

namespace Ramsha.Identity.Application;

public class RamshaIdentityUserService<TUser, TDto, TCreateDto, TUpdateDto>(RamshaIdentityUserManager<TUser> userManager, IIdentityUserRepository<TUser, Guid> repository)
: RamshaIdentityUserService<TUser, RamshaIdentityRole, Guid, RamshaIdentityUserRole<Guid>, RamshaIdentityRoleClaim<Guid>, RamshaIdentityUserClaim<Guid>, RamshaIdentityUserLogin<Guid>, RamshaIdentityUserToken<Guid>, TDto, TCreateDto, TUpdateDto>(userManager, repository)
where TUser : RamshaIdentityUser, new()
where TCreateDto : CreateRamshaIdentityUserDto, new()
where TUpdateDto : UpdateRamshaIdentityUserDto, new()
where TDto : RamshaIdentityUserDto, new()
{

}


public class RamshaIdentityUserService<TUser, TRole, TId, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TUserToken, TDto, TCreateDto, TUpdateDto>(RamshaIdentityUserManager<TUser, TRole, TId, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TUserToken> userManager, IIdentityUserRepository<TUser, TId> repository) : RamshaService, IRamshaIdentityUserService<TDto, TCreateDto, TUpdateDto, TId>
where TId : IEquatable<TId>
where TUser : RamshaIdentityUser<TId, TUserClaim, TUserRole, TUserLogin, TUserToken>, new()
where TUserClaim : RamshaIdentityUserClaim<TId>, new()
where TUserRole : RamshaIdentityUserRole<TId>, new()
where TUserLogin : RamshaIdentityUserLogin<TId>, new()
where TUserToken : RamshaIdentityUserToken<TId>, new()
where TRoleClaim : RamshaIdentityRoleClaim<TId>, new()
where TRole : RamshaIdentityRole<TId, TUserRole, TRoleClaim>, new()
where TDto : RamshaIdentityUserDto, new()
where TCreateDto : CreateRamshaIdentityUserDto, new()
where TUpdateDto : UpdateRamshaIdentityUserDto, new()

{

    public virtual async Task<RamshaResult<string>> Create(TCreateDto createDto)
    {
        return await UnitOfWork<RamshaResult<string>>(async () =>
        {
            var user = MapCreate(createDto);
            var identityResult = await userManager.CreateAsync(user, createDto.Password);
            if (!identityResult.Succeeded)
            {
                return identityResult.MapToRamshaError();
            }

            if (createDto.Roles is not null)
            {
                var roleResult = await userManager.AddToRolesAsync(user, createDto.Roles);
                if (!roleResult.Succeeded)
                {
                    return roleResult.MapToRamshaError();
                }
            }

            await userManager.AddToBaseRoles(user);

            return user.Id.ToString()!;
        });

    }

    protected virtual TUser MapCreate(TCreateDto createDto)
    {
        return new TUser()
        {
            Id = GenerateId(),
            UserName = createDto.Username,
            Email = createDto.Email,
            PhoneNumber = createDto.PhoneNumber
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
    protected virtual TUser MapUpdate(TUser user, TUpdateDto updateDto)
    {
        user.PhoneNumber = updateDto.PhoneNumber;
        return user;
    }
    protected virtual TDto ToDto(TUser user)
    {
        return new TDto()
        {
            Id = user.Id.ToString(),
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed
        };
    }

    public virtual async Task<IRamshaResult> Delete(TId id)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
        {
            var getUser = await userManager.GetByIdAsync(id);
            if (!getUser.Succeeded)
            {
                return getUser.Error!;
            }
            var result = await userManager.DeleteAsync(getUser.Value);
            if (!result.Succeeded)
            {
                return result.MapToRamshaError();
            }

            return Success();
        });

    }

    public virtual async Task<IRamshaResult> Update(TId id, TUpdateDto updateDto)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
        {
            var getUser = await userManager.GetByIdAsync(id);
            if (!getUser.Succeeded)
            {
                return getUser.Error!;
            }
            var user = getUser.Value;
            user = MapUpdate(user, updateDto);
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return result.MapToRamshaError();
            }

            return Success();
        });
    }

    public async Task<RamshaResult<TDto>> Get(TId id)
    {
        var user = await repository.FindAsync(id);
        if (user is null)
        {
            return NotFound(message: "no user with this id");
        }

        return ToDto(user);
    }

    public async Task<RamshaResult<List<TDto>>> GetList(TId id)
    {
        var users = await repository.GetListAsync();
        return users.Select(u => ToDto(u)).ToList();
    }



    //role management
    public async Task<IRamshaResult> AddToRoleAsync(TId id, string roleName)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
        {
            var getUser = await userManager.GetByIdAsync(id);
            if (!getUser.Succeeded) return getUser.Error!;

            var result = await userManager.AddToRoleAsync(getUser.Value, roleName);
            if (!result.Succeeded)
                return result.MapToRamshaError();

            return Success();
        });
    }

    public async Task<IRamshaResult> RemoveFromRoleAsync(TId id, string roleName)
    {
        var getUser = await userManager.GetByIdAsync(id);
        if (!getUser.Succeeded) return getUser.Error!;

        var result = await userManager.RemoveFromRoleAsync(getUser.Value, roleName);
        if (!result.Succeeded) return result.MapToRamshaError();

        return Success();
    }

    public async Task<IRamshaResult> SetRolesAsync(TId id, IEnumerable<string> roleNames)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
        {
            var getUser = await userManager.GetByIdAsync(id);
            if (!getUser.Succeeded) return getUser.Error!;

            var result = await userManager.SetRolesAsync(getUser.Value, roleNames);
            if (!result.Succeeded) return result.MapToRamshaError();

            return Success();
        });

    }

    public async Task<RamshaResult<List<string>>> GetRolesAsync(TId id)
    {
        return await UnitOfWork<RamshaResult<List<string>>>(async () =>
       {
           var getUser = await userManager.GetByIdAsync(id);
           if (!getUser.Succeeded) return getUser.Error!;

           var roles = await userManager.GetRolesAsync(getUser.Value);
           return roles.ToList();
       });
    }


    //password management
    public async Task<IRamshaResult> ChangePasswordAsync(TId id, string oldPassword, string newPassword)
    {

        var getUser = await userManager.GetByIdAsync(id);
        if (!getUser.Succeeded) return getUser.Error!;

        var result = await userManager.ChangePasswordAsync(getUser.Value, oldPassword, newPassword);
        if (!result.Succeeded) return result.MapToRamshaError();

        return Success();
    }



    public async Task<IRamshaResult> SetPasswordAsync(TId id, string newPassword)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
       {
           var getUser = await userManager.GetByIdAsync(id);
           if (!getUser.Succeeded) return getUser.Error!;

           var result = await userManager.RemovePasswordAsync(getUser.Value);
           if (!result.Succeeded) return result.MapToRamshaError();

           result = await userManager.AddPasswordAsync(getUser.Value, newPassword);
           if (!result.Succeeded) return result.MapToRamshaError();

           return Success();
       });

    }



    //email and username
    public async Task<IRamshaResult> SetEmailAsync(TId id, string email)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
       {
           var getUser = await userManager.GetByIdAsync(id);
           if (!getUser.Succeeded) return getUser.Error!;

           var result = await userManager.SetEmailAsync(getUser.Value, email);
           if (!result.Succeeded) return result.MapToRamshaError();

           return Success();
       });
    }



    public async Task<IRamshaResult> SetUserNameAsync(TId id, string username)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
       {
           var getUser = await userManager.GetByIdAsync(id);
           if (!getUser.Succeeded) return getUser.Error!;

           var result = await userManager.SetUserNameAsync(getUser.Value, username);
           if (!result.Succeeded) return result.MapToRamshaError();

           return Success();
       });
    }


    //lockout
    public async Task<IRamshaResult> SetLockoutAsync(TId id, DateTimeOffset? end)
    {
        return await UnitOfWork<IRamshaResult>(async () =>
       {
           var getUser = await userManager.GetByIdAsync(id);
           if (!getUser.Succeeded) return getUser.Error!;

           var result = await userManager.SetLockoutEndDateAsync(getUser.Value, end);
           if (!result.Succeeded) return result.MapToRamshaError();

           return Success();
       });
    }

    public Task<IRamshaResult> UnlockAsync(TId id)
        => SetLockoutAsync(id, null);

}

