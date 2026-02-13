
using System.Collections.Immutable;
using Ramsha.Common.Application;
using Ramsha.Localization;
using Ramsha.Translations.Contracts;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Application
{

    public class LanguagesService(
        ILanguageRepository repository,
         LanguageManager manager) : RamshaService, ILanguagesService
    {
        public Task<PagedResult<LanguageDto>> GetPaged(PaginationParams paginationParams)
        => repository.GetPagedAsync(paginationParams, l => l.ToDto());

        public async Task<RamshaResult<LanguageDto?>> FindByCulture(string culture)
        {
            var language = await repository.FindAsync(x => x.Culture == culture);
            if (language is null)
            {
                return NotFound(message: "no language found for this culture");
            }

            return language.ToDto();
        }

        public async Task<RamshaResult<LanguageDto?>> Create(string culture, string? displayName = null)
        {
            var result = await manager.Create(culture, displayName);
            if (!result.Succeeded)
            {
                return result.Error;
            }

            return result.Value?.ToDto();
        }

        public Task<IRamshaResult> DeleteByCulture(string culture)
        => manager.DeleteByCulture(culture);

        public Task<IRamshaResult> DeactivateByCulture(string culture)
        => manager.DeactivateByCulture(culture);

        public Task<IRamshaResult> ActivateByCulture(string culture)
        => manager.ActivateByCulture(culture);

        public Task<IRamshaResult> SetDefaultByCulture(string culture)
        => manager.SetDefaultByCulture(culture);
    }
    public class TranslationsService(
     TranslationsManager manager,
    ITranslationRepository repository,
    IResourcesDefinitionsProvider resourcesDefinitionsProvider) : RamshaService, ITranslationsService
    {
        public async Task<PagedResult<TranslationDto>> GetAll(PaginationParams paginationParams)
        {
            return await repository.GetPagedAsync(paginationParams, t => t.ToDto());
        }

        public async Task<RamshaResult<IReadOnlyList<LocalizationResourceDto>>> GetAllResources()
        {
            var resources = await resourcesDefinitionsProvider.GetAllResourcesAsync();
            return resources.Select(x => x.ToDto()).ToList();
        }

        public async Task<RamshaResult<TranslationDto?>> SetAsync(string key, string value, string resourceName, string culture)
        {
            var domainResult = await manager.SetAsync(key, value, resourceName, culture);
            if (!domainResult.Succeeded)
            {
                return domainResult.Error!;
            }

            return domainResult.Value?.ToDto();
        }
    }
}