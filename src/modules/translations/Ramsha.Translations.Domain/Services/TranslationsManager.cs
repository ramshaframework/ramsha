

using Ramsha.Common.Domain;

namespace Ramsha.Translations.Domain
{
    public class TranslationsManager(ITranslationRepository repository) : RamshaDomainManager
    {
        public async Task<RamshaResult<Translation>> SetAsync(string key, string value, string resourceName, string culture)
        {
            var text = await repository.FindAsync(x =>
                        x.ResourceName == resourceName
                        && x.Culture == culture
                        && x.Key == key);

            if (text is null)
            {
                text = new Translation(
                     key,
                     resourceName,
                     culture
                       );

                await repository.AddAsync(text);
            }

            text.SetValue(value);



            return text;
        }
    }
}