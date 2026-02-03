using Microsoft.AspNetCore.Mvc;
using Ramsha;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Domain;
using SimpleAppDemo.Identity;

namespace SimpleAppDemo.Controllers
{
    public class TestController(IIdentityUserRepository<AppUser,int> repository):RamshaApiController
    {
        [HttpPost]
        public async Task<ActionResult<List<UserDto>>> GetPaged(PaginationParams paginationParams)
        {
           return RamshaResult( await repository.GetPagedAsync(
            q=>q.Where(x=> x.Id > 1),
            paginationParams,
            u => new UserDto(u.Id,u.UserName)));
        }
    }


    public record UserDto(int Id,string UserName);
}