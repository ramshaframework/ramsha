

using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization
{
    public interface ILocalizationLanguagesProvider
    {
        Task<IReadOnlyList<LanguageInfo>> GetSupportedLanguagesAsync();
        Task<LanguageInfo?> GetDefaultLanguage();
    }
}