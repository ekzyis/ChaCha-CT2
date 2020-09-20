using System.Collections.Generic;
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
        private readonly List<PageAction> _pageActions = new List<PageAction>();
        private readonly List<PageAction> _pageInitActions = new List<PageAction>();

        public ActionCache Cache { get; }
        public int ActionFrames => _pageActions.Count;

        public PageAction[] Actions => _pageActions.ToArray();

        public void AddAction(params PageAction[] pageActions)
        {
            foreach (PageAction pageAction in pageActions)
            {
                _pageActions.Add(pageAction);
            }
        }
        public PageAction[] InitActions => _pageInitActions.ToArray();

        public void AddInitAction(PageAction pageAction)
        {
            _pageInitActions.Add(pageAction);
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
    }
}
