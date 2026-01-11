using Ramsha.Permissions;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddPermissions(this RamshaBuilder ramsha)
    {
        ramsha.AddModule<PermissionsModule>();
        return ramsha;
    }

}