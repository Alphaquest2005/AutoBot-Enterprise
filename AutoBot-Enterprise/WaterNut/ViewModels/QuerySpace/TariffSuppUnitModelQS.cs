using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using InventoryQS.Client.Entities;
using InventoryQS.Client.Repositories;


namespace WaterNut.QuerySpace.InventoryQS.ViewModels
{
    public partial class TariffSuppUnitModel : TariffSupUnitLkpsViewModel_AutoGen  
	{
     private static readonly TariffSuppUnitModel instance;
         static TariffSuppUnitModel()
        {
            instance = new TariffSuppUnitModel();
        }

         public static TariffSuppUnitModel Instance
        {
            get { return instance; }
        }

         private TariffSuppUnitModel()
        {
            
        }

         internal async Task SaveTariffSupUnitLkps(TariffSupUnitLkps tariffSupUnitLkps)
         {
             throw new NotImplementedException();
         }
    }
}