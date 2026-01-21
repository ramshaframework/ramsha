using DemoApp.Controllers;
using DemoApp.Data;
using DemoApp.Entities;
using DemoApp.Permissions;
using DemoApp.Settings;
using DemoModule;
using Ramsha;
using Ramsha.Account.Api;
using Ramsha.Account.Application;
using Ramsha.Account.Contracts;
using Ramsha.AspNetCore;
using Ramsha.Authorization;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore.SqlServer;
using Ramsha.Identity.Api;
using Ramsha.Identity.Application;
using Ramsha.Identity.AspNetCore;
using Ramsha.Identity.Contracts;
using Ramsha.Identity.Domain;
using Ramsha.Identity.Persistence;
using Ramsha.Identity.Shared;
using Ramsha.LocalMessaging.Abstractions;
using Ramsha.Permissions.Api;
using Ramsha.Permissions.Application;
using Ramsha.Permissions.Persistence;
using Ramsha.Security.Claims;
using Ramsha.Settings;
using Ramsha.SettingsManagement.Api;
using Ramsha.SettingsManagement.Application;
using Ramsha.SettingsManagement.Persistence;

namespace DemoApp;

public class AppModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context
        .DependsOn<IdentityApplicationModule>()
        .DependsOn<AccountApplicationModule>()
        .DependsOn<PermissionsApplicationModule>()
        .DependsOn<SettingsManagementApplicationModule>()
        .DependsOn<PermissionsPersistenceModule>()
        .DependsOn<IdentityPersistenceModule>()
        .DependsOn<SettingsManagementPersistenceModule>()
        .DependsOn<EntityFrameworkCoreSqlServerModule>()
        .DependsOn<IdentityAspNetCoreModule>()
        .DependsOn<IdentityApiModule>()
        .DependsOn<AccountApiModule>()
        .DependsOn<PermissionsApiModule>()
        .DependsOn<SettingsManagementApiModule>();


    }
    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);
        context.PrepareOptions<LocalMessagingOptions>(options =>
       {
           options.AddMessagesFromAssembly<AppModule>();
       });

        context.PrepareOptions<RamshaTypeReplacementOptions>(options =>
        {
            options.ReplaceIdentityEntities<AppIdentityUser>();
            options.ReplaceBase<CreateRamshaIdentityUserDto, CreateAppUserDto>();
            options.ReplaceBase<UpdateRamshaIdentityUserDto, UpdateAppUserDto>();
            options.ReplaceIdentityUserService<AppUserService>();
            options.ReplaceBase<RamshaRegisterDto, AppRegisterDto>();
            options.ReplaceAccountService<AppAccountService>();
        });
    }

    public override void BuildServices(BuildServicesContext context)
    {
        var configuration = context.Services.GetConfiguration();
        base.BuildServices(context);


        context.Services.AddTransient<ITestService, TestService>();


        context.Services.AddRamshaDbContext<AppDbContext>(option =>
        {
            option
             .AddDefaultRepositories(true)
            .AddGlobalQueryFilterProvider<PriceFilterProvider>()
             .AddRepository<Product, IProductRepository, ProductRepository>()
             //  .ReplaceDbContext<IIdentityDbContext>()
             ;
        });

        context.Services.Configure<ConnectionStringsOptions>(options =>
        {
            options.ConfigureAliases(builder =>
            {
                builder.Map("Default", ["MainDb", "Primary"]);
            });
        });


        context.Services.Configure<TestSetting>(configuration.GetSection(nameof(TestSetting)));

        context.Services.Configure<RamshaClaimsPrincipalFactoryOptions>(options =>
        {
            options.Transformers.Add<DemoClaimsTransformer>();
            options.Transformers.AddBefore<DemoClaimsTransformer, DemoClaimsTransformerBefore>();
            options.Transformers.AddAfter<DemoClaimsTransformer, DemoClaimsTransformerAfter>();

        });

        context.Services.Configure<RamshaPermissionOptions>(options =>
        {
            options.DefinitionProviders.Add<ProductPermissionsDefinition>();
        });

        context.Services.Configure<RamshaSettingsOptions>(options =>
        {
            options.DefinitionProviders.Add<ProductSettingDefinitions>();
        });


        context.Services.AddRamshaDomainManager<FakeManager>();
        context.Services.AddRamshaService<FakeAppService>();



    }


}


