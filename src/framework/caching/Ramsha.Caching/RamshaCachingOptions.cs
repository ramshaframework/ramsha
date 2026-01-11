namespace Ramsha.Caching;

public class RamshaCachingOptions
{
    public int MaximumPayloadBytes { get; set; } = 10 * 1024 * 1024;
    public int MaximumKeyLength { get; set; } = 1024;
    public RamshaCacheEntryOptions DefaultEntryOptions { get; set; } = new();
}
