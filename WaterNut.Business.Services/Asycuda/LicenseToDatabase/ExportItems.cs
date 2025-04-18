using Asycuda421; // Assuming LicenceLic_item_segment is here
using System;
using System.Collections.ObjectModel;
using LicenseDS.Business.Entities; // Assuming xLIC_License is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        private void ExportItems(ObservableCollection<LicenceLic_item_segment> aLic, xLIC_License lic)
        {
            try
            {
                foreach (var ditem in lic.xLIC_Lic_item_segment) // Potential NullReferenceException
                {
                   var aitem = new LicenceLic_item_segment();
                        aLic.Add(aitem);

                    // Potential NullReferenceExceptions on accessing ditem properties
                    aitem.Description = ditem.Description;
                    aitem.Commodity_code = ditem.Commodity_code;
                    aitem.Quantity_requested = Convert.ToInt32(ditem.Quantity_requested).ToString(); // FormatException
                    aitem.Origin = ditem.Origin;
                    aitem.Unit_of_measurement = ditem.Unit_of_measurement;
                    aitem.Quantity_to_approve = Convert.ToInt32(ditem.Quantity_to_approve).ToString(); // FormatException
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}