using MemoryCacheT;

namespace Turkok.Core.Caching
{
     public class CacheManager<TKey, TValue> : ICacheManager<TKey, TValue>
     {
        private readonly ICache<TKey, TValue> _cache; 

        public CacheManager(ICache<TKey, TValue> cache)
        {
            _cache = cache;
        }
         
        public TValue Set(TKey key, TValue value)
        {
            if (_cache.TryAdd(key, value))
            {
                return value;
            }

            return default(TValue);
        }

        public TValue Get(TKey key)
        {
            TValue value;

            if (_cache.TryGetValue(key, out value))
            {
                return value;
            }

            return default(TValue);
        }

        public void Remove(TKey key)
        { 
            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key);
            }
        
        }
        public void InvalidateCacheItem(TKey key)
        {
            TValue value;

            if (_cache.TryGetValue(key, out value))
            {
                _cache.Remove(key);
            }
        }
    } 
}
