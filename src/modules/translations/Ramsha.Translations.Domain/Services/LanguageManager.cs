
using Ramsha.Common.Domain;

namespace Ramsha.Translations.Domain
{
    public class LanguageManager(ILanguageRepository repository) : RamshaDomainManager
    {
        public async Task<IRamshaResult> SetDefaultByCulture(string culture)
        {
            var existDefaultLang = await repository.FindAsync(x => x.IsDefault);
            if (existDefaultLang is not null)
            {
                existDefaultLang.SetDefault(false);
            }

            var newDefaultLang = await repository.FindAsync(x => x.Culture == culture);
            if (newDefaultLang is null)
            {
                return NotFound(message: "no language found with this culture");
            }

            if (!newDefaultLang.IsActive)
            {
                return Invalid(message: "activate the language first to set to default");
            }

            newDefaultLang.SetDefault(true);

            return Success();
        }
        public async Task<IRamshaResult> ActivateByCulture(string culture)
        {
            var language = await repository.FindAsync(x => x.Culture == culture);
            if (language is null)
            {
                return NotFound(message: "no language found with this culture");
            }

            language.Activate();

            return Success();
        }
        public async Task<IRamshaResult> DeactivateByCulture(string culture)
        {
            var language = await repository.FindAsync(x => x.Culture == culture);
            if (language is null)
            {
                return NotFound(message: "no language found with this culture");
            }

            language.Deactivate();
            return Success();
        }

        public async Task<RamshaResult<Language?>> Create(string culture, string? displayName = null)
        {
            var language = await repository.FindAsync(x => x.Culture == culture);
            if (language is not null)
            {
                return Invalid(message: "there is already language with this culture");
            }

            language = Language.Create(culture, displayName);

            await repository.AddAsync(language);

            return language;
        }

        public async Task<IRamshaResult> DeleteByCulture(string culture)
        {
            var language = await repository.FindAsync(x => x.Culture == culture);
            if (language is null)
            {
                return NotFound(message: "no language found with this culture");
            }

            await repository.DeleteAsync(language);

            return Success();
        }
    }
}