using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Cryptool.Plugins.ChaChaVisualizationV2.ViewModel
{
    /// <summary>
    /// Base class for all view models.
    /// <remarks>
    /// https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#viewmodelbase-class
    /// </remarks>
    /// </summary>
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        public bool ThrowOnInvalidPropertyName { get; set; } = true;

        public ViewModelBase(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Name of page. Used as content in page navigation buttons.
        /// </summary>
        private string _name; public string Name

        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }
    }
}