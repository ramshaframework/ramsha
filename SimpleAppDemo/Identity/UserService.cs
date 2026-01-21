

using Ramsha;
using Ramsha.Identity.Application;
using Ramsha.Identity.Contracts;
using Ramsha.Identity.Domain;

namespace SimpleAppDemo.Identity;

public class AppUserService(ILogger<AppUserService> logger, RamshaIdentityUserManager<AppUser, int> manager, IIdentityUserRepository<AppUser, int> repository)
    : RamshaIdentityUserService<AppUser, int>(manager, repository)
{
    public override Task<RamshaResult<List<RamshaIdentityUserDto>>> GetList()
    {
        logger.LogInformation("******************************jbfvjbfvjhbfhvbdjfvb*****************************");
        return base.GetList();
    }
}
