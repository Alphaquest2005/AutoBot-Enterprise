using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using InventoryQS.Client.Entities;
using InventoryQS.Client.Repositories;

namespace WaterNut.QuerySpace.InventoryQS.ViewModels
{
    public partial class TariffCategoryModel : TariffCategoryViewModel_AutoGen 
	{     
         private static readonly TariffCategoryModel instance;
         static TariffCategoryModel()
        {
            instance = new TariffCategoryModel();
        }

         public static TariffCategoryModel Instance
        {
            get { return instance; }
        }

         private TariffCategoryModel()
        {
            
        }
        
	}
}