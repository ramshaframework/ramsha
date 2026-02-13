

using Microsoft.AspNetCore.Mvc;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Translations.Contracts;

namespace Ramsha.Translations.Api
{
    public class TranslationsController(ITranslationsService service) : RamshaApiController
    {
        [HttpGet("resources")]
        public async Task<ActionResult<IReadOnlyList<LocalizationResourceDto>>> GetAllResources()
        => RamshaResult(await service.GetAllResources());


        [HttpGet]
        public async Task<ActionResult<TranslationDto>> GetAll([FromQuery] PaginationParams paginationParams)
            => RamshaResult(await service.GetAll(paginationParams));


        [HttpPut]
        public async Task<ActionResult<TranslationDto?>> SetAsync(string key, string value, string resourceName, string culture)
        => RamshaResult(await service.SetAsync(key, value, resourceName, culture));

    }
}