namespace Turkok.Core.Caching
{
    public interface ICacheManager<in TKey, TValue>
    {
        TValue Set(TKey key, TValue value);
        TValue Get(TKey key);
        void Remove(TKey key);
        void InvalidateCacheItem(TKey key);
    }
}
