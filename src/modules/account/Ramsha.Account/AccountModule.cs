using Ramsha.Account.Api;
using Ramsha.Account.Application;
using Ramsha.Account.Web;

namespace Ramsha.Account;

public class AccountModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
        .DependsOn<AccountApplicationModule>()
        .DependsOn<AccountApiModule>()
        .DependsOn<AccountWebModule>();
    }
}
