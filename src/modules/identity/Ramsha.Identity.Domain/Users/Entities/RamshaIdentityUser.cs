using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ramsha.Common.Domain;

namespace Ramsha.Identity.Domain;

public class RamshaIdentityUser : RamshaIdentityUser<Guid>
{
    public RamshaIdentityUser()
    {

    }
    public RamshaIdentityUser(Guid id, string username) : base(id, username)
    {

    }

}

public class RamshaIdentityUser<TId>
 : RamshaIdentityUser<TId, RamshaIdentityUserClaim<TId>, RamshaIdentityUserRole<TId>, RamshaIdentityUserLogin<TId>, RamshaIdentityUserToken<TId>>
 where TId : IEquatable<TId>

{
    public RamshaIdentityUser()
    {

    }
    public RamshaIdentityUser(TId id, string username) : base(id, username)
    {

    }

}

public class RamshaIdentityUser<TId, TUserClaim, TUserRole, TUserLogin, TUserToken>
 : RamshaIdentityUserBase<TId>
 where TId : IEquatable<TId>
 where TUserClaim : RamshaIdentityUserClaim<TId>, new()
 where TUserRole : RamshaIdentityUserRole<TId>, new()
 where TUserLogin : RamshaIdentityUserLogin<TId>, new()
where TUserToken : RamshaIdentityUserToken<TId>, new()

{

    public RamshaIdentityUser() { }
    public RamshaIdentityUser(TId id, string userName) : this()
    {
        Id = id;
        UserName = userName;
    }

    public virtual ICollection<TUserRole> Roles { get; } = new List<TUserRole>();
    public virtual ICollection<TUserClaim> Claims { get; } = new List<TUserClaim>();
    public virtual ICollection<TUserLogin> Logins { get; } = new List<TUserLogin>();
    public virtual ICollection<TUserToken> Tokens { get; } = new List<TUserToken>();








    //
    public virtual void AddRole(TId roleId)
    {
        if (IsInRole(roleId))
        {
            return;
        }

        Roles.Add(new TUserRole() { UserId = Id, RoleId = roleId });
    }

    public virtual void RemoveRole(TId roleId)
    {
        if (!IsInRole(roleId))
        {
            return;
        }

        var role = Roles.FirstOrDefault(r => r.RoleId.Equals(roleId));
        if (role is not null)
        {
            Roles.Remove(role);
        }
    }

    public virtual bool IsInRole(TId roleId)
    {
        return Roles.Any(r => r.RoleId.Equals(roleId));
    }

    public virtual TUserClaim? FindClaim(Claim claim)
    {
        return Claims.FirstOrDefault(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
    }

    public virtual void ReplaceClaim(Claim claim, Claim newClaim)
    {
        var userClaims = Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
        foreach (var userClaim in userClaims)
        {
            userClaim.InitializeFromClaim(newClaim);
        }
    }

    public virtual void RemoveClaims(IEnumerable<Claim> claims)
    {
        foreach (var claim in claims)
        {
            RemoveClaim(claim);
        }
    }

    public virtual void RemoveClaim(Claim claim)
    {
        var claimToRemove = Claims.FirstOrDefault(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type);
        if (claimToRemove is not null)
        {
            Claims.Remove(claimToRemove);
        }
    }



    public virtual TUserLogin? FindLogin(string loginProvider, string providerKey)
    {
        return Logins.FirstOrDefault(userLogin =>
                userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey);
    }

    public virtual bool HasLogin(string loginProvider, string providerKey)
    => FindLogin(loginProvider, providerKey) is not null;

    public virtual void RemoveLogin(string loginProvider, string providerKey)
    {
        var login = FindLogin(loginProvider, providerKey);

        if (login is not null)
        {
            Logins.Remove(login);
        }
    }

    public virtual TUserToken? FindToken(string loginProvider, string name)
    {
        return Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);
    }

    public virtual bool HasClaim(Claim claim)
    {
        return FindClaim(claim) is not null;
    }
    public virtual bool HasToken(string loginProvider, string name)
    {
        return FindToken(loginProvider, name) is not null;
    }


    public virtual void RemoveToken(string loginProvider, string name)
    {
        var token = Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);

        if (token is not null)
        {
            Tokens.Remove(token);
        }
    }



    public virtual void AddClaim(Claim claim)
    {
        if (!HasClaim(claim))
        {
            var newClaim = new TUserClaim() { UserId = Id };
            newClaim.InitializeFromClaim(claim);
            Claims.Add(newClaim);
        }
    }
    public virtual void AddClaims(IEnumerable<Claim> claims)
    {
        foreach (var claim in claims)
        {
            AddClaim(claim);
        }
    }


    public virtual void AddLogin(string loginProvider, string providerKey, string? providerDisplayName = null)
    {
        if (!HasLogin(loginProvider, providerKey))
        {
            var newLogin = new TUserLogin() { UserId = Id };
            newLogin.InitializeFrom(loginProvider, providerKey, providerDisplayName);
            Logins.Add(newLogin);
        }
    }

    public virtual void AddToken(string loginProvider, string name, string? value)
    {
        if (!HasToken(loginProvider, name))
        {
            var token = new TUserToken() { UserId = Id };
            token.InitializeFrom(loginProvider, name, value);
            Tokens.Add(token);
        }
    }


    public override string ToString()
    {
        return UserName;
    }

}

public abstract class RamshaIdentityUserBase<TId> : AggregateRoot<TId>, IAudit, ISoftDelete
where TId : IEquatable<TId>
{
    public virtual string UserName { get; set; }
    public virtual string? Email { get; set; }
    public virtual bool EmailConfirmed { get; set; }
    public virtual string? NormalizedEmail { get; set; }
    public virtual string? NormalizedUserName { get; set; }
    public virtual string PasswordHash { get; set; }
    public virtual string? SecurityStamp { get; set; }
    public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    public virtual string? PhoneNumber { get; set; }
    public virtual bool PhoneNumberConfirmed { get; set; }
    public virtual bool TwoFactorEnabled { get; set; }
    public virtual DateTimeOffset? LockoutEnd { get; set; }
    public virtual bool LockoutEnabled { get; set; }
    public virtual int AccessFailedCount { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreationDate { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeletionDate { get; set; }
}


