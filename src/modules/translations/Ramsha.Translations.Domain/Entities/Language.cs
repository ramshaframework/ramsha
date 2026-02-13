
using Microsoft.Extensions.DependencyInjection;
using Ramsha.Common.Domain;

namespace Ramsha.Translations.Domain
{
    public class Language : AggregateRoot<int>
    {
        private Language()
        {

        }
        private Language(string culture, string displayName, bool isActive = false)
        {
            Culture = culture;
            DisplayName = displayName;
            IsActive = isActive;
        }

        internal static Language Create(string culture, string? displayName = null, bool isActive = false)
        {
            return new Language(culture, displayName ?? culture, isActive);
        }

        internal void Activate()
        {
            IsActive = true;
            RaiseEvent(new LanguageActivatedDomainEvent(this));
        }

        internal void Deactivate()
        {
            IsActive = false;
            RaiseEvent(new LanguageDeactivatedDomainEvent(this));
        }


        internal void SetDefault(bool isDefault)
        {
            IsDefault = isDefault;
        }

        public string Culture { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsDefault { get; private set; }
        public string DisplayName { get; private set; }
    }
}