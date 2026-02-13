

using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Persistence
{
    [ConnectionString(TranslationsDbContextConstants.ConnectionStringName)]
    public interface ITranslationsDbContext : IRamshaEFDbContext
    {
        DbSet<Translation> Translations { get; }
    }
}