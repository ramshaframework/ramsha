
using Ramsha.Common.Domain;

namespace SimpleAppDemo.LocalizationModule
{
    public class LocalizationText : Entity<Guid>
    {
        private LocalizationText() { }
        public LocalizationText(Guid id, string resourceName, string culture, string key)
        {
            Id = id;
            ResourceName = resourceName;
            Culture = culture;
            Key = key;
        }

        public void SetValue(string value)
        {
            Value = value;
        }
        public string ResourceName { get; private set; } = default!;
        public string Culture { get; private set; } = default!;
        public string Key { get; private set; } = default!;
        public string Value { get; private set; } = default!;
    }

}