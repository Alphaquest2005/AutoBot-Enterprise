SELECT        AsycudaDocumentItem.Item_Id, AsycudaDocumentItem.AsycudaDocumentId, AsycudaDocumentItem.EntryDataDetailsId, AsycudaDocumentItem.LineNumber, AsycudaDocumentItem.IsAssessed, 
                         AsycudaDocumentItem.DoNotAllocate, AsycudaDocumentItem.DoNotEX, AsycudaDocumentItem.AttributeOnlyAllocation, AsycudaDocumentItem.Description_of_goods, AsycudaDocumentItem.Commercial_Description, 
                         AsycudaDocumentItem.Gross_weight_itm, AsycudaDocumentItem.Net_weight_itm, AsycudaDocumentItem.Item_price, AsycudaDocumentItem.ItemQuantity, AsycudaDocumentItem.Suppplementary_unit_code, 
                         AsycudaDocumentItem.ItemNumber, AsycudaDocumentItem.TariffCode, AsycudaDocumentItem.TariffCodeLicenseRequired, AsycudaDocumentItem.TariffCategoryLicenseRequired, AsycudaDocumentItem.TariffCodeDescription, 
                         AsycudaDocumentItem.DutyLiability, AsycudaDocumentItem.Total_CIF_itm, AsycudaDocumentItem.Freight, AsycudaDocumentItem.Statistical_value, AsycudaDocumentItem.DPQtyAllocated, 
                         AsycudaDocumentItem.DFQtyAllocated, AsycudaDocumentItem.ImportComplete, AsycudaDocumentItem.CNumber, AsycudaDocumentItem.RegistrationDate, AsycudaDocumentItem.Number_of_packages, 
                         AsycudaDocumentItem.Country_of_origin_code, AsycudaDocumentItem.PiWeight, AsycudaDocumentItem.Currency_rate, AsycudaDocumentItem.Currency_code, AsycudaDocumentItem.InvalidHSCode, 
                         AsycudaDocumentItem.WarehouseError, AsycudaDocumentItem.PiQuantity, AsycudaDocumentItem.SalesFactor, AsycudaDocumentItem.ReferenceNumber, AsycudaDocumentItem.PreviousInvoiceNumber, 
                         AsycudaDocumentItem.PreviousInvoiceLineNumber, AsycudaDocumentItem.PreviousInvoiceItemNumber, AsycudaDocumentItem.ApplicationSettingsId, AsycudaDocumentItem.Cancelled, AsycudaDocumentItem.ExpiryDate, 
                         AsycudaDocumentItem.InventoryItemId, AsycudaDocumentItem.CustomsProcedure, AsycudaDocumentItem.AssessmentDate, Customs_Procedure.CustomsOperationId
FROM            AsycudaDocumentItem INNER JOIN
                         Customs_Procedure ON AsycudaDocumentItem.CustomsProcedure = Customs_Procedure.CustomsProcedure
WHERE        (Customs_Procedure.CustomsOperationId = 2) AND  ItemNumber ='313856'  --and (AsycudaDocumentItem.CNumber = '31369') AND (AsycudaDocumentItem.LineNumber = 232)

select * from AsycudaSalesAndAdjustmentAllocationsEx where ApplicationSettingsId = 6 and ItemNumber = '317229'

select * from [AsycudaSalesAndAdjustmentAllocationsEx] where pCNumber = '31369' and pLineNumber = '232' and ApplicationSettingsId = 6 



SELECT        AsycudaSalesAllocations.AllocationId, AsycudaSalesAllocations.EntryDataDetailsId, AsycudaSalesAllocations.PreviousItem_Id, AsycudaSalesAllocations.Status, AsycudaSalesAllocations.QtyAllocated, 
                         AsycudaSalesAllocations.EntryTimeStamp, AsycudaSalesAllocations.EANumber, AsycudaSalesAllocations.SANumber, AsycudaSalesAllocations.xEntryItem_Id, AsycudaSalesAllocations.xStatus, 
                         AsycudaSalesAllocations.Comments, AsycudaItemBasicInfo.ItemNumber, AsycudaItemBasicInfo.ItemQuantity, AsycudaItemBasicInfo.DPQtyAllocated, AsycudaItemBasicInfo.DFQtyAllocated, AsycudaItemBasicInfo.IsAssessed, 
                         AsycudaItemBasicInfo.LineNumber, AsycudaItemBasicInfo.CNumber, AsycudaItemBasicInfo.RegistrationDate
FROM           AsycudaSalesAllocations INNER JOIN
                         AsycudaItemBasicInfo ON AsycudaSalesAllocations.PreviousItem_Id = AsycudaItemBasicInfo.Item_Id
WHERE        (AsycudaSalesAllocations.PreviousItem_Id = 76013)

select * from AsycudaItemBasicInfo where ApplicationSettingsId = 6

select * from AsycudaItemPiQuantityData where ItemNumber = '317229'


select * from entrydata where entrydataid = '29916-7440'

select * from EntryDataDetailsEx where ApplicationSettingsId = 6 and ItemNumber = '317229'


select * from AsycudaItemRemainingQuantities where ItemNumber = '323999'


SELECT        EntryData.EntryType, EntryData.EntryDataDate, EntryDataDetailsEx.EntryDataDetailsId, EntryDataDetailsEx.EntryData_Id, EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemNumber, 
                         EntryDataDetailsEx.Quantity, EntryDataDetailsEx.Units, EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.Cost, EntryDataDetailsEx.QtyAllocated, EntryDataDetailsEx.VolumeLiters, EntryDataDetailsEx.UnitWeight, 
                         EntryDataDetailsEx.DoNotAllocate, EntryDataDetailsEx.TariffCode, EntryDataDetailsEx.CNumber, EntryDataDetailsEx.CLineNumber, EntryDataDetailsEx.Downloaded, EntryDataDetailsEx.DutyFreePaid, 
                         EntryDataDetailsEx.Total, EntryDataDetailsEx.AsycudaDocumentSetId, EntryDataDetailsEx.InvoiceQty, EntryDataDetailsEx.ReceivedQty, EntryDataDetailsEx.Status, EntryDataDetailsEx.PreviousInvoiceNumber, 
                         EntryDataDetailsEx.PreviousCNumber, EntryDataDetailsEx.PreviousCLineNumber, EntryDataDetailsEx.Comment, EntryDataDetailsEx.EffectiveDate, EntryDataDetailsEx.IsReconciled, EntryDataDetailsEx.ApplicationSettingsId, 
                         EntryDataDetailsEx.LastCost, EntryDataDetailsEx.FileLineNumber, EntryDataDetailsEx.TaxAmount, EntryDataDetailsEx.EmailId, EntryDataDetailsEx.FileTypeId, EntryDataDetailsEx.Name, 
                         EntryDataDetailsEx.InventoryItemId
FROM            EntryDataDetailsEx INNER JOIN
                         EntryData ON EntryDataDetailsEx.EntryData_Id = EntryData.EntryData_Id
WHERE        (EntryDataDetailsEx.ItemNumber = '320834')