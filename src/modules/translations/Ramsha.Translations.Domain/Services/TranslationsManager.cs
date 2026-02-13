

using Ramsha.Common.Domain;
using Ramsha.Localization;

namespace Ramsha.Translations.Domain
{
    public class TranslationsManager(
        ITranslationRepository repository,
        ILanguageRepository languageRepository,
        IResourcesDefinitionsProvider resourcesDefinitionsProvider) : RamshaDomainManager
    {
        public async Task<RamshaResult<Translation>> SetAsync(string key, string value, string resourceName, string culture)
        {
            var resources = await resourcesDefinitionsProvider.GetAllResourcesAsync();
            if (!resources.Any(x => x.Name == resourceName))
            {
                return Invalid(message: "invalid resourceName");
            }

            var language = await languageRepository.FindAsync(x => x.Culture == culture);
            if (language is null)
            {
                return Invalid(message: "invalid culture");
            }

            var text = await repository.FindAsync(x =>
                        x.ResourceName == resourceName
                        && x.Culture == culture
                        && x.Key == key);

            if (text is null)
            {
                text = new Translation(
                     key,
                     resourceName,
                     culture
                       );

                await repository.AddAsync(text);
            }

            text.SetValue(value);
            return text;
        }
    }
}