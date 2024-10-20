﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Core.Common.UI.DataVirtualization
{
    /// <summary>
    ///     Base class of VirtualListItem&ltT&gt, should be named as VirtualListItem for better code readability
    ///     however this will crash Visual Studio WPF Designer.
    /// </summary>
    public abstract class VirtualListItemBase
    {
        public static readonly DependencyProperty AutoLoadProperty = DependencyProperty.RegisterAttached("AutoLoad",
            typeof(bool), typeof(VirtualListItemBase),
            new FrameworkPropertyMetadata(false, OnAutoLoadChanged));

        public abstract bool IsLoaded { get; }

        public object Data => GetData();

        private static void OnAutoLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dpd = DependencyPropertyDescriptor.FromProperty(ContentControl.ContentProperty,
                typeof(DependencyObject));
            if (dpd == null)
                return;

            var isEnabled = (bool) e.NewValue;
            if (isEnabled)
                dpd.AddValueChanged(d, OnContentChanged);
            else
                dpd.RemoveValueChanged(d, OnContentChanged);
        }

        private static void OnContentChanged(object sender, EventArgs e)
        {
            if (((DependencyObject) sender).GetValue(ContentControl.ContentProperty) is VirtualListItemBase item)
                item.LoadAsync();
        }

        public static bool GetAutoLoad(DependencyObject d)
        {
            return (bool) d.GetValue(AutoLoadProperty);
        }

        public static void SetAutoLoad(DependencyObject d, bool value)
        {
            d.SetValue(AutoLoadProperty, value);
        }

        internal abstract object GetData();

        public abstract void Load();

        public abstract void LoadAsync();
    }
}