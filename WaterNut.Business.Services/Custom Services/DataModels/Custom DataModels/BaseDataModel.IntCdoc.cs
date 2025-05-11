using System;
using System.Collections.Generic;
using System.Linq;
using DocumentDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Entities;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    public void IntCdoc(DocumentCT cdoc, AsycudaDocumentSet ads, string prefix = "F")
    {
        cdoc.Document.xcuda_Declarant.Number = ads.Declarant_Reference_Number + $"-{prefix}" +
                                               cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.FileNumber;
        cdoc.Document.xcuda_Declarant.Declarant_code = BaseDataModel.Instance.CurrentApplicationSettings.Declarants
            .First(x => x.IsDefault).DeclarantCode;
        cdoc.Document.xcuda_Identification.Manifest_reference_number = ads.Manifest_Number;
        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = ads.AsycudaDocumentSetId;


        cdoc.Document.xcuda_Identification.xcuda_Type.Declaration_gen_procedure_code =
            ads.Customs_Procedure.Document_Type.Declaration_gen_procedure_code;
        cdoc.Document.xcuda_Identification.xcuda_Type.Type_of_declaration =
            ads.Customs_Procedure.Document_Type.Type_of_declaration;
        cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination =
            ads.Country_of_origin_code;
        cdoc.Document.xcuda_General_information.xcuda_Country.Trading_country = ads.Country_of_origin_code;
        cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_code =
            ads.Country_of_origin_code;

        cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_rate = Convert.ToSingle(ads.Exchange_Rate);
        cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = ads.Currency_Code;

        if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId !=
            ads.Customs_Procedure.Customs_ProcedureId)
        {
            if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId != 0)
            {
                var c = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure;
                foreach (var item in cdoc.DocumentItems.Where(x =>
                             x.xcuda_Tarification.Extended_customs_procedure == c.Extended_customs_procedure &&
                             x.xcuda_Tarification.National_customs_procedure == c.National_customs_procedure).ToList())
                {
                    item.xcuda_Tarification.Extended_customs_procedure =
                        ads.Customs_Procedure.Extended_customs_procedure;
                    item.xcuda_Tarification.National_customs_procedure =
                        ads.Customs_Procedure.National_customs_procedure;
                }
            }

            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = ads.Customs_ProcedureId;
            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure = ads.Customs_Procedure;
        }


        if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber != ads.BLNumber)
        {
            if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber != null)
            {
                var b = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber;
                foreach (var item in cdoc.DocumentItems
                             .Where(x => x.xcuda_Previous_doc.Summary_declaration == b).ToList())
                    item.xcuda_Previous_doc.Summary_declaration = ads.BLNumber;
            }

            cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber = ads.BLNumber;
        }


        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Description = ads.Description;
        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed = false;
        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AutoUpdate = false;
        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate = false;
        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.ImportComplete = false;
        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Cancelled = false;


        var Exp = BaseDataModel.Instance.ExportTemplates
            .Where(x => x.ApplicationSettingsId ==
                        cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet
                            .ApplicationSettingsId)
            .First(x =>
                x.Customs_Procedure ==
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.Customs_Procedure.CustomsProcedure);

        cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Destination.Destination_country_code =
            Exp.Destination_country_code;
        cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_region =
            Exp.Trading_country;
        if (string.IsNullOrEmpty(ads.Currency_Code))
            cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice.Currency_code = Exp.Gs_Invoice_Currency_code;
        if (string.IsNullOrEmpty(ads.Country_of_origin_code))
        {
            cdoc.Document.xcuda_General_information.xcuda_Country.Trading_country = Exp.Trading_country;
            cdoc.Document.xcuda_General_information.xcuda_Country.Country_first_destination =
                Exp.Country_first_destination;

            cdoc.Document.xcuda_General_information.xcuda_Country.xcuda_Export.Export_country_code =
                Exp.Export_country_code;
        }

        if (Exp.Delivery_terms_Code != null)
        {
            if (cdoc.Document.xcuda_Transport == null)
                cdoc.Document.xcuda_Transport = new List<xcuda_Transport>();
            var deliveryTerms = cdoc.Document.xcuda_Transport.FirstOrDefault()?.xcuda_Delivery_terms
                .FirstOrDefault();
            if (deliveryTerms == null)
                cdoc.Document.xcuda_Transport.Add(new xcuda_Transport(true)
                {
                    xcuda_Delivery_terms = new List<xcuda_Delivery_terms>
                    {
                        new xcuda_Delivery_terms(true)
                        {
                            Code = Exp.Delivery_terms_Code,
                            TrackingState = TrackingState.Added
                        }
                    },
                    TrackingState = TrackingState.Added
                });
            else
                deliveryTerms.Code = Exp.Delivery_terms_Code;
        }

        cdoc.Document.xcuda_Traders.xcuda_Consignee.Consignee_name =
            $"{ads.Consignee?.ConsigneeName ?? Exp.Consignee_name}\r\n{ads.Consignee?.Address ?? Exp.Consignee_Address}";
        cdoc.Document.xcuda_Traders.xcuda_Consignee.Consignee_code = ads.Consignee?.ConsigneeCode ?? Exp.Consignee_code;
    }
}