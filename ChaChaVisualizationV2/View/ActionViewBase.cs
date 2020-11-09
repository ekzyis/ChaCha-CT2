using Cryptool.Plugins.ChaChaVisualizationV2.Helper;
using Cryptool.Plugins.ChaChaVisualizationV2.Helper.Validation;
using Cryptool.Plugins.ChaChaVisualizationV2.ViewModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
        /// <param name="root">The ContentControl which has the slider and textbox in its template.</param>
        public static void AddEventHandlers(ActionViewModelBase viewModel, ContentControl root)
        {
            // --- ACTION SLIDER ---
            // Add value changed event handler to action slider
            root.ApplyTemplate();
            Slider actionSlider = (Slider)root.Template.FindName("ActionSlider", root);
            actionSlider.ValueChanged += viewModel.HandleActionSliderValueChange;

            // --- USER ACTION INPUT ---
            TextBox actionInputTextbox = (TextBox)root.Template.FindName("ActionInputTextBox", root);
            int maxAction = viewModel.TotalActions - 1;
            InitUserInputField(actionInputTextbox, "CurrentUserActionIndex", 0, maxAction, viewModel.HandleUserActionInput);
        }

        /// <summary>
        /// Initialize the textbox to support user input including validation.
        /// It adds a two-way binding to the property at the specified path and adds a validation rule
        /// such that only the minimum and maximum value are valid.
        /// It also adds the given handler to the 'KeyDown' event and sets the maximum length
        /// to the amount of digits the maximum value has.
        /// </summary>
        /// <param name="tb">The Textbox we want to setup.</param>
        /// <param name="bindingPath">Path to property the Textbox should bind to.</param>
        /// <param name="min">Minimum valid user value-</param>
        /// <param name="max">Maximum valid user value.</param>
        /// <param name="handleUserInput">The function which handles the user input.</param>
        public static void InitUserInputField(TextBox tb, string bindingPath, int min, int max, KeyEventHandler handleUserInput)
        {
            // The following code adds a binding with validation to the action input textbox.
            // (It was not possible in pure XAML because the ValidationRule
            // needs an argument and ValidationRule is not a DependencyObject thus no data binding available
            // to pass in the argument.)
            tb.KeyDown += handleUserInput;
            Binding binding = new Binding(bindingPath)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            binding.ValidationRules.Add(new UserInputValidationRule(min, max));
            tb.SetBinding(TextBox.TextProperty, binding);
            tb.MaxLength = Digits.GetAmountOfDigits(max);
        }
    }
}