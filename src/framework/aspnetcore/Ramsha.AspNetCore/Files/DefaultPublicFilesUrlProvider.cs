

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Ramsha.Files;

namespace Ramsha.AspNetCore
{
    // public class DefaultPublicFilesUrlProvider(IOptions<RamshaFilesOptions> options, IWebHostEnvironment hostEnvironment) : IPublicFilesUrlProvider
    // {
    //     private readonly RamshaFilesOptions _options = options.Value;

    //     public string GetBaseUrl()
    //     {
    //         return _options.PublicBaseUrl ?? hostEnvironment.WebRootPath;
    //     }

    //     public string GetFullUrl(string relativePath)
    //     {
    //         var baseUrl = GetBaseUrl();
    //         if (string.IsNullOrWhiteSpace(baseUrl))
    //             return relativePath;

    //         return $"{baseUrl.TrimEnd('/')}/{relativePath.Replace("\\", "/")}";
    //     }
    // }
}