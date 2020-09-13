using System.Collections.Generic;

namespace Cryptool.Plugins.ChaCha
{
    class ActionCache : IActionCache
    {
        private Dictionary<int, CachePageAction> _cache;
        private ChaChaPresentation _pres;
        public ActionCache(ChaChaPresentation pres)
        {
            _pres = pres;
            _cache = GetCache();
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

        public static Dictionary<int, CachePageAction> GetCache()
        {
            // TODO init cache
            return null;
        }
    }
}
