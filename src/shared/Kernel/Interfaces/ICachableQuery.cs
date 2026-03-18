namespace Kernel.Interfaces;

/// <summary>
/// Defines the contract for a query that supports caching by providing a cache key and an optional expiry duration.
/// </summary>
public interface ICachableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiry { get; }
}
