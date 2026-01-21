using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Ramsha.Core;
using Ramsha.Identity.Shared;

using static Ramsha.Identity.Shared.RamshaIdentityErrorsCodes;

namespace Ramsha.Identity.Domain;

public static class IdentityResultExtensions
{
    public static RamshaErrorResult MapToRamshaError(
     this IdentityResult result,
     RamshaResultStatus status = RamshaResultStatus.Invalid)
    {
        List<NamedError> errors = result
        .Errors
        .Select(e => new NamedError(e.Code, e.Description, e.Code))
        .ToList();

        return new RamshaErrorResult(status, "Identity Errors happened .", Default, errors);
    }
}
