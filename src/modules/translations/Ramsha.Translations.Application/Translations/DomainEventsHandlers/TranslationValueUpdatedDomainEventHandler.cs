using Ramsha.Caching;
using Ramsha.Localization;
using Ramsha.LocalMessaging.Abstractions;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Application
{
    public class TranslationValueUpdatedDomainEventHandler(IRamshaCache cache) : ILocalEventHandler<TranslationValueUpdatedDomainEvent>
    {
        public async Task HandleAsync(TranslationValueUpdatedDomainEvent message, CancellationToken cancellationToken = default)
        {
            var resourceName = message.Translation.ResourceName;

            await cache.RemoveByTagAsync(RamshaResourcesLoader.ResourceTagPrefix + resourceName);
        }
    }
}