using System;
using System.Linq;
using Core.Common.Utils;
using DocumentItemDS.Business.Entities;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using WaterNut.Business.Entities;
using AsycudaDocumentEntryData = DocumentDS.Business.Entities.AsycudaDocumentEntryData;

namespace WaterNut.DataSpace;

public partial class BaseDataModel
{
    internal xcuda_Item CreateItemFromEntryDataDetail(BaseDataModel.IEntryLineData pod, DocumentCT cdoc)
    {
        var itm = CreateNewDocumentItem();
        cdoc.DocumentItems.Add(itm);


        itm.xcuda_Goods_description.Commercial_Description = CleanText(pod.ItemDescription).Truncate(159);
        if (cdoc.Document.xcuda_General_information != null)
            itm.xcuda_Goods_description.Country_of_origin_code = cdoc.Document.xcuda_General_information
                .xcuda_Country.Country_first_destination;
        itm.xcuda_Tarification.Item_price = Convert.ToSingle(pod.Cost * pod.Quantity);
        itm.xcuda_Tarification.National_customs_procedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
            .Customs_Procedure
            .National_customs_procedure;
        itm.xcuda_Tarification.Extended_customs_procedure = cdoc.Document.xcuda_ASYCUDA_ExtendedProperties
            .Customs_Procedure
            .Extended_customs_procedure;

        itm.xcuda_Tarification.xcuda_HScode.Commodity_code = pod.TariffCode?.Trim() ?? "NULL";
        itm.xcuda_Tarification.xcuda_HScode.Precision_4 =
            pod.ItemNumber;

        if (cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber != null)
            itm.xcuda_Previous_doc.Summary_declaration =
                cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.BLNumber;

        itm.xcuda_Valuation_item.Total_CIF_itm =
            Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));
        itm.xcuda_Valuation_item.Statistical_value =
            Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4));


        var ivc = new xcuda_Item_Invoice(true)
        {
            TrackingState = TrackingState.Added,
            Amount_national_currency = Convert.ToSingle(Math.Round(
                Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost) *
                Convert.ToDecimal(cdoc.Document.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSet.Exchange_Rate),
                4)),
            Amount_foreign_currency =
                Convert.ToSingle(Math.Round(Convert.ToDecimal(pod.Quantity) * Convert.ToDecimal(pod.Cost), 4)),
            xcuda_Valuation_item = itm.xcuda_Valuation_item
        };


        if (cdoc.Document.xcuda_Valuation != null && cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice != null)
        {
            //;
            ivc.Currency_code =
                cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice
                    .Currency_code;
            ivc.Currency_rate =
                cdoc.Document.xcuda_Valuation.xcuda_Gs_Invoice
                    .Currency_rate;
        }

        itm.xcuda_Valuation_item.xcuda_Item_Invoice = ivc;

        switch (Instance.CurrentApplicationSettings.WeightCalculationMethod)
        {
            case "WeightEqualQuantity":
                if (itm.xcuda_Valuation_item.xcuda_Weight_itm != null)
                {
                    itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm = (decimal)Math.Round(pod.Quantity, 4);

                    itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm = (decimal)Math.Round(pod.Quantity, 4);
                }

                break;
            case "OneOrMore":
                if ((float)pod.Quantity > 99)
                {
                    itm.xcuda_Valuation_item.xcuda_Weight_itm = new xcuda_Weight_itm(true)
                    {
                        TrackingState = TrackingState.Added,
                        Gross_weight_itm = (decimal)(pod.Quantity * .1),
                        Net_weight_itm = (decimal)(pod.Quantity * .1),
                        xcuda_Valuation_item = itm.xcuda_Valuation_item
                    };
                }

                if (pod.Weight != 0)
                    if (itm.xcuda_Valuation_item.xcuda_Weight_itm != null)
                        itm.xcuda_Valuation_item.xcuda_Weight_itm.Gross_weight_itm = (decimal)Math.Round(pod.Weight, 4);
                if (pod.Weight != 0)
                    if (itm.xcuda_Valuation_item.xcuda_Weight_itm != null)
                        itm.xcuda_Valuation_item.xcuda_Weight_itm.Net_weight_itm = (decimal)Math.Round(pod.Weight, 4);
                break;
            case "MinimumWeight":
                SetMinWeight(pod, itm);
                break;
            case "Value":
                //Still set minium weight in event no weight is set via docset
                SetMinWeight(pod, itm);
                break;
            default:
                throw new ApplicationException("Please Configure WeightCalculationMethod");
        }


        if (pod.InternalFreight != 0)
            itm.xcuda_Valuation_item.xcuda_item_internal_freight.Amount_foreign_currency =
                Convert.ToSingle(pod.InternalFreight);


        if (pod.Freight != 0)
            itm.xcuda_Valuation_item.xcuda_item_external_freight.Amount_foreign_currency =
                Convert.ToSingle(pod.Freight);
        // set on each line in event of grouped invoices per IM7 etc.
        if (pod.EntryDataDetails.Count() == 1)
        {
            var fr = pod.EntryDataDetails.FirstOrDefault();
            if (fr != null)
            {
                if (fr.Comment == null)
                {
                    itm.Free_text_1 = $"{fr.EntryDataId}|{fr.LineNumber}";
                    itm.Free_text_2 = $"{pod.ItemNumber}| ";
                }
                else
                {
                    itm.Free_text_1 = $"{fr.EntryDataId}|{fr.LineNumber}|{pod.ItemNumber}";
                    itm.Free_text_2 = fr.Comment;
                }

                itm.PreviousInvoiceItemNumber = pod.ItemNumber;
                itm.PreviousInvoiceLineNumber = fr.LineNumber.ToString();
                itm.PreviousInvoiceNumber = fr.EntryDataId;


                LimitFreeText(itm);
            }
        }

        foreach (var ed in pod.EntryDataDetails)
        {
            if (itm.EntryDataDetails.All(x => x.EntryDataDetailsId != ed.EntryDataDetailsId))
                itm.EntryDataDetails.Add(new xcuda_ItemEntryDataDetails(true)
                {
                    Item_Id = itm.Item_Id,
                    EntryDataDetailsId = ed.EntryDataDetailsId,
                    TrackingState = TrackingState.Added,
                    xcuda_Item = itm
                });

            if (cdoc.Document.AsycudaDocumentEntryDatas.All(x => x.EntryData_Id != ed.EntryData_Id))
                cdoc.Document.AsycudaDocumentEntryDatas.Add(new AsycudaDocumentEntryData(true)
                {
                    AsycudaDocumentId = cdoc.Document.ASYCUDA_Id,
                    EntryData_Id = ed.EntryData_Id,
                    TrackingState = TrackingState.Added,
                    xcuda_ASYCUDA = cdoc.Document
                });

            cdoc.EntryDataDetails.Add(new EntryDataDetails
            {
                EntryDataDetailsId = ed.EntryDataDetailsId,
                EntryDataId = ed.EntryDataId,
                EntryData_Id = ed.EntryData_Id,
                EffectiveDate = ed.EffectiveDate == DateTime.MinValue ? ed.EntryDataDate : ed.EffectiveDate,
            });
        }


        itm.xcuda_Tarification.Unordered_xcuda_Supplementary_unit.Add(new xcuda_Supplementary_unit(true)
        {
            Tarification_Id = itm.xcuda_Tarification.Item_Id,
            Suppplementary_unit_code = "NMB",
            Suppplementary_unit_quantity = pod.Quantity,
            IsFirstRow = true,
            TrackingState = TrackingState.Added,
            xcuda_Tarification = itm.xcuda_Tarification
        });

        ProcessItemTariff(pod, cdoc.Document, itm);

        return itm;
    }
}