using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Cryptool.Plugins.ChaCha
{
    public class Page
    {
        public Page(ContentControl pageElement)
        {
            _page = pageElement;
        }
        // the visual tree element which contains the page - the Visibility of this element will be set to Collapsed / Visible when going to next / previous page.
        private readonly ContentControl _page;
        private readonly List<PageAction> _pageActions = new List<PageAction>();
        private readonly List<PageAction> _pageInitActions = new List<PageAction>();
        public int ActionFrames
        {
            get
            {
                return _pageActions.Count;
            }
        }
        public PageAction[] Actions
        {
            get
            {
                return _pageActions.ToArray();
            }
        }
        public void AddAction(params PageAction[] pageActions)
        {
            foreach (PageAction pageAction in pageActions)
            {
                _pageActions.Add(pageAction);
            }
        }
        public PageAction[] InitActions
        {
            get
            {
                return _pageInitActions.ToArray();
            }
        }
        public void AddInitAction(PageAction pageAction)
        {
            _pageInitActions.Add(pageAction);
        }
        public Visibility Visibility
        {
            get
            {
                return _page.Visibility;
            }
            set
            {
                _page.Visibility = value;
            }
        }
        public StackPanel PageNavigationBar
        {
            get
            {
                bool b = _page.ApplyTemplate();
                return (StackPanel)_page.Template.FindName("PageNavBar", _page);
            }
        }
        public StackPanel ActionNavigationBar
        {
            get
            {
                bool b = _page.ApplyTemplate();
                return (StackPanel)_page.Template.FindName("ActionNavBar", _page);
            }
        }
    }
}
