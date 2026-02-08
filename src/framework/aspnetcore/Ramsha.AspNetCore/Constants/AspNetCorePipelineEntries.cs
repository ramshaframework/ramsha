using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ramsha.AspNetCore;

public static class AspNetCorePipelineEntries
{
    public const string ExceptionHandler = Prefix + "exception-handler";
    public const string Prefix = "aspnetcore_";
    public const string Localization = Prefix + "localization";

    public const string Authentication = Prefix + "authentication";
    public const string Authorization = Prefix + "authorization";
    public const string Routing = Prefix + "routing";
    public const string Endpoints = Prefix + "endpoints";
    public const string StaticFiles = Prefix + "static-files";
    public const string HttpsRedirection = Prefix + "https-redirection";
    public const string UnitOfWork = Prefix + "uow";





}
