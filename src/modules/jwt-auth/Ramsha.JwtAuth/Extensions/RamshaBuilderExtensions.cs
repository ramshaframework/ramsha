
using Ramsha.JwtAuth;


namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddJwtAuth(this RamshaBuilder ramsha)
    {
        ramsha.AddModule<JwtAuthModule>();
        return ramsha;
    }
}
