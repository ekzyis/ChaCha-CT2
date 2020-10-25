using System;
using System.Collections.Generic;

namespace Cryptool.Plugins.ChaChaVisualization
{
    /**
     * Interface to create actions which can be undone automatically.
     */
    interface IActionNavigation<T1, T2, T3, T4, T5> // I wish I had variadic templates in C# like in C++11 ...
    {
        // Save state of each element such that we can retrieve it later for undoing action.
        void SaveState(params T1[] t);

        void SaveState(params T2[] t);

        void SaveState(params T3[] t);

        void SaveState(params T4[] t);

        void SaveState(params T5[] t);

        // Tells that current page action is finished and thus next calls to save state are for a new action.
        void FinishPageAction();

        // Get list of actions which completely reverts the action of the given index.
        Dictionary<int, Action> GetUndoActions();

        // Execute automatic undoing of actions.
        void Undo();
    }
}
