using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ramsha.Common.Domain;

namespace Ramsha.Identity.Domain;

public class RamshaIdentityUserStore<TUser, TRole, TId, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TUserToken>(
    IIdentityUserRepository<TUser, TId> userRepository,
    IIdentityRoleRepository<TRole, TId> roleRepository) :
    IUserStore<TUser>,
    IUserLoginStore<TUser>,
    IUserRoleStore<TUser>,
    IUserClaimStore<TUser>,
    IUserPasswordStore<TUser>,
    IUserSecurityStampStore<TUser>,
    IUserEmailStore<TUser>,
    IUserLockoutStore<TUser>,
    IUserPhoneNumberStore<TUser>,
    IQueryableUserStore<TUser>,
    IUserTwoFactorStore<TUser>,
    IUserAuthenticationTokenStore<TUser>
     where TId : IEquatable<TId>
     where TUser : RamshaIdentityUser<TId, TUserClaim, TUserRole, TUserLogin, TUserToken>, new()
where TUserClaim : RamshaIdentityUserClaim<TId>, new()
where TUserRole : RamshaIdentityUserRole<TId>, new()
where TUserLogin : RamshaIdentityUserLogin<TId>, new()
where TUserToken : RamshaIdentityUserToken<TId>, new()
where TRole : RamshaIdentityRole<TId, TUserRole, TRoleClaim>, new()
where TRoleClaim : RamshaIdentityRoleClaim<TId>, new()
{
    public IQueryable<TUser> Users 
    => AsyncHelper.RunSync(userRepository.GetQueryableAsync);

    private void ThrowIfUserNull(TUser user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
    }
    public async Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfUserNull(user);

        user.AddClaims(claims);
    }

    public async Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ThrowIfUserNull(user);

        user.AddLogin(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName);
    }

    public async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ThrowIfUserNull(user);

        var role = await roleRepository.FindAsync(x => x.Name == roleName);
        user.AddRole(role.Id);
    }

    public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ThrowIfUserNull(user);


        try
        {
            var result = await userRepository.AddAsync(user);

            if (!result.Equals(default(TId)))
            {
                return IdentityResult.Success;
            }
            else
            {
                return IdentityResult.Failed();
            }
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError[]
            {
                    new IdentityError{ Description = ex.Message }
            });
        }
    }

    public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ThrowIfUserNull(user);


        try
        {
            var result = await userRepository.DeleteAsync(user.Id);

            return result ? IdentityResult.Success : IdentityResult.Failed();
        }
        catch (Exception ex)
        {
            return IdentityResult.Failed(new IdentityError[]
            {
                    new IdentityError{ Description = ex.Message }
            });
        }
    }

    public void Dispose()
    {

    }

    public async Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(normalizedEmail))
            throw new ArgumentNullException(nameof(normalizedEmail));

        try
        {
            var result = await userRepository.FindAsync(x => x.NormalizedEmail == normalizedEmail,
            [r => r.Claims, r => r.Roles, r => r.Tokens, r => r.Logins]);

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }

    }

    public async Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId));

        try
        {
            var id = EntityHelper.ConvertId<TId>(userId);
            return await userRepository.FindAsync(id, [r => r.Claims, r => r.Roles, r => r.Tokens, r => r.Logins]);
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<TUser?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var result = await userRepository
            .FindAsync(
              x => x.Logins.Any(x => x.LoginProvider == loginProvider && x.ProviderKey == providerKey)
            , [r => r.Claims, r => r.Roles, r => r.Tokens, r => r.Logins]);

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<TUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(normalizedUserName))
            throw new ArgumentNullException(nameof(normalizedUserName));

        try
        {
            var result = await userRepository.FindAsync(x => x.NormalizedUserName == normalizedUserName
            , [r => r.Claims, r => r.Roles, r => r.Tokens, r => r.Logins]);

            return result;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.AccessFailedCount);
    }

    public async Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return user.Claims.Select(x => x.ToClaim()).ToList();
    }

    public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.EmailConfirmed);
    }

    public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.LockoutEnabled);
    }

    public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.LockoutEnd);
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return user.Logins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
    }

    public Task<string?> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.NormalizedEmail);
    }

    public Task<string?> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.NormalizedUserName);
    }

    public Task<string?> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.PasswordHash);
    }

    public Task<string?> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    /// <summary>
    /// //
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>

    public async Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var roleIds = user.Roles.Select(r => r.RoleId).ToList();

        if (roleIds.Count == 0)
            return [];

        var roles = await roleRepository.GetListAsync(r => roleIds.Contains(r.Id));

        return roles.Select(r => r.Name).ToList();

    }

    public Task<string?> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.SecurityStamp);
    }

    public Task<string?> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.TwoFactorEnabled);
    }

    public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.Id.ToString());
    }

    public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return Task.FromResult(user.UserName);
    }

    public async Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (claim == null)
            throw new ArgumentNullException(nameof(claim));

        return [];
    }

    public async Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrEmpty(roleName))
            throw new ArgumentNullException(nameof(roleName));


        var role = await roleRepository.FindAsync(x => x.Name == roleName, [r => r.Users]);

        var userIds = role?.Users.Select(x => x.UserId) ?? [];
        return await userRepository.GetListAsync(x => userIds.Contains(x.Id));

    }

    public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(user.PasswordHash != null);
    }

    public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.AccessFailedCount++;
        return Task.FromResult(user.AccessFailedCount);
    }

    public async Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrEmpty(roleName))
            throw new ArgumentNullException(nameof(roleName));

        var role = await roleRepository.FindAsync(x => x.Name == roleName);

        return user.IsInRole(role.Id);

    }

    public async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (claims == null)
            throw new ArgumentNullException(nameof(claims));

        user.RemoveClaims(claims);
    }

    public async Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrEmpty(roleName))
            throw new ArgumentNullException(nameof(roleName));

        var role = await roleRepository.FindAsync(x => x.Name == roleName);


        user.RemoveRole(role.Id);
    }

    public async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (string.IsNullOrEmpty(loginProvider))
            throw new ArgumentNullException(nameof(loginProvider));

        if (string.IsNullOrEmpty(providerKey))
            throw new ArgumentNullException(nameof(providerKey));

        user.RemoveLogin(loginProvider, providerKey);
    }

    public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }

    public async Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (claim == null)
            throw new ArgumentNullException(nameof(claim));

        if (newClaim == null)
            throw new ArgumentNullException(nameof(newClaim));

        user.ReplaceClaim(claim, newClaim);
    }

    public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.AccessFailedCount = 0;

        return Task.FromResult(0);
    }

    public Task SetEmailAsync(TUser user, string? email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.Email = email;

        return Task.FromResult(0);
    }

    public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.EmailConfirmed = confirmed;

        return Task.FromResult(0);
    }

    public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.LockoutEnabled = enabled;

        return Task.FromResult(0);
    }

    public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.LockoutEnd = lockoutEnd;

        return Task.FromResult(0);
    }

    public Task SetNormalizedEmailAsync(TUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.NormalizedEmail = normalizedEmail;

        return Task.FromResult(0);
    }

    public Task SetNormalizedUserNameAsync(TUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.NormalizedUserName = normalizedName;

        return Task.CompletedTask;
    }

    public Task SetPasswordHashAsync(TUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.PasswordHash = passwordHash;

        return Task.FromResult(0);
    }

    public Task SetPhoneNumberAsync(TUser user, string? phoneNumber, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.PhoneNumber = phoneNumber;

        return Task.FromResult(0);
    }

    public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.PhoneNumberConfirmed = confirmed;

        return Task.FromResult(0);
    }

    public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.SecurityStamp = stamp;

        return Task.FromResult(0);
    }

    public Task SetTokenAsync(TUser user, string loginProvider, string name, string? value, CancellationToken cancellationToken)
    {
        return Task.FromResult(0);
    }

    public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.TwoFactorEnabled = enabled;

        return Task.FromResult(0);
    }

    public Task SetUserNameAsync(TUser user, string? userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        user.UserName = userName;

        return Task.FromResult(0);
    }

    public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return IdentityResult.Success;
    }

}



