
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Shared;


namespace Ramsha.Account.Auth.WebAuth;

public static class MvcBuilderExtensions
{
    public static void AddWebAuthGenericControllers(this IMvcBuilder builder)
    {
        var typesOptions = builder.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();
        builder.AddGenericControllers(
            typeof(AccountController<,,>)
            .MakeGenericType(
                typesOptions.GetUserTypeOrBase(),
                 typesOptions.GetIdentityIdOrBase(),
                 typesOptions.GetOrSelf<RamshaLoginViewModel>())
            );
    }
}
