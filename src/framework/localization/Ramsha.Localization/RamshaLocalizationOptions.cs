using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;

public class RamshaLocalizationOptions
{
    public TimeSpan ResourcesCacheExpiration { get; set; } = TimeSpan.FromDays(30);
    public TimeSpan ResourcesLocalCacheExpiration { get; set; } = TimeSpan.FromDays(1);
    public ResourceDefinitions Resources { get; set; } = [];
    public string ResourcesPath { get; set; } = "Resources";
    public LanguageInfo DefaultLanguage { get; set; } = new("ar", "ar");
    public LanguageInfo[] SupportedLanguages { get; set; } = [new("ar", "ar"), new("en", "en")];
    public ITypeList<ILocalizationResourceStore> ResourcesStores { get; }
    public RamshaLocalizationOptions()
    {
        ResourcesStores = new TypeList<ILocalizationResourceStore>();
    }
}
