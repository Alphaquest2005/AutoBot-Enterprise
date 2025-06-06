using Asycuda421; // Assuming LicenceLic_item_segment is here
using System;
using System.Collections.ObjectModel;
using System.Linq;
using LicenseDS.Business.Entities; // Assuming xLIC_License, xLIC_Lic_item_segment are here
using TrackableEntities; // Assuming TrackingState is here

namespace WaterNut.DataSpace.Asycuda
{
    // Assuming this is part of the same partial class C71ToDataBase as the previous file
    public partial class C71ToDataBase
    {
        // Assuming 'da' is a field accessible across partial classes
        // private LicenseDS.Business.Entities.Registered da;

        private void SaveItems(ObservableCollection<LicenceLic_item_segment> aItems, xLIC_License dl)
        {
            try
            {
                foreach (var aitem in aItems)
                {
                    // Potential NullReferenceException if dl or xLIC_Lic_item_segment is null
                    var ditem = dl.xLIC_Lic_item_segment.FirstOrDefault(x =>
                        x.Commodity_code == aitem.Commodity_code && x.Description == aitem.Description);
                    if (ditem == null)
                    {
                        ditem = new xLIC_Lic_item_segment(true)
                        {
                            xLIC_License = dl,
                            TrackingState = TrackingState.Added
                        };
                        dl.xLIC_Lic_item_segment.Add(ditem); // Potential NullReferenceException
                    }

                    // Potential NullReferenceExceptions on accessing aitem properties
                    ditem.Description = aitem.Description;
                    ditem.Commodity_code = aitem.Commodity_code;
                    ditem.Quantity_requested = Convert.ToDouble(aitem.Quantity_requested); // FormatException
                    ditem.Origin = aitem.Origin;
                    ditem.Unit_of_measurement = aitem.Unit_of_measurement;
                    ditem.Quantity_to_approve = Convert.ToDouble(aitem.Quantity_to_approve); // FormatException
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