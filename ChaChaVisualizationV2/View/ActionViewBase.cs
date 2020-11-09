using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Validation;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cryptool.Plugins.ChaChaVisualizationV2.View
{
    /// <summary>
    /// Class with helper methods which all views whose view model implements IActionNavigation use.
    /// Implements the event handler for the slider and the action input textbox.
    /// </summary>
    internal static class ActionViewBase
    {
        /// <summary>
        /// Attaches the event handler from the view model to the slider and textbox which are inside a template.
        /// </summary>
        /// <param name="Root">The ContentControl which has the slider and textbox in its template.</param>
        public static void AddEventHandlers(ActionViewModelBase viewModel, ContentControl root)
        {
            // Add value changed event handler to action slider
            root.ApplyTemplate();
            Slider actionSlider = (Slider)root.Template.FindName("ActionSlider", root);
            actionSlider.ValueChanged += viewModel.HandleActionSliderValueChange;

            TextBox actionInputTextbox = (TextBox)root.Template.FindName("ActionInputTextBox", root);
            actionInputTextbox.KeyDown += viewModel.HandleUserActionInput;
            // The following code adds a binding with validation to the action input textbox.
            // (It was not possible in pure XAML because the ValidationRule
            // needs an argument and ValidationRule is not a DependencyObject thus no data binding available
            // to pass in the argument.)
            Binding actionInputBinding = new Binding("CurrentUserActionIndex")
            { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };

            ValidationRule inputActionIndexRule = new UserActionInputValidationRule(viewModel.TotalActions);
            actionInputBinding.ValidationRules.Add(inputActionIndexRule);
            actionInputTextbox.SetBinding(TextBox.TextProperty, actionInputBinding);
        }
    }
}