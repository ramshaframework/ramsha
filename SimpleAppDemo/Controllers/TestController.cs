using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Ramsha;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Domain;
using SimpleAppDemo.Identity;
using SimpleAppDemo.Resources;

namespace SimpleAppDemo.Controllers
{
    public class TestController(IStringLocalizer<AdditionalResource> stringLocalizer, IIdentityUserRepository<AppUser, int> repository) : RamshaApiController
    {

        [HttpGet]
        public async Task<ActionResult> GetLocalized()
        {
            return Ok(stringLocalizer["hello"]);
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