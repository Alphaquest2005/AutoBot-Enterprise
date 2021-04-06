using System;
using System.Collections.Generic;
using System.Threading;

namespace Core.Common.UI.DataVirtualization
{
    public enum QueuedBackgroundWorkerState
    {
        Standby,
        Processing,
        StoppedByError
    }

    /// <summary>Executes queued operations on a separate thread.</summary>
    /// <typeparam name="T">Type of work item argument provided to DoWork callback.</typeparam>
    /// <remarks>This class is thread safe.</remarks>
    public sealed class QueuedBackgroundWorker<T> : IDisposable
    {
        private readonly SynchronizationContext _synchronizationContext;
        private QueuedBackgroundWorkerState _currentState = QueuedBackgroundWorkerState.Standby;
        private readonly Action<T> _doWorkCallback;
        private bool _flagClear;
        private AutoResetEvent _processingWaitSignal;
        private readonly Queue<T> _queue = new Queue<T>();

        public QueuedBackgroundWorker(Action<T> doWorkCallback)
            : this(doWorkCallback, SynchronizationContext.Current)
        {
        }

        public QueuedBackgroundWorker(Action<T> doWorkCallback, SynchronizationContext synchronizationContext)
        {
            if (doWorkCallback == null)
                throw new ArgumentNullException("doWorkCallback");

            _doWorkCallback = doWorkCallback;
            _synchronizationContext = synchronizationContext;
        }

        public QueuedBackgroundWorkerState State
        {
            get => _currentState;
            set
            {
                if (_currentState == value)
                    return;

                _currentState = value;
                if (value == QueuedBackgroundWorkerState.Processing)
                {
                    _processingWaitSignal = new AutoResetEvent(false);
                    _flagClear = false;
                    LastError = null;
                    ThreadPool.QueueUserWorkItem(Process);
                }
                else
                {
                    if (_processingWaitSignal != null)
                    {
                        _processingWaitSignal.Set();
                        _processingWaitSignal = null;
                    }
                }

                OnStateChanged();
            }
        }

        public Exception LastError { get; private set; }

        public void Dispose()
        {
            Clear();
            GC.SuppressFinalize(this);
        }

        private void OnStateChanged()
        {
            if (_synchronizationContext == null || SynchronizationContext.Current != null)
                RaiseStateChangedEvent(null);
            else
                _synchronizationContext.Post(RaiseStateChangedEvent, null);
        }

        private void RaiseStateChangedEvent(object args)
        {
            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        public event EventHandler StateChanged;

        public void Add(T workItem)
        {
            lock (this)
            {
                if (_queue.Contains(workItem))
                    return;
                _queue.Enqueue(workItem);
                if (State == QueuedBackgroundWorkerState.Standby)
                    State = QueuedBackgroundWorkerState.Processing;
            }
        }

        private void Process(object arg)
        {
            while (true)
            {
                T workItem;
                lock (this)
                {
                    workItem = _queue.Peek();
                }

                try
                {
                    _doWorkCallback(workItem);
                }
                catch (Exception ex)
                {
                    lock (this)
                    {
                        LastError = ex;
                        State = QueuedBackgroundWorkerState.StoppedByError;
                    }

                    return;
                }

                lock (this)
                {
                    if (_flagClear)
                        _queue.Clear();
                    else
                        _queue.Dequeue();

                    if (_queue.Count == 0)
                    {
                        State = QueuedBackgroundWorkerState.Standby;
                        return;
                    }
                }
            }
        }

        public void Clear()
        {
            AutoResetEvent waitSignal = null;
            lock (this)
            {
                if (State == QueuedBackgroundWorkerState.Processing)
                {
                    _flagClear = true;
                    waitSignal = _processingWaitSignal;
                }
                else
                {
                    _queue.Clear();
                    State = QueuedBackgroundWorkerState.Standby;
                }
            }

            // Wait for the completion of currently processing work item
            if (waitSignal != null)
                waitSignal.WaitOne();
        }

        public void Retry()
        {
            lock (this)
            {
                if (State == QueuedBackgroundWorkerState.StoppedByError)
                    State = QueuedBackgroundWorkerState.Processing;
            }
        }
    }
}