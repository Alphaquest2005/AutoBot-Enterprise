using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InventoryQS.Client.Entities
{
    public partial class InventoryItemsEx 
    {
              
        public bool IsTariffCodeNull
        {
            get
            {
                return TariffCode == null;
            }
        }


    }
}
