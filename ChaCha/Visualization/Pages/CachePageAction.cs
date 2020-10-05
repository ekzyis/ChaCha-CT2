using System;

namespace Cryptool.Plugins.ChaCha
{
    class CachePageAction : PageAction
    {
        public CachePageAction(Action exec) : base(exec, null) { }

        public CachePageAction() : base(() => { }, null) { }

        public override void Undo()
        {
            throw new InvalidOperationException("Undo should never be called on a cached page action");
        }
    }
}
