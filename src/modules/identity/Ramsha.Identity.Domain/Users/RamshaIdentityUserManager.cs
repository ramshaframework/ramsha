
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ramsha.Core;
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Domain;

public class RamshaIdentityUserManager<TUser>(
  IUserStore<TUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<TUser> passwordHasher,
        IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<RamshaIdentityUserManager<TUser>> logger,

IIdentityRoleRepository<RamshaIdentityRole<Guid>, Guid> roleRepository
) : RamshaIdentityUserManager<TUser, Guid>(
store,
     optionsAccessor,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger,

roleRepository
)
where TUser : RamshaIdentityUser, new()
{

}

public class RamshaIdentityUserManager<TUser, TId>(
  IUserStore<TUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<TUser> passwordHasher,
        IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<RamshaIdentityUserManager<TUser, TId>> logger,

IIdentityRoleRepository<RamshaIdentityRole<TId>, TId> roleRepository
) : RamshaIdentityUserManager<TUser, RamshaIdentityRole<TId>, TId, RamshaIdentityUserRole<TId>, RamshaIdentityRoleClaim<TId>, RamshaIdentityUserClaim<TId>, RamshaIdentityUserLogin<TId>, RamshaIdentityUserToken<TId>>(
store,
     optionsAccessor,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger,

roleRepository
)
where TId : IEquatable<TId>
where TUser : RamshaIdentityUser<TId>, new()
{

}

public class RamshaIdentityUserManager<TUser, TRole, TId, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TUserToken>(
        IUserStore<TUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<TUser> passwordHasher,
        IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<RamshaIdentityUserManager<TUser, TRole, TId, TUserRole, TRoleClaim, TUserClaim, TUserLogin, TUserToken>> logger,
        IIdentityRoleRepository<TRole, TId> roleRepository) : UserManager<TUser>(store,
            optionsAccessor,
            passwordHasher,
            userValidators,
            passwordValidators,
            keyNormalizer,
            errors,
            services,
            logger)
     where TId : IEquatable<TId>
     where TUser : RamshaIdentityUser<TId, TUserClaim, TUserRole, TUserLogin, TUserToken>, new()
where TUserClaim : RamshaIdentityUserClaim<TId>, new()
where TUserRole : RamshaIdentityUserRole<TId>, new()
where TUserLogin : RamshaIdentityUserLogin<TId>, new()
where TUserToken : RamshaIdentityUserToken<TId>, new()
where TRole : RamshaIdentityRole<TId, TUserRole, TRoleClaim>, new()
where TRoleClaim : RamshaIdentityRoleClaim<TId>, new()

