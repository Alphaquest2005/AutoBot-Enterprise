using CoreEntities.Business.Entities; // Assuming TODO_C71ToXML is here
using EntryDataDS.Business.Entities; // Assuming Suppliers is here
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TrackableEntities; // Assuming TrackingState is here
using ValuationDS.Business.Entities; // Assuming xC71_Value_declaration_form, xC71_Identification_segment, xC71_Seller_segment, xC71_Buyer_segment, xC71_Declarant_segment, xC71_Item are here
using WaterNut.DataSpace; // Assuming BaseDataModel is here

namespace WaterNut.DataSpace.Asycuda
{
    public partial class C71ToDataBase
    {
        public xC71_Value_declaration_form CreateC71(Suppliers supplier, List<TODO_C71ToXML> lst,
            string docRef, string consigneeCode, string consigneeName, string consigneeAddress) // Added parameters
        {
            try
            {
                // This calls CreateNewC71, which needs to be in its own partial class
                var c71 = CreateNewC71();

                // Set Seller Info
                c71.xC71_Identification_segment.xC71_Seller_segment.Name = // Potential NullReferenceException
                    supplier.SupplierName ?? supplier.SupplierCode; // Potential NullReferenceException
                c71.xC71_Identification_segment.xC71_Seller_segment.Address = $"Ref:{docRef},\r\n{supplier.Street}"; // Potential NullReferenceException
                c71.xC71_Identification_segment.xC71_Seller_segment.CountryCode = supplier.CountryCode; // Potential NullReferenceException

                // Set Buyer (Consignee) Info using passed parameters
                if (!string.IsNullOrEmpty(consigneeName))
                {
                    // Ensure Buyer segment exists (it should from CreateNewC71)
                    if (c71.xC71_Identification_segment.xC71_Buyer_segment == null) // Potential NullReferenceException
                        c71.xC71_Identification_segment.xC71_Buyer_segment = new xC71_Buyer_segment(true); // Potential NullReferenceException

                    c71.xC71_Identification_segment.xC71_Buyer_segment.Code = consigneeCode; // Potential NullReferenceException
                    c71.xC71_Identification_segment.xC71_Buyer_segment.Name = consigneeName; // Potential NullReferenceException
                    c71.xC71_Identification_segment.xC71_Buyer_segment.Address = consigneeAddress ?? ""; // Potential NullReferenceException
                    // Code might need to be fetched similarly if required, currently not set.
                }

                // Set Declarant & Other Fields
                c71.xC71_Identification_segment.xC71_Declarant_segment.Code = BaseDataModel.Instance.CurrentApplicationSettings.Declarants.First(x => x.IsDefault == true).DeclarantCode; // Assuming Declarants exists, InvalidOperationException if no default
                c71.xC71_Identification_segment.No_7A = true; // Potential NullReferenceException
                c71.xC71_Identification_segment.No_8A = true; // Potential NullReferenceException
                c71.xC71_Identification_segment.No_9A = true; // Potential NullReferenceException
                c71.xC71_Identification_segment.No_9B = true; // Potential NullReferenceException

                foreach (var item in lst)
                {
                    c71.xC71_Item.Add(new xC71_Item(true) // Potential NullReferenceException
                    {
                        Terms_of_Delivery_Code = item.Code ?? "FOB",
                        Invoice_Number = item.InvoiceNo,
                        Invoice_Date = item.InvoiceDate.ToShortDateString(), // Potential NullReferenceException
                        Currency_code_net = item.Currency,
                        Net_Price = item.InvoiceTotal.ToString(CultureInfo.InvariantCulture), // Potential NullReferenceException

                        TrackingState = TrackingState.Added
                    });
                }

                return c71;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}