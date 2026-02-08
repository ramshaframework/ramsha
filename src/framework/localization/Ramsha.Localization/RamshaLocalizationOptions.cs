using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;

public class RamshaLocalizationOptions
{
    public ResourceDefinitions Resources { get; set; } = [];
    public string ResourcesPath { get; set; } = "Resources";
    public LanguageInfo DefaultLanguage { get; set; } = new("ar", "ar");
    public LanguageInfo[] SupportedLanguages { get; set; } = [new("ar", "ar"), new("en", "en")];
}
