﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Core.Common.UI.DataVirtualization
{
    public sealed class VirtualListLoadingIndicator : Control
    {
        public static readonly RoutedCommand RetryCommand =
            new RoutedCommand("Retry", typeof(VirtualListLoadingIndicator));

        private static readonly DependencyPropertyKey HasErrorPropertyKey = DependencyProperty.RegisterReadOnly(
            "HasError", typeof(bool), typeof(VirtualListLoadingIndicator),
            new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey ErrorMessagePropertyKey = DependencyProperty.RegisterReadOnly(
            "ErrorMessage", typeof(string), typeof(VirtualListLoadingIndicator),
            new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ErrorMessageProperty = ErrorMessagePropertyKey.DependencyProperty;

        private IVirtualList _list;
        private QueuedBackgroundWorkerState _loadingState = QueuedBackgroundWorkerState.Standby;

        static VirtualListLoadingIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VirtualListLoadingIndicator),
                new FrameworkPropertyMetadata(typeof(VirtualListLoadingIndicator)));
            var retryCommandBinding = new CommandBinding(RetryCommand,
                OnRetryCommandExecuted,
                CanExecuteRetryCommand);
            CommandManager.RegisterClassCommandBinding(typeof(VirtualListLoadingIndicator), retryCommandBinding);
        }

        private VirtualListLoadingIndicator(IVirtualList list, QueuedBackgroundWorkerState loadingState)
        {
            SetState(list, loadingState);
        }

        public bool HasError
        {
            get => (bool) GetValue(HasErrorProperty);
            private set => SetValue(HasErrorPropertyKey, value);
        }

        public string ErrorMessage
        {
            get => (string) GetValue(ErrorMessageProperty);
            private set => SetValue(ErrorMessagePropertyKey, value);
        }

        private static void OnRetryCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((VirtualListLoadingIndicator) sender).RetryLoading();
        }

        private static void CanExecuteRetryCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            var control = (VirtualListLoadingIndicator) sender;
            e.CanExecute = control.HasError;
        }

        private void SetState(IVirtualList list, QueuedBackgroundWorkerState loadingState)
        {
            if (_list == list && _loadingState == loadingState)
                return;

            _list = list;
            _loadingState = loadingState;
            HasError = _loadingState == QueuedBackgroundWorkerState.StoppedByError;
            ErrorMessage = HasError ? _list.LastLoadingError.ToString() : null;
        }

        public void RetryLoading()
        {
            if (HasError)
                _list.RetryLoading();
        }

        #region IsAttached attached property

        private sealed class ItemsControlManager : IDisposable
        {
            private ItemsControl _itemsControl;
            private IVirtualList _list;

            public ItemsControlManager(ItemsControl itemsControl)
            {
                Debug.Assert(itemsControl != null);
                _itemsControl = itemsControl;
                SetState(itemsControl.ItemsSource as IVirtualList);
                var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty,
                    typeof(ItemsControl));
                dpd.AddValueChanged(itemsControl, OnItemsSourceChanged);
            }

            private IVirtualList List
            {
                get => _list;
                set
                {
                    if (_list == value)
                        return;

                    var oldValue = _list;
                    _list = value;

                    if (oldValue != null)
                        oldValue.LoadingStateChanged -= OnLoadingStateChanged;
                    if (value != null)
                        value.LoadingStateChanged += OnLoadingStateChanged;
                }
            }

            private QueuedBackgroundWorkerState LoadingState { get; set; } = QueuedBackgroundWorkerState.Standby;

            private VirtualListLoadingIndicator LoadingIndicator =>
                AdornerManager.GetAdorner(_itemsControl) as VirtualListLoadingIndicator;

            public void Dispose()
            {
                if (LoadingIndicator != null)
                    AdornerManager.SetAdorner(_itemsControl, null);

                if (_itemsControl != null)
                {
                    var dpd = DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty,
                        typeof(ItemsControl));
                    dpd.RemoveValueChanged(_itemsControl, OnItemsSourceChanged);
                    _itemsControl = null;
                }

                if (_list != null)
                {
                    _list.LoadingStateChanged -= OnLoadingStateChanged;
                    _list = null;
                }

                LoadingState = QueuedBackgroundWorkerState.Standby;
                GC.SuppressFinalize(this);
            }

            private void OnItemsSourceChanged(object sender, EventArgs e)
            {
                SetState(_itemsControl.ItemsSource as IVirtualList);
            }

            private void OnLoadingStateChanged(object sender, EventArgs e)
            {
                var list = (IVirtualList) sender;
                SetState(list.LoadingState);
            }

            private void SetState(IVirtualList list)
            {
                SetState(list, list == null ? QueuedBackgroundWorkerState.Standby : list.LoadingState);
            }

            private void SetState(QueuedBackgroundWorkerState loadingState)
            {
                SetState(_list, loadingState);
            }

            private void SetState(IVirtualList list, QueuedBackgroundWorkerState loadingState)
            {
                if (list == List && loadingState == LoadingState)
                    return;

                List = list;
                LoadingState = loadingState;
                var loadingIndicator = LoadingIndicator;
                var oldIsVisible = loadingIndicator != null;
                var newIsVisible = LoadingState != QueuedBackgroundWorkerState.Standby;
                if (oldIsVisible == newIsVisible)
                {
                    if (newIsVisible)
                        loadingIndicator.SetState(List, LoadingState);
                }
                else if (newIsVisible)
                {
                    AdornerManager.SetAdorner(_itemsControl, new VirtualListLoadingIndicator(List, LoadingState));
                }
                else
                {
                    AdornerManager.SetAdorner(_itemsControl, null);
                }
            }
        }

        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached("IsAttached",
            typeof(bool), typeof(VirtualListLoadingIndicator),
            new FrameworkPropertyMetadata(false, OnIsAttachedChanged));

        private static readonly DependencyProperty ItemsControlManagerProperty = DependencyProperty.RegisterAttached(
            "ItemsControlManager", typeof(ItemsControlManager), typeof(VirtualListLoadingIndicator),
            new FrameworkPropertyMetadata(null));

        public static bool GetIsAttached(DependencyObject d)
        {
            return (bool) d.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(DependencyObject d, bool value)
        {
            d.SetValue(IsAttachedProperty, value);
        }

        private static ItemsControlManager GetItemsControlManager(ItemsControl itemsControl)
        {
            return (ItemsControlManager) itemsControl.GetValue(ItemsControlManagerProperty);
        }

        private static void SetItemsControlManager(ItemsControl itemsControl, ItemsControlManager value)
        {
            itemsControl.SetValue(ItemsControlManagerProperty, value);
        }

        private static void ClearItemsControlManager(ItemsControl itemsControl)
        {
            itemsControl.ClearValue(ItemsControlManagerProperty);
        }

        private static void OnIsAttachedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ItemsControl itemsControl))
                return;
            var oldValue = (bool) e.OldValue;
            var newValue = (bool) e.NewValue;
            if (oldValue == newValue)
                return;

            if (newValue)
            {
                SetItemsControlManager(itemsControl, new ItemsControlManager(itemsControl));
            }
            else
            {
                var itemsControlManager = GetItemsControlManager(itemsControl);
                itemsControlManager.Dispose();
                ClearItemsControlManager(itemsControl);
            }
        }

        #endregion
    }
}