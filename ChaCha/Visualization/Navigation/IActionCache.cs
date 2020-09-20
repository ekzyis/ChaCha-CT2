namespace Cryptool.Plugins.ChaCha
{
    /**
     * Interface for page action caches.
     */
    interface IActionCache
    {
        /**
         * Check if action at actionIndex is included in this cache.
         */
        bool Includes(int actionIndex);

        /**
         * Get the page action for this action index if it exists.
         */
        CachePageAction Get(int actionIndex);

        /**
         * Cache PageAction with this action index, linking them together.
         */
        void Set(CachePageAction pageAction, int actionIndex);
    }
}
