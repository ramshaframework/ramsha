

using Ramsha.EntityFrameworkCore.SqlServer;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddEFSqlServer(this RamshaBuilder ramsha)
    {
        ramsha.AddModule<EntityFrameworkCoreSqlServerModule>();
        return ramsha;
    }
}
