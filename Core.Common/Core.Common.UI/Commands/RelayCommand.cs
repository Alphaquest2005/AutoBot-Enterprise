﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows.Input;

namespace Core.Common.UI
{
    //using Microsoft.Kinect.Toolkit.Properties;

    /// <summary>
    ///     Command that executes a delegate that takes no parameters.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        ///     Predicate determining whether this command can currently execute
        /// </summary>
        private readonly Func<bool> canExecuteDelegate;

        private EventHandler canExecuteEventhandler;

        /// <summary>
        ///     Delegate to be executed
        /// </summary>
        private readonly Action executeDelegate;

        /// <summary>
        ///     Initializes a new instance of the RelayCommand class with the provided delegate and predicate
        /// </summary>
        /// <param name="executeDelegate">Delegate to be executed</param>
        /// <param name="canExecuteDelegate">Predicate determining whether this command can currently execute</param>
        public RelayCommand(Action executeDelegate, Func<bool> canExecuteDelegate)
        {
            this.canExecuteDelegate = canExecuteDelegate;
            this.executeDelegate = executeDelegate ?? throw new ArgumentNullException(nameof(executeDelegate));
        }

        /// <summary>
        ///     Initializes a new instance of the RelayCommand class with the provided delegate
        /// </summary>
        /// <param name="executeDelegate">Delegate to be executed</param>
        public RelayCommand(Action executeDelegate)
            : this(executeDelegate, null)
        {
        }

        /// <summary>
        ///     Event signaling that the possibility of this command executing has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                canExecuteEventhandler += value;
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                canExecuteEventhandler -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        ///     Evaluates whether the command can currently execute
        /// </summary>
        /// <param name="parameter">ICommand required parameter that is ignored</param>
        /// <returns>True if the command can currently execute, false otherwise</returns>
        public bool CanExecute(object parameter)
        {
            if (null == canExecuteDelegate) return true;

            return canExecuteDelegate.Invoke();
        }

        /// <summary>
        ///     Executes the associated delegate
        /// </summary>
        /// <param name="parameter">ICommand required parameter that is ignored</param>
        public void Execute(object parameter)
        {
            executeDelegate.Invoke();
        }

        /// <summary>
        ///     Raises the CanExecuteChanged event to signal that the possibility of execution has changed
        /// </summary>
        public void InvokeCanExecuteChanged()
        {
            if (null != canExecuteDelegate)
            {
                var handler = canExecuteEventhandler;
                if (null != handler) handler(this, EventArgs.Empty);
            }
        }
    }

    public class RelayCommand<T> : ICommand where T : class
    {
        /// <summary>
        ///     Predicate determining whether this command can currently execute
        /// </summary>
        private readonly Predicate<T> canExecuteDelegate;

        private EventHandler canExecuteEventhandler;

        /// <summary>
        ///     Delegate to be executed
        /// </summary>
        private readonly Action<T> executeDelegate;

        /// <summary>
        ///     Initializes a new instance of the RelayCommand class with the provided delegate and predicate
        /// </summary>
        /// <param name="executeDelegate">Delegate to be executed</param>
        /// <param name="canExecuteDelegate">Predicate determining whether this command can currently execute</param>
        public RelayCommand(Action<T> executeDelegate, Predicate<T> canExecuteDelegate)
        {
            this.canExecuteDelegate = canExecuteDelegate;
            this.executeDelegate = executeDelegate ?? throw new ArgumentNullException(nameof(executeDelegate));
        }

        /// <summary>
        ///     Initializes a new instance of the RelayCommand class with the provided delegate
        /// </summary>
        /// <param name="executeDelegate">Delegate to be executed</param>
        public RelayCommand(Action<T> executeDelegate)
            : this(executeDelegate, null)
        {
        }

        /// <summary>
        ///     Event signaling that the possibility of this command executing has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                canExecuteEventhandler += value;
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                canExecuteEventhandler -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <summary>
        ///     Evaluates whether the command can currently execute
        /// </summary>
        /// <param name="parameter">Context of type T used for evaluating the current possibility of execution</param>
        /// <returns>True if the command can currently execute, false otherwise</returns>
        public bool CanExecute(object parameter)
        {
            if (null == parameter)
            {
                return true;
                throw new ArgumentNullException(nameof(parameter));
            }

            if (null == canExecuteDelegate) return true;

            if (!(parameter is T castParameter))
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture,
                    UIResources.DelegateCommandCastException, parameter.GetType().FullName, typeof(T).FullName));

            return canExecuteDelegate.Invoke(castParameter);
        }

        /// <summary>
        ///     Executes the associated delegate
        /// </summary>
        /// <param name="parameter">Parameter of type T passed to the associated delegate</param>
        public void Execute(object parameter)
        {
            if (null == parameter) throw new ArgumentNullException(nameof(parameter));

            if (!(parameter is T castParameter))
                throw new InvalidCastException(string.Format(CultureInfo.InvariantCulture,
                    UIResources.DelegateCommandCastException, parameter.GetType().FullName, typeof(T).FullName));

            executeDelegate.Invoke(castParameter);
        }

        /// <summary>
        ///     Raises the CanExecuteChanged event to signal that the possibility of execution has changed
        /// </summary>
        public void InvokeCanExecuteChanged()
        {
            if (null != canExecuteDelegate)
            {
                var handler = canExecuteEventhandler;
                if (null != handler) handler(this, EventArgs.Empty);
            }
        }
    }
}