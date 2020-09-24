using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptool.Plugins.ChaCha
{
    class ActionCache : IActionCache
    {
        public static readonly ActionCache Empty = new ActionCache(new Dictionary<int, CachePageAction>());
        private Dictionary<int, CachePageAction> _cache;
        public ActionCache(Dictionary<int, CachePageAction> cache)
        {
            _cache = cache;
        }
        public ActionCache(params ActionCache[] caches)
        {
            _cache = caches.SelectMany(cache => cache._cache).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        public bool NotEmpty
        {
            get
            {
                return _cache.Keys.Count != 0;
            }
        }
        public CachePageAction Get(int actionIndex)
        {
            return _cache[actionIndex];
        }

        public bool Includes(int actionIndex)
        {
            return _cache.ContainsKey(actionIndex);
        }

        public void Set(CachePageAction pageAction, int actionIndex)
        {
            _cache[actionIndex] = pageAction;
        }

        public int GetClosestCache(int n)
        {
            return _cache.Keys.ToArray().OrderBy(x => Math.Abs(x - n)).First();
        }
    }
}
