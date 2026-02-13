using System;
using Ramsha.Common.Domain;

namespace Ramsha.Translations.Domain;

public class Translation : AggregateRoot<int>
{
    private Translation() { }

    public Translation(string key, string resourceName, string culture)
    {
        ResourceName = resourceName;
        Culture = culture;
        Key = key;
    }

    public void SetValue(string value)
    {
        Value = value;
        RaiseEvent(new TranslationValueUpdatedDomainEvent(this));
    }
    public string ResourceName { get; private set; } = default!;
    public string Culture { get; private set; } = default!;
    public string Key { get; private set; } = default!;
    public string Value { get; private set; } = default!;

}
