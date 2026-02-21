using Microsoft.Extensions.DependencyInjection;

namespace Ramsha.Files;

public class FilesModule : RamshaModule
{
    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        context.Services.AddTransient<IFileHandler, FileHandler>();
        context.Services.AddTransient<IFileStorage, LocalFileStorage>();
    }
}
