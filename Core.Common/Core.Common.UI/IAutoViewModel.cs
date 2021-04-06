using System.Collections.ObjectModel;
using Core.Common.UI.DataVirtualization;

namespace Core.Common.UI
{
    public interface IAutoViewModel<T>
    {
        VirtualList<T> VirtualData { get; set; }
        ObservableCollection<T> SelectedItems { get; set; }
        void ViewAll();
        void SelectAll();
    }
}