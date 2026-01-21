using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ramsha.AspNetCore;

public class RamshaGlobalExceptionHandler(
    ILogger<RamshaGlobalExceptionHandler> logger
    )
     : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An exception occurred: {Message}", exception.Message);

        var errorResult = exception.ToKnownError();

        httpContext.Response.StatusCode = (int)errorResult.Status;
        httpContext.Response.ContentType = "application/json; charset=utf-8";

        var jsonResult = JsonSerializer.Serialize(errorResult, new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        });
        await httpContext.Response.WriteAsync(jsonResult, cancellationToken);

        return true;
    }






}