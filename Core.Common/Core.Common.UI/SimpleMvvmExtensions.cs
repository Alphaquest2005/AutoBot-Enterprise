using System;
using SimpleMvvmToolkit;
using System.Linq.Expressions;
using System.ComponentModel;

namespace SimpleMvvmExtensions
{
    internal static class BindingHelper
    {
        // Defined as virtual so you can override if you wish
        public static void NotifyPropertyChanged<TModel, TResult>
            (this TModel model, Expression<Func<TModel, TResult>> property,
            PropertyChangedEventHandler propertyChanged)
        {
            // Convert expression to a property name
            var propertyName = ((MemberExpression)property.Body).Member.Name;

            // Fire notify property changed event
            InternalNotifyPropertyChanged(propertyName, model, propertyChanged);
        }

        public static void InternalNotifyPropertyChanged(string propertyName,
            object sender, PropertyChangedEventHandler propertyChanged)
        {
            if (propertyChanged != null)
            {
                // Always fire the event on the UI thread
                if (UIDispatcher.Current.CheckAccess())
                {
                    propertyChanged(sender, new PropertyChangedEventArgs(propertyName));

                }
                else
                {
                    Action action = () => propertyChanged
                        (sender, new PropertyChangedEventArgs(propertyName));
                    UIDispatcher.Current.BeginInvoke(action);
                }
            }
        }
    }
}