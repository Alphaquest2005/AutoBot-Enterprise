using System.Collections.Generic;
using System.ComponentModel;

namespace Core.Common.UI.DataVirtualization
{
    public interface IVirtualListLoader<T>
    {
        bool CanSort { get; }
        IList<T> LoadRange(int startIndex, int count, SortDescriptionCollection sortDescriptions, out int overallCount);
    }
}