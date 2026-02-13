

using Microsoft.AspNetCore.Mvc;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Translations.Contracts;

namespace Ramsha.Translations.Api
{
    public class LanguagesController(ILanguagesService service) : RamshaApiController
    {
        [HttpGet]
        public async Task<ActionResult<LanguageDto>> GetPaged([FromQuery] PaginationParams paginationParams)
        => RamshaResult(await service.GetPaged(paginationParams));
        [HttpPost]
        public async Task<ActionResult<LanguageDto?>> Create(string culture, string? displayName = null)
        => RamshaResult(await service.Create(culture, displayName));

        [HttpDelete("by-culture/{culture}")]
        public async Task<IActionResult> DeleteByCulture(string culture)
         => RamshaResult(await service.DeleteByCulture(culture));

        [HttpGet("by-culture/{culture}")]
        public async Task<ActionResult<LanguageDto?>> FindByCulture(string culture)
        => RamshaResult(await service.FindByCulture(culture));

        [HttpPost("activate-by-culture/{culture}")]
        public async Task<IActionResult> ActivateByCulture(string culture)
        => RamshaResult(await service.ActivateByCulture(culture));

        [HttpPost("deactivate-by-culture/{culture}")]
        public async Task<IActionResult> DeactivateByCulture(string culture)
        => RamshaResult(await service.DeactivateByCulture(culture));

        [HttpPut("default-by-culture/{culture}")]
        public async Task<IActionResult> SetDefaultByCulture(string culture)
         => RamshaResult(await service.SetDefaultByCulture(culture));


    }
}