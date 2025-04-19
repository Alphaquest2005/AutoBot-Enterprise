using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.DataLayer
{
    public partial class xcuda_Tarification
    {
        public ObservableCollection<xcuda_Supplementary_unit> xcuda_Supplementary_unit
        {
            get
            {
                return new ObservableCollection<xcuda_Supplementary_unit>(Unordered_xcuda_Supplementary_unit.OrderBy(x => x.Supplementary_unit_Id));
            }
        }
    }
}
