
using Ramsha.Identity.AspNetCore;

namespace Ramsha.Account.Web;

public class AccountWebModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<IdentityAspNetCoreModule>();
    }
}