{

    public virtual async Task AddToBaseRoles(TUser user)
    {
        var roles = await roleRepository.GetListAsync(x => x.IsBase);
        foreach (var role in roles)
        {
            if (!user.IsInRole(role.Id))
            {
                user.AddRole(role.Id);
            }
        }
    }



    public virtual async Task<IdentityResult> CreateAsync(TUser user, string password, bool validatePassword)
    {
        var result = await UpdatePasswordHash(user, password, validatePassword);
        if (!result.Succeeded)
        {
            return result;
        }

        return await CreateAsync(user);
    }


    public async override Task<IdentityResult> UpdateAsync(TUser user)
    {
        var result = await base.UpdateAsync(user);
        if (result.Succeeded)
        {
            user.RaiseEvent(new IdentityUserCreatedDomainEvent<TUser>(user));
        }
        return result;
    }
    public async override Task<IdentityResult> DeleteAsync(TUser user)
    {
        user.Claims.Clear();
        user.Roles.Clear();
        user.Tokens.Clear();
        user.Logins.Clear();

        await UpdateAsync(user);

        return await base.DeleteAsync(user);
    }








    public virtual async Task<RamshaResult<TUser>> GetByIdAsync(TId id)
    {
        var user = await Store.FindByIdAsync(id.ToString(), CancellationToken);
        if (user == null)
        {
            return RamshaResults.NotFound(message: "no user found with this id");
        }

        return user;
    }

    public virtual async Task<IdentityResult> SetRolesAsync(TUser user,
        IEnumerable<string> roleNames)
    {

        var currentRoleNames = await GetRolesAsync(user);

        var result = await RemoveFromRolesAsync(user, currentRoleNames.Except(roleNames).Distinct());
        if (!result.Succeeded)
        {
            return result;
        }

        result = await AddToRolesAsync(user, roleNames.Except(currentRoleNames).Distinct());
        if (!result.Succeeded)
        {
            return result;
        }

        return IdentityResult.Success;
    }






    public async override Task<IdentityResult> SetEmailAsync(TUser user, string? email)
    {

        var result = await base.SetEmailAsync(user, email);

        if (result.Succeeded)
        {
            //will publish event later
        }
        return result;
    }

    public async override Task<IdentityResult> SetUserNameAsync(TUser user, string? userName)
    {
        var result = await base.SetUserNameAsync(user, userName);

        if (result.Succeeded)
        {
            //will publish event later
        }

        return result;
    }


    public virtual async Task<bool> ValidateUserNameAsync(string userName, Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(Options.User.AllowedUserNameCharacters) && userName.Any(c => !Options.User.AllowedUserNameCharacters.Contains(c)))
        {
            return false;
        }

        var owner = await FindByNameAsync(userName);
        if (owner != null && !owner.Id.Equals(userId))
        {
            return false;
        }

        return true;
    }

    public virtual Task<string> GetRandomUserNameAsync(int length)
    {
        var allowedUserNameCharacters = Options.User.AllowedUserNameCharacters;
        if (string.IsNullOrWhiteSpace(allowedUserNameCharacters))
        {
            allowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        }

        var randomUserName = string.Empty;
        var random = new Random();
        while (randomUserName.Length < length)
        {
            randomUserName += allowedUserNameCharacters[random.Next(0, allowedUserNameCharacters.Length)];
        }

        return Task.FromResult(randomUserName);
    }

    public virtual async Task<RamshaResult<string>> GetUserNameFromEmailAsync(string email)
    {
        const int maxTryCount = 20;
        var tryCount = 0;

        var userName = email.Split('@')[0];

        if (await ValidateUserNameAsync(userName))
        {
            // The username is valid.
            return userName;
        }

        if (string.IsNullOrWhiteSpace(Options.User.AllowedUserNameCharacters))
        {
            tryCount = 0;
            do
            {
                var randomUserName = userName + RandomHelper.GetRandom(1000, 9999);
                if (await ValidateUserNameAsync(randomUserName))
                {
                    return randomUserName;
                }
                tryCount++;
            } while (tryCount < maxTryCount);
        }
        else if (!userName.All(Options.User.AllowedUserNameCharacters.Contains))
        {
            // The username contains not allowed characters. So, we are generating a random username.
            do
            {
                var randomUserName = await GetRandomUserNameAsync(userName.Length);
                if (await ValidateUserNameAsync(randomUserName))
                {
                    return randomUserName;
                }
                tryCount++;
            } while (tryCount < maxTryCount);
        }
        else if (Options.User.AllowedUserNameCharacters.Where(char.IsDigit).Distinct().Count() >= 4)
        {
            // The AllowedUserNameCharacters includes 4 numbers. So, we are generating 4 random numbers and appending to the username.
            var numbers = Options.User.AllowedUserNameCharacters.Where(char.IsDigit).OrderBy(x => Guid.NewGuid()).Take(4).ToArray();
            var minArray = numbers.OrderBy(x => x).ToArray();
            if (minArray[0] == '0')
            {
                var secondItem = minArray[1];
                minArray[0] = secondItem;
                minArray[1] = '0';
            }
            var min = int.Parse(new string(minArray));
            var max = int.Parse(new string(numbers.OrderByDescending(x => x).ToArray()));
            tryCount = 0;
            do
            {
                var randomUserName = userName + RandomHelper.GetRandom(min, max);
                if (await ValidateUserNameAsync(randomUserName))
                {
                    return randomUserName;
                }
                tryCount++;
            } while (tryCount < maxTryCount);
        }
        else
        {
            tryCount = 0;
            do
            {
                // The AllowedUserNameCharacters does not include numbers. So, we are generating 4 random characters and appending to the username.
                var randomUserName = userName + await GetRandomUserNameAsync(4);
                if (await ValidateUserNameAsync(randomUserName))
                {
                    return randomUserName;
                }
                tryCount++;
            } while (tryCount < maxTryCount);
        }

        Logger.LogError($"Could not get a valid user name for the given email address: {email}, allowed characters: {Options.User.AllowedUserNameCharacters}, tried {maxTryCount} times.");
        return RamshaResults.Invalid(RamshaIdentityErrorsCodes.GenerateUsernameErrorCode, "Could not get a valid username for the given email address");
    }
}
