using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.Common.Domain;
using Ramsha.Identity.Shared;

namespace Ramsha.Identity.Domain;

public class IdentityDomainModule : RamshaModule
{

    public override void Register(RegisterContext context)
    {
        base.Register(context);
        context
       .DependsOn<IdentitySharedModule>()
        .DependsOn<CommonDomainModule>();
    }

    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);

        context.PrepareOptions<RamshaIdentityOptions>(options => { });
        context.PrepareOptions<RamshaTypeReplacementOptions>(options =>
        {
            // options.ReplaceUser<RamshaIdentityUser>();
            // options.ReplaceRole<RamshaIdentityRole>();
        });
    }


    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        var options = context.Services.ExecutePreparedOptions<RamshaIdentityOptions>();
        var typesOptions = context.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();

        var entitiesTypes = typesOptions.GetRamshaIdentityEntitiesTypes();
        var keyType = typesOptions.GetIdentityIdOrBase();


        var addIdentityCoreMethod = typeof(IdentityServiceCollectionExtensions)
            .GetMethods()
            .First(m => m.Name == nameof(IdentityServiceCollectionExtensions.AddIdentityCore)
                     && m.IsGenericMethodDefinition
                     && m.GetParameters().Length == 2);

        var genericAddIdentityCore = addIdentityCoreMethod.MakeGenericMethod(entitiesTypes.UserType);

        var builder = (IdentityBuilder)genericAddIdentityCore.Invoke(
            null,
            new object[]
            {
                    context.Services,
                    options.IdentityOptionsAction ?? new Action<IdentityOptions>(opt =>
                    {
                        opt.Password.RequireDigit = false;
                        opt.Password.RequireNonAlphanumeric = false;
                        opt.Password.RequireUppercase = false;
                        opt.Password.RequiredLength = 4;
                    })
            })!;



        var addRolesGeneric = typeof(IdentityBuilder)
      .GetMethod(nameof(IdentityBuilder.AddRoles))
      ?.MakeGenericMethod(entitiesTypes.RoleType);

        addRolesGeneric?.Invoke(builder, null);


        var factoryType = typeof(RamshaUserClaimsPrincipalFactory<,,,,,,,>)
        .MakeGenericType(entitiesTypes.UserType, entitiesTypes.RoleType, keyType, entitiesTypes.UserRoleType, entitiesTypes.RoleClaimType, entitiesTypes.UserClaimType, entitiesTypes.UserLoginType, entitiesTypes.UserTokenType);
        var addClaimsFactoryGeneric = typeof(IdentityBuilder)
            .GetMethods()
            .First(m => m.Name == nameof(IdentityBuilder.AddClaimsPrincipalFactory)
                     && m.IsGenericMethodDefinition
                     && m.GetParameters().Length == 0)
            .MakeGenericMethod(factoryType);

        addClaimsFactoryGeneric.Invoke(builder, null);



        foreach (var configureAction in options.ConfigureIdentityActions)
        {
            configureAction(builder);
        }

        context.Services.AddRamshaIdentityDomainServices();

        context.Services.AddObjectAccessor(builder);
    }
}
