using Ramsha.EntityFrameworkCore;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Persistence
{
    public class EfCoreTranslationRepository : EFCoreRepository<ITranslationsDbContext, Translation>, ITranslationRepository
    {

    }
}