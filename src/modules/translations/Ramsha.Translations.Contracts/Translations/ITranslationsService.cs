

namespace Ramsha.Translations.Contracts
{
    public interface ITranslationsService
    {
        Task<RamshaResult<TranslationDto?>> SetAsync(string key, string value, string resourceName, string culture);
        Task<PagedResult<TranslationDto>> GetAll(PaginationParams paginationParams);
        Task<RamshaResult<IReadOnlyList<LocalizationResourceDto>>> GetAllResources();
    }
}