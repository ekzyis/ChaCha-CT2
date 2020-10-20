using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha
{
    partial class Page
    {
        private protected Page(ContentControl pageElement, ActionCache cache)
        {
            _page = pageElement;
            Cache = cache;
        }
        private protected Page(ContentControl pageElement)
        {
            _page = pageElement;
            Cache = ActionCache.Empty;
        }
        // the visual tree element which contains the page - the Visibility of this element will be set to Collapsed / Visible when going to next / previous page.
        private readonly ContentControl _page;
        private List<PageAction> _pageActions = new List<PageAction>();

        public ActionCache Cache { get; protected set; }
        public int ActionFrames => _pageActions.Count;

        public PageAction[] Actions => _pageActions.ToArray();
        public void AddAction(params PageAction[] pageActions)
        {
            foreach (PageAction pageAction in pageActions)
            {
                _pageActions.Add(pageAction);
            }
        }
        public Visibility Visibility
        {
            get => _page.Visibility;
            set => _page.Visibility = value;
        }
        public StackPanel PageNavigationBar
        {
            get
            {
                _page.ApplyTemplate();
                return (StackPanel)_page.Template.FindName("PageNavBar", _page);
            }
        }
        public StackPanel ActionNavigationBar
        {
            get
            {
                _page.ApplyTemplate();
                return (StackPanel)_page.Template.FindName("ActionNavBar", _page);
            }
        }

        public StackPanel PageNavigationBar2
        {
            get
            {
                _page.ApplyTemplate();
                return (StackPanel)_page.Template.FindName("PageNavBar2", _page);
            }
        }

        // Initialize a page before entering.
        // Can be used to setup things using variables which weren't available during instance creation.
        public virtual void Setup() { }

        // Reset a page before leaving.
        // This should undo the changes `Setup` applied to the page.
        public virtual void TearDown() { }

        public void InsertAction(string label, PageAction action)
        {
            int insertIndex = _pageActions.FindIndex(a => a.Labels.Any(l => l == label));
            _pageActions.Insert(insertIndex, action);
        }
        public void InsertAction(string label, IEnumerable<PageAction> action)
        {
            int insertIndex = _pageActions.FindIndex(a => a.Labels.Any(l => l == label));
            IEnumerable<PageAction> reversedActions = action.Reverse();
            // We reverse the array to ensure that the order of inserted elements is the same as before reversing.
            // Basically, the first element inserted is the element the furthest away from the starting index after all elements have been inserted.
            foreach(PageAction a in reversedActions)
            {
                _pageActions.Insert(insertIndex + 1, a);
            }
        }

        public void RemoveActionRange(string label, int count)
        {
            int insertIndex = _pageActions.FindIndex(a => a.Labels.Any(l => l == label));
            _pageActions.RemoveRange(insertIndex + 1, count);
        }
    }
}
