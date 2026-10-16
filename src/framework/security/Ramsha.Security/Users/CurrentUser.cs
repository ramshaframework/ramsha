using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Ramsha.Security.Claims;

namespace Ramsha.Security.Users;

public class CurrentUser(IPrincipalAccessor accessor) : ICurrentUser
{
    public string? Id => accessor.Principal.FindUserId();
    public string? Username => accessor.Principal.FindUsername();
    public string? Email => accessor.Principal.FindEmail();
    public string[] GetRoles() => accessor.Principal.GetRoles();
    public bool IsAuthenticated() => Id is not null;
    public bool IsInRole(string roleName)
    => accessor.Principal.HasRole(roleName);
    public Claim? FindClaim(string claimType)
        => accessor.Principal?.FindFirst(claimType);
    public Claim[] GetClaims(string claimType)
    => accessor.Principal.FindClaims(claimType);
    public Claim[] GetAllClaims()
    => [.. accessor.Principal?.Claims ?? []];

}
