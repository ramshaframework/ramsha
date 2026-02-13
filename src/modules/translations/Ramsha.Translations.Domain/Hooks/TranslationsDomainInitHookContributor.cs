

using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ramsha.Localization;

namespace Ramsha.Translations.Domain
{
    public class TranslationsDomainInitHookContributor(
        IOptions<RamshaLocalizationOptions> options) : IInitHookContributor
    {
        public async Task OnInitialize(InitContext context)
        {
            var languageRepository = context.ServiceProvider.GetRequiredService<ILanguageRepository>();

            var query = await languageRepository.GetQueryableAsync();
            if (!query.Any())
            {
                var allCultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures);

                var defaultCulture = options.Value.DefaultLanguage.Culture;

                var languages = allCultures
                .Where(c => !string.IsNullOrEmpty(c.Name))
                .Select(c =>
                {
                    var lang = Language.Create(c.Name, c.NativeName);
                    lang.SetDefault(defaultCulture == c.Name);
                    return lang;
                });
                await languageRepository.AddRangeAsync(languages);
            }
        }
    }
}