
using Microsoft.EntityFrameworkCore;
using Ramsha.Jwt.Domain;

namespace Ramsha.Jwt.Persistence;

public static class ModelBuilderExtensions
{
    public static void ConfigureJwt(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RamshaRefreshToken>(entity =>
        {
            entity.ToTable("RamshaRefreshTokens");
        });
    }

}