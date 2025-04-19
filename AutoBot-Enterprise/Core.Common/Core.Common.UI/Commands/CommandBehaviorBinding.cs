﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace AttachedCommandBehavior
{
    /// <summary>
    ///     Defines the command behavior binding
    /// </summary>
    public class CommandBehaviorBinding : IDisposable
    {
        //Creates an EventHandler on runtime and registers that handler to the Event specified
        public void BindEvent(DependencyObject owner, string eventName)
        {
            EventName = eventName;
            Owner = owner;
            Event = Owner.GetType().GetEvent(EventName, BindingFlags.Public | BindingFlags.Instance);
            if (Event == null)
                throw new InvalidOperationException($"Could not resolve event name {EventName}");

            //Create an event handler for the event that will call the ExecuteCommand method
            EventHandler = EventHandlerGenerator.CreateDelegate(
                Event.EventHandlerType,
                typeof(CommandBehaviorBinding).GetMethod("ExecuteCommand", BindingFlags.Public | BindingFlags.Instance),
                this);
            //Register the handler to the Event
            Event.AddEventHandler(Owner, EventHandler);
        }

        /// <summary>
        ///     Executes the command
        /// </summary>
        public void ExecuteCommand()
        {
            if (Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }

        #region Properties

        /// <summary>
        ///     Get the owner of the CommandBinding ex: a Button
        ///     This property can only be set from the BindEvent Method
        /// </summary>
        public DependencyObject Owner { get; private set; }

        /// <summary>
        ///     The command to execute when the specified event is raised
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        ///     Gets or sets a CommandParameter
        /// </summary>
        public object CommandParameter { get; set; }

        /// <summary>
        ///     The event name to hook up to
        ///     This property can only be set from the BindEvent Method
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        ///     The event info of the event
        /// </summary>
        public EventInfo Event { get; private set; }

        /// <summary>
        ///     Gets the EventHandler for the binding with the event
        /// </summary>
        public Delegate EventHandler { get; private set; }

        #endregion

        #region IDisposable Members

        private bool disposed;

        /// <summary>
        ///     Unregisters the EventHandler from the Event
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                Event.RemoveEventHandler(Owner, EventHandler);
                disposed = true;
            }
        }

        #endregion
    }
}