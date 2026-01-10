using Ramsha;
using Ramsha.EntityFrameworkCore.SqlServer;
using Ramsha.Identity;

public class AppModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context.DependsOn<IdentityModule>();
        context.DependsOn<EntityFrameworkCoreSqlServerModule>();
    }

    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services.AddRamshaDbContext<AppDbContext>();
    }
}
