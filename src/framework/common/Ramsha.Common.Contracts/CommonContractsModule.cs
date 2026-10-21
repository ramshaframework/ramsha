using Ramsha.Authorization;
using Ramsha.LocalMessaging.Abstractions;

namespace Ramsha.Common.Contracts;

public class CommonContractsModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<AuthorizationModule>()
        .DependsOn<LocalMessagingAbstractionsModule>();
    }
}
