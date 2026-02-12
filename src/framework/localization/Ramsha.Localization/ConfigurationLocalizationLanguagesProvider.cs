using System.Collections.Immutable;
using Microsoft.Extensions.Options;
using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization
{
    public class ConfigurationLocalizationResourcesProvider(IOptions<RamshaLocalizationOptions> options) : IResourcesDefinitionsProvider
    {
        public Task<IReadOnlyList<ResourceDefinition>> GetAllResourcesAsync()
        {
            return Task.FromResult<IReadOnlyList<ResourceDefinition>>(options.Value.Resources.Values.ToImmutableArray());
        }
    }

    public class ConfigurationLocalizationLanguagesProvider(IOptions<RamshaLocalizationOptions> options) : ILocalizationLanguagesProvider
    {
        public Task<IReadOnlyList<LanguageInfo>> GetSupportedLanguagesAsync()
        {
            return Task.FromResult<IReadOnlyList<LanguageInfo>>(options.Value.SupportedLanguages);
        }
    }
}