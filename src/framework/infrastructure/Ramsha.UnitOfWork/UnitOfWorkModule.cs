using Microsoft.Extensions.DependencyInjection;
using Ramsha.UnitOfWork.Abstractions;

namespace Ramsha.UnitOfWork;

public class UnitOfWorkModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context.DependsOn<UnitOfWorkAbstractionsModule>();
    }


    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);
        context.Services.AddScoped<UnitOfWork>();
        context.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UnitOfWork>());

        context.Services.AddSingleton<ICurrentUnitOfWork, CurrentUnitOfWork>();
        context.Services.AddScoped<IUnitOfWorkManager, UnitOfWorkManager>();
        context.Services.AddSingleton<IUnitOfWorkOptions, UnitOfWorkOptions>();
        context.Configure<GlobalUnitOfWorkOptions>(options =>
        {
            options.IsEnabled = true;
        });

    }
}
