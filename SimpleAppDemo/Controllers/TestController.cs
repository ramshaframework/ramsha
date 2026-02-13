using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Ramsha;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Domain;
using Ramsha.Localization;
using SimpleAppDemo.Identity;
using SimpleAppDemo.Resources;

namespace SimpleAppDemo.Controllers
{
    public class TestController(ILocalizationLanguagesProvider languagesProvider, IStringLocalizer<AdditionalResource> addStringLocalizer, IStringLocalizer<AppResource> appStringLocalizer, IIdentityUserRepository<AppUser, int> repository) : RamshaApiController
    {
        [HttpGet("defaultLanguage")]
        public async Task<ActionResult> DefaultLanguage()
        {
            return Ok(await languagesProvider.GetDefaultLanguage());
        }

        [HttpGet("supported-languages")]
        public async Task<ActionResult> GetSupportedLanguages()
        {
            return Ok(await languagesProvider.GetSupportedLanguagesAsync());
        }

        [HttpGet("localize-app")]
        public async Task<ActionResult> Localize(string key)
        {
            return Ok(appStringLocalizer[key]);
        }

        [HttpGet("localize-add")]
        public async Task<ActionResult> LocalizeAdd(string key)
        {
            return Ok(addStringLocalizer[key]);
        }


        [HttpPost]
        public async Task<ActionResult<List<UserDto>>> GetPaged(PaginationParams paginationParams)
        {
            return RamshaResult(await repository.GetPagedAsync(
             q => q.Where(x => x.Id > 1),
             paginationParams,
             u => new UserDto(u.Id, u.UserName)));
        }
    }


    public record UserDto(int Id, string UserName);
}