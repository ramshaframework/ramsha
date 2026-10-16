using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ramsha.Security.Claims;

public interface IPrincipalAccessor
{
    ClaimsPrincipal? Principal { get; }

    IDisposable Change(ClaimsPrincipal? principal);
}


public abstract class PrincipalAccessorBase : IPrincipalAccessor
{
    public ClaimsPrincipal? Principal => _currentPrincipal.Value ?? GetClaimsPrincipal();

    private readonly AsyncLocal<ClaimsPrincipal?> _currentPrincipal = new AsyncLocal<ClaimsPrincipal?>();

    protected abstract ClaimsPrincipal? GetClaimsPrincipal();

    public virtual IDisposable Change(ClaimsPrincipal? principal)
    {
        return SetCurrent(principal);
    }

    private IDisposable SetCurrent(ClaimsPrincipal? principal)
    {
        var parent = Principal;
        _currentPrincipal.Value = principal;

        return new OnDispose<ValueTuple<AsyncLocal<ClaimsPrincipal?>, ClaimsPrincipal?>>(static (state) =>
        {
            var (currentPrincipal, parent) = state;
            currentPrincipal.Value = parent;
        }, (_currentPrincipal, parent));
    }
}