
using Microsoft.AspNetCore.Builder;
using Ramsha.UnitOfWork.Abstractions;

namespace Ramsha.AspNetCore;

public class AspNetCorePipelineContributor : IRamshaPipelineContributor
{
    public void Configure(RamshaPipelineContext context)
    {
        var aspNetCoreOptions = context.GetOptions<AspNetCoreOptions>().Value;

        var entryOptions = new RamshaPipelineEntryOptions
        {
            CanMove = false,
            CanRemove = false
        };

        context.Pipeline.Use(AspNetCorePipelineEntries.ExceptionHandler, app =>
        {
            app.UseExceptionHandler();
        }, entryOptions, () => aspNetCoreOptions.ExceptionHandler);

        context.Pipeline.Use(AspNetCorePipelineEntries.HttpsRedirection, app =>
        {
            app.UseHttpsRedirection();
        }, entryOptions, () => aspNetCoreOptions.HttpsRedirection);

        context.Pipeline.Use(AspNetCorePipelineEntries.StaticFiles, app =>
        {
            app.UseStaticFiles();
        }, entryOptions);

        var unitOfWorkOptions = context.GetOptions<GlobalUnitOfWorkOptions>().Value;

        context.Pipeline.Use(AspNetCorePipelineEntries.UnitOfWork, app =>
        {
            app.UseUnitOfWork();
        }, entryOptions, () => unitOfWorkOptions.IsEnabled);

        context.Pipeline.Use(AspNetCorePipelineEntries.Routing, app =>
        {
            app.UseRouting();
        }, entryOptions);

        context.Pipeline.Use(AspNetCorePipelineEntries.Authentication, app =>
        {
            app.UseAuthentication();
        }, entryOptions);

        context.Pipeline.Use(AspNetCorePipelineEntries.Authorization, app =>
        {
            app.UseAuthorization();
        }, entryOptions);

        context.Pipeline.Use(AspNetCorePipelineEntries.Endpoints, app =>
        {
            app.UseRamshaEndpoints();
        }, entryOptions);
    }
}
