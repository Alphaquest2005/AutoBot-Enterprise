using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
