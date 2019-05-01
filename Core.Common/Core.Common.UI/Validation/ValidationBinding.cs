using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Validation;
using System.Windows.Markup;

namespace Core.Common.UI.Validation
{
/// <summary>
/// Binding that will automatically implement the validation
/// </summary>
public class ValidationBinding : BindingDecoratorBase
{
    public ValidationBinding()
        : base()
    {
        Binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
    }

    /// <summary>
    /// This method is being invoked during initialization.
    /// </summary>
    /// <param name="provider">Provides access to the bound items.</param>
    /// <returns>The binding expression that is created by the base class.</returns>
    public override object ProvideValue(IServiceProvider provider)
    { 
        // Get the binding expression
        var bindingExpression = base.ProvideValue(provider);
        
        // Bound items
        DependencyObject targetObject;
        DependencyProperty targetProperty;

       


        // Try to get the bound items
        if (TryGetTargetItems(provider, out targetObject, out targetProperty))
        {
            if (targetObject == null)
            {
                return this;
            }
            if (targetObject is FrameworkElement)
            {
                // Get the element and implement dataconText changes
                var element = targetObject as FrameworkElement;
                element.DataContextChanged += new DependencyPropertyChangedEventHandler(element_DataContextChanged);

                //can't wait for event
                GetValidationRulesFromDataContext(element.DataContext);

                // Set the template
                var controlTemplate = element.TryFindResource("validationTemplate") as ControlTemplate;
                if (controlTemplate != null)
                    System.Windows.Controls.Validation.SetErrorTemplate(element, controlTemplate);
            }
        }

        // Go on with the flow
        return bindingExpression;
    }

    /// <summary>
    /// DataconText of the control has changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void element_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var dataconText = e.NewValue;
        GetValidationRulesFromDataContext(dataconText);
    }

    private void GetValidationRulesFromDataContext(object dataconText)
    {
        if (dataconText != null)
        {
            var property = dataconText.GetType().GetProperty(Binding.Path.Path);
            if (property != null)
            {
                var attributes = property.GetCustomAttributes(true).Where(o => o is IValidationRule);
                foreach (IValidationRule validationRule in attributes)
                    ValidationRules.Add(new GenericValidationRule(validationRule));
            }
        }
    }
}
}