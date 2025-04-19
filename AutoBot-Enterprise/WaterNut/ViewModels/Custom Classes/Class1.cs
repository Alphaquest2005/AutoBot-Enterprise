using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.QuerySpace.CoreEntities.ViewModels
{
    public partial class AsycudaDocumentItemViewModelAutoGen
    {
        private int? _asycudaDocumentIdFilter;
        public int? AsycudaDocumentIdFilter
        {
            get
            {
                return _asycudaDocumentIdFilter;
            }
            set
            {
                _asycudaDocumentIdFilter = value;
                NotifyPropertyChanged(x => AsycudaDocumentIdFilter);
                FilterData();

            }
        }
    }
}
