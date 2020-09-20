using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha
{
    class ActionCache : IActionCache
    {
        private Dictionary<int, CachePageAction> _cache;
        private ChaChaPresentation _pres;
        public ActionCache(ChaChaPresentation pres)
        {
            _pres = pres;
            _cache = GetCache(pres);
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

        public static Dictionary<int, CachePageAction> GetCache(ChaChaPresentation pres)
        {
            CachePageAction GenerateRoundCacheEntry(int round)
            {
                CachePageAction cache = new CachePageAction();
                cache.AddToExec(() =>
                {
                    KeystreamBlockGenPage.ClearQRDetail(pres);
                    // update state matrix
                    TextBox[] stateTextBoxes = KeystreamBlockGenPage.GetStateTextBoxes(pres);
                    uint[] round2State = pres.GetResult(ResultType.CHACHA_HASH_ROUND, round - 1);
                    Debug.Assert(stateTextBoxes.Length == round2State.Length);
                    for (int x = 0; x < round2State.Length; ++x)
                    {
                        stateTextBoxes[x].Text = ChaChaPresentation.HexString(round2State[x]);
                    }
                    // highlight only diagonal state entries because they will be copied into QR detail in next action
                    (int i, int j, int k, int l) = KeystreamBlockGenPage.GetStateIndices(round);
                    KeystreamBlockGenPage.UnmarkAllStateEntriesExcept(pres, i, j, k, l);
                    // update round indicator
                    pres.CurrentRoundIndex = round;
                    pres.Nav.Replace(pres.CurrentRound, round.ToString());
                    // hide all QR arrows except the one for the round
                    int qrIndex = round % 2 == 1 ? 1 : 5;
                    KeystreamBlockGenPage.HideAllQRArrowsExcept(pres, qrIndex);
                });
                return cache;
            }
            // Cache entry for action index: 3 - "Round 1"
            CachePageAction cache3 = GenerateRoundCacheEntry(1);
            // Cache entry for action index: 283 - "Round 2"
            CachePageAction cache283 = GenerateRoundCacheEntry(2);
            // Cache entry for action index: 563 - "Round 3"
            CachePageAction cache563 = GenerateRoundCacheEntry(3);
            // Cache entry for action index: 843 - "Round 4"
            CachePageAction cache843 = GenerateRoundCacheEntry(4);
            // Cache entry for action index: 1123 - "Round 5"
            CachePageAction cache1123 = GenerateRoundCacheEntry(5);
            // Cache entry for action index: 1403 - "Round 6"
            CachePageAction cache1403 = GenerateRoundCacheEntry(6);
            // Cache entry for action index: 1683 - "Round 7"
            CachePageAction cache1683 = GenerateRoundCacheEntry(7);
            // Cache entry for action index: 1963 - "Round 8"
            CachePageAction cache1963 = GenerateRoundCacheEntry(8);
            // Cache entry for action index: 2243 - "Round 9"
            CachePageAction cache2243 = GenerateRoundCacheEntry(9);
            // Cache entry for action index: 2523 - "Round 10"
            CachePageAction cache2523 = GenerateRoundCacheEntry(10);
            // Cache entry for action index: 2803 - "Round 11"
            CachePageAction cache2803 = GenerateRoundCacheEntry(11);
            // Cache entry for action index: 3083 - "Round 12"
            CachePageAction cache3083 = GenerateRoundCacheEntry(12);
            // Cache entry for action index: 3363 - "Round 13"
            CachePageAction cache3363 = GenerateRoundCacheEntry(13);
            // Cache entry for action index: 3643 - "Round 14"
            CachePageAction cache3643 = GenerateRoundCacheEntry(14);
            // Cache entry for action index: 3923 - "Round 15"
            CachePageAction cache3923 = GenerateRoundCacheEntry(15);
            // Cache entry for action index: 4203 - "Round 16"
            CachePageAction cache4203 = GenerateRoundCacheEntry(16);
            // Cache entry for action index: 4483 - "Round 17"
            CachePageAction cache4483 = GenerateRoundCacheEntry(17);
            // Cache entry for action index: 4763 - "Round 18"
            CachePageAction cache4763 = GenerateRoundCacheEntry(18);
            // Cache entry for action index: 5043 - "Round 19"
            CachePageAction cache5043 = GenerateRoundCacheEntry(19);
            // Cache entry for action index: 5323 - "Round 20"
            CachePageAction cache5323 = GenerateRoundCacheEntry(20);
            return new Dictionary<int, CachePageAction>
            {
                { 3, cache3 },
                { 283, cache283 },
                { 563, cache563 },
                { 843, cache843 },
                { 1123, cache1123 },
                { 1403, cache1403 },
                { 1683, cache1683 },
                { 1963, cache1963 },
                { 2243, cache2243 },
                { 2523, cache2523 },
                { 2803, cache2803 },
                { 3083, cache3083 },
                { 3363, cache3363 },
                { 3643, cache3643 },
                { 3923, cache3923 },
                { 4203, cache4203 },
                { 4483, cache4483 },
                { 4763, cache4763 },
                { 5043, cache5043 },
                { 5323, cache5323 }
            };
        }
    }
}
