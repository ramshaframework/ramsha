

namespace Ramsha.Caching
{
    public class RamshaCacheEntryOptions
    {
        public TimeSpan Expiration { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan LocalCacheExpiration { get; set; } = TimeSpan.FromSeconds(5);
    }
}