using System;
using Microsoft.Extensions.Localization;

namespace Ramsha.Localization.Abstractions;


public record LanguageInfo(string Culture, string UiCulture);
public interface IRamshaStringLocalizer : IStringLocalizer
{

}
