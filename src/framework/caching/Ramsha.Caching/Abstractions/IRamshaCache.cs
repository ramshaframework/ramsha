using Microsoft.Extensions.Caching.Hybrid;

namespace Ramsha.Caching;

public interface IRamshaCache
{
    ValueTask SetAsync<T>(
        string key,
        T value,
        RamshaCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default);

    ValueTask RemoveAsync(
        string key,
        CancellationToken cancellationToken = default);

    ValueTask RemoveByTagAsync(
    string tag,
    CancellationToken cancellationToken = default);

    ValueTask<T> GetOrCreateAsync<T>(
    string key,
    Func<CancellationToken, ValueTask<T>> factory,
    RamshaCacheEntryOptions? options = null,
       IEnumerable<string>? tags = null,
    CancellationToken cancellationToken = default);

}

public class RamshaCache(HybridCache cache) : IRamshaCache
{
    public async ValueTask SetAsync<T>(
        string key,
        T value,
        RamshaCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        await cache.SetAsync(
            key,
            value,
            MapOptions(options),
            cancellationToken: cancellationToken);
    }

    public ValueTask RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        return cache.RemoveAsync(key, cancellationToken);
    }

    public async ValueTask<T> GetOrCreateAsync<T>(
        string key,
        Func<CancellationToken, ValueTask<T>> factory,
        RamshaCacheEntryOptions? options = null,
        IEnumerable<string>? tags = null,
        CancellationToken cancellationToken = default)
    {
        return await cache.GetOrCreateAsync(
            key,
            factory,
            MapOptions(options),
            cancellationToken: cancellationToken);
    }

    public ValueTask RemoveByTagAsync(string tag, CancellationToken cancellationToken = default)
    {
        return cache.RemoveByTagAsync(tag, cancellationToken);
    }

    private static HybridCacheEntryOptions? MapOptions(RamshaCacheEntryOptions? options)
    {
        return options is not null ? new HybridCacheEntryOptions
        {
            Expiration = options.Expiration,
            LocalCacheExpiration = options.LocalCacheExpiration
        } : null;
    }
}
