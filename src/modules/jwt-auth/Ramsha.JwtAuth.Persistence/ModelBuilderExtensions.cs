
using Microsoft.EntityFrameworkCore;
using Ramsha.JwtAuth.Domain;

namespace Ramsha.JwtAuth.Persistence;

public static class ModelBuilderExtensions
{
    public static void ConfigureJwtAuth(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RamshaRefreshToken>(entity =>
        {
            entity.ToTable("RamshaRefreshTokens");
        });
    }

}