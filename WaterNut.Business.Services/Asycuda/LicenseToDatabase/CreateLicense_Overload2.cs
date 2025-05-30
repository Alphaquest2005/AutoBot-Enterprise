using CoreEntities.Business.Entities; // Assuming TODO_LicenseToXML, Contacts are here
using EntryDataDS.Business.Entities; // Assuming Suppliers is here
using System;
using System.Collections.Generic;
using System.Linq;
using LicenseDS.Business.Entities; // Assuming xLIC_License, xLIC_General_segment, xLIC_Lic_item_segment are here
using TrackableEntities; // Assuming TrackingState is here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class LicenseToDataBase
    {
        public xLIC_License CreateLicense(List<TODO_LicenseToXML> lst, Contacts contact, Suppliers supplier,
            string docRef, string consigneeCode,string consigneeName, string consigneeAddress)
        {
            // This calls CreateLicense(), which needs to be in its own partial class
            var lic = CreateLicense();
            lic.xLIC_General_segment.Application_date = DateTime.Now.Date.ToShortDateString(); // Potential NullReferenceException
            lic.xLIC_General_segment.Importation_date = DateTime.Now.Date.AddMonths(3).ToShortDateString(); // Potential NullReferenceException
            lic.xLIC_General_segment.Arrival_date = DateTime.Now.Date.ToShortDateString(); // Potential NullReferenceException

            lic.xLIC_General_segment.Exporter_address = // Potential NullReferenceException
                $"Ref:{docRef}\r\n{supplier?.Street}\r\n{supplier?.City},{supplier?.CountryCode}\r\n{supplier?.Country}";
            lic.xLIC_General_segment.Exporter_name = supplier?.SupplierName?? supplier?.SupplierCode; // Potential NullReferenceException
            lic.xLIC_General_segment.Exporter_country_code = supplier?.CountryCode; // Potential NullReferenceException

            lic.xLIC_General_segment.Importer_code = consigneeCode; //BaseDataModel.Instance.CurrentApplicationSettings.Declarants.First(x => x.IsDefault == true).DeclarantCode; // Potential NullReferenceException
            lic.xLIC_General_segment.Importer_name = $"{consigneeName}\r\n{consigneeAddress}"; //BaseDataModel.Instance.CurrentApplicationSettings.Declarants.First(x => x.IsDefault == true).DeclarantName; // Potential NullReferenceException
            lic.xLIC_General_segment.Importer_contact = contact.Name; // Potential NullReferenceException
            lic.xLIC_General_segment.Importer_cellphone = contact.CellPhone; // Potential NullReferenceException
            lic.xLIC_General_segment.Importer_email = contact.EmailAddress; // Potential NullReferenceException

            foreach (var item in lst.GroupBy(x => new{ x.TariffCode, x.LicenseDescription}))
            {
                lic.xLIC_Lic_item_segment.Add(new xLIC_Lic_item_segment(true) // Potential NullReferenceException
                {
                    Description = item.Key.LicenseDescription,
                    Commodity_code = item.Key.TariffCode,
                    Quantity_requested = Convert.ToInt32(Math.Ceiling((double)item.Sum(x => x.VolumeLiters == 0 ? x.Quantity : x.VolumeLiters))), // Potential NullReferenceException
                    Quantity_to_approve = Convert.ToInt32(Math.Ceiling((double)item.Sum(x => x.VolumeLiters == 0 ? x.Quantity : x.VolumeLiters))), // Potential NullReferenceException
                    Origin = item.First().Country_of_origin_code, // InvalidOperationException if item is empty
                    Unit_of_measurement = item.First().UOM, // InvalidOperationException if item is empty
                    TrackingState = TrackingState.Added
                });
            }

            return lic;
        }
    }
}