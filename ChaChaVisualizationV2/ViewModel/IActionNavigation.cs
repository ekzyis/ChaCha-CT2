namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    internal interface IActionNavigation
    {
        /// <summary>
        /// The index of the action in the list of all actions which is currently visible on the page.
        /// If currently no action is visible (initial state of page), the index can be negative.
        /// </summary>
        int CurrentActionIndex { get; }

        /// <summary>
        /// Total amount of actions this page has.
        /// </summary>
        int TotalActions { get; }

        /// <summary>
        /// Does this page has any actions at all?
        /// </summary>
        bool HasActions { get; }

        /// <summary>
        /// Move to Action at index n.
        ///
        /// This implements absolute action navigation.
        /// </summary>
        /// <param name="n"></param>
        void MoveToAction(int n);

        /// <summary>
        /// Move n Actions from the current one. If n is positive, go to the n-th next action. If n is negative, go to the n-th previous action.
        ///
        /// This implements relative action navigation.
        /// </summary>
        /// <param name="n"></param>
        void MoveActions(int n);

        /// <summary>
        /// Move to next action if there is one.
        /// </summary>
        void NextAction();

        /// <summary>
        /// Move to previous action if there is one.
        /// </summary>
        void PrevAction();

        /// <summary>
        /// Go to the last action.
        /// </summary>
        void MoveToLastAction();

        /// <summary>
        /// Go to the first action.
        /// </summary>
        void MoveToFirstAction();
    }
}