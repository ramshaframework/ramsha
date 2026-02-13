

namespace Ramsha.Translations.Contracts
{
    public interface ILanguagesService
    {
        Task<PagedResult<LanguageDto>> GetPaged(PaginationParams paginationParams);
        Task<RamshaResult<LanguageDto?>> FindByCulture(string culture);
        Task<RamshaResult<LanguageDto?>> Create(string culture, string? displayName = null);
        Task<IRamshaResult> DeleteByCulture(string culture);
        Task<IRamshaResult> DeactivateByCulture(string culture);
        Task<IRamshaResult> ActivateByCulture(string culture);
        Task<IRamshaResult> SetDefaultByCulture(string culture);


    }
}