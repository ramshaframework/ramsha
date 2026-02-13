
using System.Collections.Immutable;
using Ramsha.Common.Application;
using Ramsha.Localization;
using Ramsha.Translations.Contracts;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Application
{
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