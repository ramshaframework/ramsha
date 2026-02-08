using Microsoft.AspNetCore.Builder;

namespace Ramsha.AspNetCore
{
    public class RamshaRequestLocalizationOptions
    {
        public List<Func<IServiceProvider, RequestLocalizationOptions, Task>> RequestLocalizationOptionConfigurators { get; }

        public RamshaRequestLocalizationOptions()
        {
            RequestLocalizationOptionConfigurators = new List<Func<IServiceProvider, RequestLocalizationOptions, Task>>();
        }
    }
}