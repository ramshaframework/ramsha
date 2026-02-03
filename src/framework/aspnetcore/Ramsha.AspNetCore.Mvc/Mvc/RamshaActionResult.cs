
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Ramsha.AspNetCore.Mvc;

public class RamshaActionResult : ObjectResult
{
    public RamshaActionResult(
        [ActionResultObjectValue] IRamshaResult result,
        HttpContext httpContext
    ) : base(
            result is IRamshaValueSuccessResult valueSuccessResult ? valueSuccessResult.Value : result is not RamshaErrorResult error ? result : new RamshaActionResultError(error, httpContext)
        )
    {
        MapResult(result,httpContext);
    }

    protected virtual void MapResult(IRamshaResult result,HttpContext httpContext)
    {
        if(result is IRamshaPagedResult pagedResult)
        {
                  var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        httpContext.Response.Headers.Append("PagedInfo", JsonSerializer.Serialize(pagedResult.PagedInfo, options));
        httpContext.Response.Headers.Append("Access-Control-Expose-Headers", "PagedInfo");
            
        }
        
        StatusCode = (int)result.Status;
    }

}


public class RamshaActionResultError(RamshaErrorResult error, HttpContext context)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ConnectionId { get; } =
        ((ushort)error.Status) is < 500 ? null
            : context.Connection.Id;

    // TODO: this throws error if no session is created.
    //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    //public string? SessionId { get; } = ((ushort)error.Status) is < 500 ? null 
    //    : context.Session.Id;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; } =
        ((ushort)error.Status) is < 500 ? null : context.TraceIdentifier;

    public string? Code { get; } = error.Code;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Message { get; } = error.Message;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<NamedError>? Errors { get; } = error.Errors;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public RamshaErrorContext? Context { get; } =
        error.Context is null
            ? null
            : new(
                CorrelationId: error.Context.CorrelationId,
                ExceptionMeta: error.Context.ExceptionMeta,
                SourceName: error.Context.SourceName
            );
}
