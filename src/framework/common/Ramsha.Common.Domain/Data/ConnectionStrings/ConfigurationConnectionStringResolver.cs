using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Ramsha.Common.Domain;

public class DefaultConnectionStringResolver(IOptions<ConnectionStringsOptions> options) : IConnectionStringResolver
{
    public string? Resolve(string? connectionStringName = null)
    {
        return options.Value.Get(connectionStringName ?? "Default");
    }

    public Task<string?> ResolveAsync(string? connectionStringName = null)
    {
        return Task.FromResult(Resolve(connectionStringName));
    }
}
