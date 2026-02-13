
using Ramsha.Translations;


namespace Ramsha;

public static class RamshaBuilderExtensions
{
    public static RamshaBuilder AddTranslations(this RamshaBuilder ramsha)
    {
        ramsha.AddModule<TranslationsModule>();
        return ramsha;
    }

}
