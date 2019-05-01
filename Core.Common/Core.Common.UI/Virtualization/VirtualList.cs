using System;
using System.Linq;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;

namespace Core.Common.UI.DataVirtualization
{
    internal interface IVirtualList
    {
        QueuedBackgroundWorkerState LoadingState { get; }
        Exception LastLoadingError { get; }
        event EventHandler LoadingStateChanged;
        void RetryLoading();
    }

    public partial class VirtualList<T> : IDisposable, IVirtualList, IList<VirtualListItem<T>>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public const int DefaultPageSize = 25;
        private static readonly NotifyCollectionChangedEventArgs _collectionReset = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        IVirtualListLoader<T> _loader;
        int _version;
        int _pageSize;
        VirtualListItem<T>[] _list;
        QueuedBackgroundWorker<int> _pageRequests;
        readonly SynchronizationContext _synchronizationContext;
        

        public VirtualList(IVirtualListLoader<T> loader)
            : this(loader, DefaultPageSize, SynchronizationContext.Current)
        {
        }

        public VirtualList(IVirtualListLoader<T> loader, SynchronizationContext synchronizationContext)
            : this(loader, DefaultPageSize, synchronizationContext)
        {
        }

        public VirtualList(IVirtualListLoader<T> loader, int pageSize, SynchronizationContext synchronizationContext)
        {
            

            if (loader == null)
                throw new ArgumentNullException("loader");
            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException("pageSize");

            _synchronizationContext = synchronizationContext;
            _pageRequests = new QueuedBackgroundWorker<int>(LoadPage, synchronizationContext);
            _pageRequests.StateChanged += new EventHandler(OnPageRequestsStateChanged);
            

            _version++;
            _loader = loader;
            _pageSize = pageSize;
            LoadAsync(0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _pageRequests.Clear();
        }

        public void Refresh()
        {
            ThrowIfDeferred();
            _list = null;
           SetCurrent(null, -1);
           // LoadAsync(0);
            LoadRange(0, _pageSize);
     

        }

        public QueuedBackgroundWorkerState LoadingState
        {
            get { return _pageRequests.State; }
        }

        public Exception LastLoadingError
        {
            get { return _pageRequests.LastError; }
        }

        void OnPageRequestsStateChanged(object sender, EventArgs e)
        {
            if (LoadingStateChanged != null)
                LoadingStateChanged(this, EventArgs.Empty);
        }

        public event EventHandler LoadingStateChanged;

        public void RetryLoading()
        {
            if (LoadingState == QueuedBackgroundWorkerState.StoppedByError)
                _pageRequests.Retry();
        }

        private void PopulatePageData(int startIndex, IList<T> pageData, int overallCount)
        {
            var flagRefresh = false;
            if (_list == null || _list.Length != overallCount || overallCount == 0)
            {
                _list = new VirtualListItem<T>[overallCount];
                flagRefresh = true;
                
            }
            for (var i = 0; i < pageData.Count; i++)
            {
                var index = startIndex + i;
                if (index >= _list.Count()) continue;
                if (_list.Count() == index || _list[index] == null)
                {
                    _list[index] = new VirtualListItem<T>(this, index, pageData[i]);
                    
                }
                else
                    _list[index].Data = pageData[i];
            }
            if (flagRefresh)
            {
                if (_synchronizationContext == null || SynchronizationContext.Current != null)
                    FireCollectionReset(null);
                else
                    _synchronizationContext.Post(FireCollectionReset, null);
            }
        }

        private void FireCollectionReset(object arg)
        {
            SetCurrent(null, -1);
            OnCollectionReset();
        }

        internal void Load(int index)
        {
            var startIndex = index - (index % _pageSize);
            LoadRange(startIndex, _pageSize);
        }

        private void LoadPage(int pageIndex)
        {
            var startIndex = pageIndex * _pageSize;
            LoadRange(startIndex, _pageSize);
        }

        private void LoadRange(int startIndex, int count)
        {
            int overallCount;
            var result = _loader.LoadRange(startIndex, count, SortDescriptions, out overallCount);
            PopulatePageData(startIndex, result, overallCount);
        }

        internal void LoadAsync(int index)
        {
            var pageIndex = index / _pageSize;
            _pageRequests.Add(pageIndex);
        }

        internal int Version
        {
            get { return _version; }
        }

        public bool Contains(VirtualListItem<T> item)
        {
            return IndexOf(item) != -1;
        }

        public int IndexOf(VirtualListItem<T> item)
        {
            return item == null || item.List != this ? -1 : item.Index;
        }

        public void CopyTo(VirtualListItem<T>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (arrayIndex >= array.Length)
                throw new ArgumentException("arrayIndex is greater or equal than the array length");
            if (arrayIndex + Count > array.Length)
                throw new ArgumentException("Number of elements in list is greater than available space");
            foreach (var item in this)
                array[arrayIndex++] = item;
        }

        public int Count
        {
            get { return _list == null ? 0 : _list.Length; }
        }

        public VirtualListItem<T> this[int index]
        {
            get
            {
                try
                {
                    if (_list != null && _list[index] == null)// && _list.Length <= index
                                        _list[index] = new VirtualListItem<T>(this, index);
                                    return _list != null ? _list[index] : null;
                }
                catch (Exception)
                {
                    throw;
                }
                 
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        #region IList<VirtualListItem<T>> Members

        void IList<VirtualListItem<T>>.Insert(int index, VirtualListItem<T> item)
        {
            throw new NotSupportedException();
        }

        void IList<VirtualListItem<T>>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICollection<VirtualListItem<T>> Members

        void ICollection<VirtualListItem<T>>.Add(VirtualListItem<T> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<VirtualListItem<T>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<VirtualListItem<T>>.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<VirtualListItem<T>>.Remove(VirtualListItem<T> item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<VirtualListItem<T>> Members

        IEnumerator<VirtualListItem<T>> IEnumerable<VirtualListItem<T>>.GetEnumerator()
        {
            for (var i = 0; i < Count; i++)
                yield return this[i];
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < 0; i++)// Count
                yield return this[i];
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        event PropertyChangedEventHandler PropertyChanged;
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { PropertyChanged += value; }
            remove { PropertyChanged -= value; }
        }

        #endregion

        #region INotifyCollectionChanged Members

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        void OnCollectionReset()
        {
            OnCollectionChanged(_collectionReset);
        }


        event NotifyCollectionChangedEventHandler CollectionChanged;
        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add { CollectionChanged += value; }
            remove { CollectionChanged -= value; }
        }

        #endregion

        public void LoadAll()
        {
           LoadRange(0, Count);
        }
    }
}
