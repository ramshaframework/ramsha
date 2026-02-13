using Ramsha.EntityFrameworkCore;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Persistence
{
    public class EfCoreLanguageRepository : EFCoreRepository<ITranslationsDbContext, Language, int>, ILanguageRepository
    {

    }
}