
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

namespace Ramsha.AspNetCore.Mvc;

public class AspNetCoreMvcModule : RamshaModule
{
    public override void Register(RegisterContext context)
    {
        base.Register(context);

        context.DependsOn<AspNetCoreModule>();

    }

    public override void Prepare(PrepareContext context)
    {
        base.Prepare(context);
        context.PrepareOptions<IMvcBuilder>(builder =>
        {
            builder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        });
    }


    public override void BuildServices(BuildServicesContext context)
    {
        base.BuildServices(context);

        var mvcBuilder = context.Services.AddMvc();

        context.Services.ExecutePreparedOptions(mvcBuilder);

        mvcBuilder.AddControllersAsServices();
        context.Services.AddOpenApi();

        context.Services.AddSingleton<IOptionsSnapshot<ScalarOptions>>(sp =>
       {
           ScalarOptions options = new();
           return new StaticOptions<ScalarOptions>(options);
       });

        context.Services.Configure<RamshaEndpointRouterOptions>(options =>
         {
             options.EndpointConfigureActions.Add(endpointContext =>
             {
                 endpointContext.Endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
                 endpointContext.Endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
                 endpointContext.Endpoints.MapRazorPages().WithStaticAssets();
                 endpointContext.Endpoints.MapOpenApi("/openapi/{documentName}.json");
                 endpointContext.Endpoints.MapScalarApiReference(options =>
                 {
                     options.Title = "Weather Forecast API Sample";
                     options.Theme = ScalarTheme.Default;
                     options.Favicon = "/favicon.svg";
                     options.Layout = ScalarLayout.Modern;
                     options.DarkMode = true;
                     options.CustomCss = "* { font-family: 'Monaco'; }";
                     options.OpenApiRoutePattern = "/openapi/{documentName}.json";
                 });
             });
         });


    }

}
