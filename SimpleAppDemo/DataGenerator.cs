

using Bogus;
using Ramsha.Identity.Domain;

namespace SimpleAppDemo;

public class DataGenerator
{
    public static List<RamshaIdentityUser> GenerateUserList(int count, bool useNewSeed = false)
    {
        return GetUserFaker(useNewSeed).Generate(count);
    }

    public static RamshaIdentityUser GenerateUser(bool useNewSeed = false)
    {
        return GenerateUserList(1, useNewSeed).Single();
    }

    private static Faker<RamshaIdentityUser> GetUserFaker(bool useNewSeed)
    {
        var seed = 0;
        if (useNewSeed)
        {
            seed = Random.Shared.Next(10, int.MaxValue);
        }

        return new Faker<RamshaIdentityUser>()
            .RuleFor(p => p.UserName, (f, s) => s.UserName = f.Name.FirstName())
            .RuleFor(p => p.Email, (f, s) => s.Email = s.UserName + "@gmail.com")
            .UseSeed(seed);
    }
}