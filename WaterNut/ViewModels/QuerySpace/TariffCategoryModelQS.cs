using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using InventoryQS.Client.Repositories;
using InventoryQS.Client.Entities;

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