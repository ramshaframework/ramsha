using Ramsha.SettingsManagement;

namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddSettingsManagement(this RamshaBuilder ramsha)
    {
        ramsha.AddModule<SettingsManagementModule>();
        return ramsha;
    }
}
