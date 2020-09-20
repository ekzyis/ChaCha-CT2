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
            // Cache entry for action index: 283 - "Round 2"
            CachePageAction cache283 = new CachePageAction();
            cache283.AddToExec(() => KeystreamBlockGenPage.ClearQRDetail(pres));
            cache283.AddToExec(() =>
            {
                // update state matrix
                TextBox[] stateTextBoxes = KeystreamBlockGenPage.GetStateTextBoxes(pres);
                uint[] round2State = pres.GetResult(ResultType.CHACHA_HASH_ROUND, 1);
                Debug.Assert(stateTextBoxes.Length == round2State.Length);
                for (int i = 0; i < round2State.Length; ++i)
                {
                    stateTextBoxes[i].Text = ChaChaPresentation.HexString(round2State[i]);
                }
                // highlight only diagonal state entries because they will be copied into QR detail in next action
                KeystreamBlockGenPage.UnmarkAllStateEntriesExcept(pres, 0, 5, 10, 15);
                // update round indicator
                pres.CurrentRoundIndex = 2;
                pres.Nav.Replace(pres.CurrentRound, "2");
                // hide all QR arrows except the fifth
                KeystreamBlockGenPage.HideAllQRArrowsExcept(pres, 5);
            });
            return new Dictionary<int, CachePageAction>{{ 283, cache283 } };
        }
    }
}
