using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.Account.Contracts;
using Ramsha.AspNetCore.Mvc;


namespace Ramsha.Account.Api;

public static class MvcBuilderExtensions
{
    public static void AddAccountGenericControllers(this IMvcBuilder builder)
    {
        var typesOptions = builder.Services.ExecutePreparedOptions<RamshaTypeReplacementOptions>();
        builder.AddGenericControllers(
            typeof(RamshaAccountController<>).MakeGenericType(typesOptions.GetOrBase<RamshaRegisterDto>())
            );
    }
}
