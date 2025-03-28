﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace Core.Common.UI.Validation
{
    /// <summary>
    ///     A base class for custom markup extension which provides properties
    ///     that can be found on regular <see cref="Binding" /> markup extension.
    /// </summary>
    [MarkupExtensionReturnType(typeof(object))]
    public abstract class BindingDecoratorBase : MarkupExtension
    {
        /// <summary>
        ///     The decorated binding class.
        /// </summary>
        private Binding binding = new Binding();

        /// <summary>
        ///     This basic implementation just sets a binding on the targeted
        ///     <see cref="DependencyObject" /> and returns the appropriate
        ///     <see cref="BindingExpressionBase" /> instance.<br />
        ///     All this work is delegated to the decorated <see cref="Binding" />
        ///     instance.
        /// </summary>
        /// <returns>
        ///     The object value to set on the property where the extension is applied.
        ///     In case of a valid binding expression, this is a <see cref="BindingExpressionBase" />
        ///     instance.
        /// </returns>
        /// <param name="provider">
        ///     Object that can provide services for the markup
        ///     extension.
        /// </param>
        public override object ProvideValue(IServiceProvider provider)
        {
            //create a binding and associate it with the target
            return binding.ProvideValue(provider);
        }

        /// <summary>
        ///     Validates a service provider that was submitted to the <see cref="ProvideValue" />
        ///     method. This method checks whether the provider is null (happens at design time),
        ///     whether it provides an <see cref="IProvideValueTarget" /> service, and whether
        ///     the service's <see cref="IProvideValueTarget.TargetObject" /> and
        ///     <see cref="IProvideValueTarget.TargetProperty" /> properties are valid
        ///     <see cref="DependencyObject" /> and <see cref="DependencyProperty" />
        ///     instances.
        /// </summary>
        /// <param name="provider">The provider to be validated.</param>
        /// <param name="target">The binding target of the binding.</param>
        /// <param name="dp">The target property of the binding.</param>
        /// <returns>True if the provider supports all that's needed.</returns>
        protected virtual bool TryGetTargetItems(IServiceProvider provider, out DependencyObject target,
            out DependencyProperty dp)
        {
            target = null;
            dp = null;
            if (provider == null) return false;

            //create a binding and assign it to the target
            var service = (IProvideValueTarget) provider.GetService(typeof(IProvideValueTarget));
            if (service == null) return true;

            if (service.TargetObject.GetType().FullName == "System.Windows.SharedDp") return true;

            //we need dependency objects / properties
            target = service.TargetObject as DependencyObject;
            dp = service.TargetProperty as DependencyProperty;
            return target != null && dp != null;
        }

        // Check documentation of the Binding class for property information

        #region properties

        /// <summary>
        ///     The decorated binding class.
        /// </summary>
        [Browsable(false)]
        public Binding Binding
        {
            get => binding;
            set => binding = value;
        }


        [DefaultValue(null)]
        public object AsyncState
        {
            get => binding.AsyncState;
            set => binding.AsyncState = value;
        }

        [DefaultValue(false)]
        public bool BindsDirectlyToSource
        {
            get => binding.BindsDirectlyToSource;
            set => binding.BindsDirectlyToSource = value;
        }

        [DefaultValue(null)]
        public IValueConverter Converter
        {
            get => binding.Converter;
            set => binding.Converter = value;
        }

        [TypeConverter(typeof(CultureInfoIetfLanguageTagConverter))]
        [DefaultValue(null)]
        public CultureInfo ConverterCulture
        {
            get => binding.ConverterCulture;
            set => binding.ConverterCulture = value;
        }

        [DefaultValue(null)]
        public object ConverterParameter
        {
            get => binding.ConverterParameter;
            set => binding.ConverterParameter = value;
        }

        [DefaultValue(null)]
        public string ElementName
        {
            get => binding.ElementName;
            set => binding.ElementName = value;
        }

        [DefaultValue(null)]
        public object FallbackValue
        {
            get => binding.FallbackValue;
            set => binding.FallbackValue = value;
        }

        [DefaultValue(false)]
        public bool IsAsync
        {
            get => binding.IsAsync;
            set => binding.IsAsync = value;
        }

        [DefaultValue(BindingMode.Default)]
        public BindingMode Mode
        {
            get => binding.Mode;
            set => binding.Mode = value;
        }

        [DefaultValue(false)]
        public bool NotifyOnSourceUpdated
        {
            get => binding.NotifyOnSourceUpdated;
            set => binding.NotifyOnSourceUpdated = value;
        }

        [DefaultValue(false)]
        public bool NotifyOnTargetUpdated
        {
            get => binding.NotifyOnTargetUpdated;
            set => binding.NotifyOnTargetUpdated = value;
        }

        [DefaultValue(false)]
        public bool NotifyOnValidationError
        {
            get => binding.NotifyOnValidationError;
            set => binding.NotifyOnValidationError = value;
        }

        [DefaultValue(null)]
        public PropertyPath Path
        {
            get => binding.Path;
            set => binding.Path = value;
        }

        [DefaultValue(null)]
        public RelativeSource RelativeSource
        {
            get => binding.RelativeSource;
            set => binding.RelativeSource = value;
        }

        [DefaultValue(null)]
        public object Source
        {
            get => binding.Source;
            set => binding.Source = value;
        }

        [DefaultValue("")]
        public string StringFormat
        {
            get => binding.StringFormat;
            set => binding.StringFormat = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public UpdateSourceExceptionFilterCallback UpdateSourceExceptionFilter
        {
            get => binding.UpdateSourceExceptionFilter;
            set => binding.UpdateSourceExceptionFilter = value;
        }

        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get => binding.UpdateSourceTrigger;
            set => binding.UpdateSourceTrigger = value;
        }

        [DefaultValue(false)]
        public bool ValidatesOnDataErrors
        {
            get => binding.ValidatesOnDataErrors;
            set => binding.ValidatesOnDataErrors = value;
        }

        [DefaultValue(false)]
        public bool ValidatesOnExceptions
        {
            get => binding.ValidatesOnExceptions;
            set => binding.ValidatesOnExceptions = value;
        }

        [DefaultValue(null)]
        public string XPath
        {
            get => binding.XPath;
            set => binding.XPath = value;
        }

        [DefaultValue(null)] public Collection<ValidationRule> ValidationRules => binding.ValidationRules;

        #endregion
    }
}