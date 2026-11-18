

namespace Ramsha.Common.Domain;

public interface IConnectionStringResolver
{
    Task<string?> ResolveAsync(string? connectionStringName = null);
    string? Resolve(string? connectionStringName = null);


}
