using System;
using Microsoft.Extensions.Localization;

namespace Ramsha.Localization.Abstractions;


public record LanguageInfo(string Culture, string DisplayName);
public interface IRamshaStringLocalizer : IStringLocalizer
{

}
