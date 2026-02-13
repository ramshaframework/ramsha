

using Ramsha.Localization;
using Ramsha.Translations.Contracts;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Application
{
    public static class MappingsExtensions
    {
        public static LanguageDto ToDto(this Language language)
        {
            return new LanguageDto(language.Id, language.Culture, language.DisplayName);
        }
        public static TranslationDto ToDto(this Translation translation)
        {
            return new TranslationDto(translation.Key, translation.Value, translation.ResourceName, translation.Culture);
        }

        public static LocalizationResourceDto ToDto(this ResourceDefinition resourceDefinition)
        {
            return new LocalizationResourceDto(resourceDefinition.Name, resourceDefinition.Stores.ToList());
        }
    }
}