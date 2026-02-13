

using Microsoft.EntityFrameworkCore;
using Ramsha.Translations.Domain;

namespace Ramsha.Translations.Persistence
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ConfigureTranslations(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("RamshaTranslations");

                entity.HasIndex(x => new { x.Key, x.ResourceName, x.Culture })
                .IsUnique();
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("RamshaLanguages");
            });

            return modelBuilder;
        }
    }
}