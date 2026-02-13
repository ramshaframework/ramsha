

using Ramsha.Localization;
using Ramsha.Localization.Abstractions;

namespace Ramsha.Translations.Domain
{
    public class TranslationsLanguagesProvider(ILanguageRepository languageRepository) : ILocalizationLanguagesProvider
    {
        public async Task<LanguageInfo?> GetDefaultLanguage()
        {
            var language = await languageRepository.FindAsync(x => x.IsDefault && x.IsActive);
            return language is not null ? new LanguageInfo(language.Culture, language.DisplayName) : null;
        }

        public async Task<IReadOnlyList<LanguageInfo>> GetSupportedLanguagesAsync()
        {
            var languages = await languageRepository
            .GetListAsync(x => x.IsActive);

            return languages
            .Select(lang => new LanguageInfo(lang.Culture, lang.DisplayName))
            .ToList();
        }
    }
}