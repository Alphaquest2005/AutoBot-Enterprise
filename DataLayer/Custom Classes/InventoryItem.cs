using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WaterNut.DataLayer
{
    public partial class InventoryItems : IHasEntryTimeStamp
    {
        public InventoryItems()
        {
          
        }

       
        public bool IsTariffCodeNull
        {
            get
            {
                return TariffCode == null;
            }
        }

        //public bool IsSalesItem
        //{
        //    get
        //    {
        //        return (SalesItms.FirstOrDefault() != null) ;
        //    }
        //}
    }
}
