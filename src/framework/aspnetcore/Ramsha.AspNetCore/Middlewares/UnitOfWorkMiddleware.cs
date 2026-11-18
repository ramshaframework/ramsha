using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.UnitOfWork;
using Ramsha.UnitOfWork.Abstractions;

namespace Ramsha.AspNetCore;

public class UnitOfWorkMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var unitOfWorkManager = context.RequestServices.GetRequiredService<IUnitOfWorkManager>();
        using (var uow = unitOfWorkManager.Reserve(RamshaUnitOfWorkReservationNames.ActionUnitOfWorkReservationName))
        {
            await next(context);
            await uow.CompleteAsync();
        }
    }
}

public static class UnitOfWorkMiddlewareExtensions
{
    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UnitOfWorkMiddleware>();
    }
}