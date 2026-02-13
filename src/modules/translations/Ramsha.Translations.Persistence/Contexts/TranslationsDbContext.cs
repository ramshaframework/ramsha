

using Microsoft.EntityFrameworkCore;
using Ramsha.Common.Domain;
using Ramsha.EntityFrameworkCore;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Persistence
{
    [ConnectionString(TranslationsDbContextConstants.ConnectionStringName)]
    public class TranslationsDbContext(DbContextOptions<TranslationsDbContext> options)
     : RamshaEFDbContext<TranslationsDbContext>(options), ITranslationsDbContext
    {
        public DbSet<Translation> Translations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureTranslations();
        }
    }
}