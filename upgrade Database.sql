INSERT INTO WeightCalculationMethods
SELECT Name
FROM    [Rouge-ENTERPRISEDB].dbo.WeightCalculationMethods AS WeightCalculationMethods_1

INSERT INTO BondTypes
SELECT Name
FROM    [AutoBot-EnterpriseDB].dbo.BondTypes AS BondTypes_1

--SET IDENTITY_INSERT dbo.FileTypes ON;  
--INSERT INTO FileTypes
--                 (ApplicationSettingsId, FilePattern, Type, AsycudaDocumentSetId, CreateDocumentSet, DocumentSpecific, DocumentCode, ReplyToMail, FileGroupId, MergeEmails, CopyEntryData, ParentFileTypeId, OverwriteFiles, 
--                 HasFiles, OldFileTypeId, ReplicateHeaderRow, Id)
--SELECT 5, FilePattern, Type, case when  AsycudaDocumentSetId = 1 then 4285 when AsycudaDocumentSetId = 3 then 4321 else AsycudaDocumentSetId end as AsycudaDocumentSetId , CreateDocumentSet, DocumentSpecific, DocumentCode, ReplyToMail, FileGroupId, MergeEmails, CopyEntryData, ParentFileTypeId, OverwriteFiles, 
--                 HasFiles, OldFileTypeId, ReplicateHeaderRow, Id
--FROM    [Rouge-DiscoveryDB].dbo.FileTypes AS IWW

--SET IDENTITY_INSERT dbo.FileTypes off;  


INSERT INTO Customs_Procedure
                 (Document_TypeId, Customs_ProcedureId, Extended_customs_procedure, National_customs_procedure, IsObsolete, IsPaid, BondTypeId, Stock, Discrepancy, Adjustment, Sales, CustomsOperationId, 
                 SubmitToCustoms)
SELECT  Document_TypeId, Customs_ProcedureId, Extended_customs_procedure, National_customs_procedure,  IsObsolete, IsPaid, BondTypeId, Stock, Discrepancy, Adjustment, Sales, 
                 CustomsOperationId, SubmitToCustoms
FROM    [AutoBot-EnterpriseDB].dbo.Customs_Procedure AS BondTypes_1


INSERT INTO ApplicationSettings
                 (TestMode,IsActive, CompanyName, Description, MaxEntryLines, SoftwareName, AllowCounterPoint, GroupEX9, InvoicePerEntry, AllowTariffCodes, AllowWareHouse, AllowXBond, AllowAsycudaManager, AllowQuickBooks, 
                 ItemDescriptionContainsAsycudaAttribute, AllowExportToExcel, AllowAutoWeightCalculation, AllowEntryPerIM7, AllowSalesToPI, AllowEffectiveAssessmentDate, AllowAutoFreightCalculation, AllowSubItems, 
                 AllowEntryDoNotAllocate, AllowPreviousItems, AllowOversShort, AllowContainers, AllowNonXEntries, AllowValidateTariffCodes, AllowCleanBond, OrderEntriesBy, OpeningStockDate, WeightCalculationMethod, 
                 DeclarantCode, BondQuantum, BondTypeId)
SELECT 0,1,'Rouge',Description, MaxEntryLines, SoftwareName, AllowCounterPoint, GroupEX9, InvoicePerEntry, AllowTariffCodes, AllowWareHouse, AllowXBond, AllowAsycudaManager, AllowQuickBooks, 
                 ItemDescriptionContainsAsycudaAttribute, AllowExportToExcel, AllowAutoWeightCalculation, AllowEntryPerIM7, AllowSalesToPI, AllowEffectiveAssessmentDate, AllowAutoFreightCalculation, AllowSubItems, 
                 AllowEntryDoNotAllocate, AllowPreviousItems, AllowOversShort, AllowContainers, AllowNonXEntries, AllowValidateTariffCodes, AllowCleanBond, OrderEntriesBy, OpeningStockDate, WeightCalculationMethod, 
                 DeclarantCode, BondQuantum, 2
FROM    [Rouge-EnterpriseDB].dbo.ApplicationSettings AS ApplicationSettings_1

--AsycudaDocumentSet
INSERT INTO AsycudaDocumentSet
                 (ApplicationSettingsId, Declarant_Reference_Number, Exchange_Rate, Customs_ProcedureId, Country_of_origin_code, Currency_Code, Document_TypeId, Description, Manifest_Number, BLNumber, EntryTimeStamp, 
                 StartingFileCount, ApportionMethod, TotalWeight, TotalFreight, TotalPackages, UpgradeKey)
SELECT 6 AS Expr1, ApplicationSettings_1.Declarant_Reference_Number, ApplicationSettings_1.Exchange_Rate, Customs_Procedure.Customs_ProcedureId, ApplicationSettings_1.Country_of_origin_code, 
                 ApplicationSettings_1.Currency_Code, Document_Type.Document_TypeId, ApplicationSettings_1.Description, ApplicationSettings_1.Manifest_Number, ApplicationSettings_1.BLNumber, 
                 ApplicationSettings_1.EntryTimeStamp, ApplicationSettings_1.StartingFileCount, ApplicationSettings_1.ApportionMethod, ApplicationSettings_1.TotalWeight, ApplicationSettings_1.TotalFreight, 
                 ApplicationSettings_1.TotalPackages, ApplicationSettings_1.AsycudaDocumentSetId
FROM    [Rouge-EnterpriseDB].dbo.AsycudaDocumentSet AS ApplicationSettings_1 INNER JOIN
                 [Rouge-EnterpriseDB].dbo.Customs_Procedure AS Customs_Procedure_1 ON ApplicationSettings_1.Customs_ProcedureId = Customs_Procedure_1.Customs_ProcedureId INNER JOIN
                 Customs_Procedure ON Customs_Procedure_1.Extended_customs_procedure = Customs_Procedure.Extended_customs_procedure AND 
                 Customs_Procedure_1.National_customs_procedure = Customs_Procedure.National_customs_procedure INNER JOIN
                 [Rouge-EnterpriseDB].dbo.Document_Type AS Document_Type_1 ON ApplicationSettings_1.Document_TypeId = Document_Type_1.Document_TypeId INNER JOIN
                 Document_Type ON Document_Type_1.Type_of_declaration = Document_Type.Type_of_declaration AND Document_Type_1.Declaration_gen_procedure_code = Document_Type.Declaration_gen_procedure_code

INSERT INTO ExportTemplate
                 (Description, Exporter_code, Exporter_name, Consignee_code, Consignee_name, Financial_code, Customs_clearance_office_code, Customs_Procedure, Declarant_code, Country_first_destination, Trading_country, 
                 Export_country_code, Destination_country_code, TransportName, TransportNationality, Location_of_goods, Border_information_Mode, Delivery_terms_Code, Border_office_Code, Gs_Invoice_Currency_code, 
                 Warehouse_Identification, Warehouse_Delay, Number_of_packages, Total_number_of_packages, Deffered_payment_reference, AttachedDocumentCode, ApplicationSettingsId)
SELECT Description, Exporter_code, Exporter_name, Consignee_code, Consignee_name, Financial_code, Customs_clearance_office_code, Customs_Procedure, Declarant_code, Country_first_destination, Trading_country, 
                 Export_country_code, Destination_country_code, TransportName, TransportNationality, Location_of_goods, Border_information_Mode, Delivery_terms_Code, Border_office_Code, Gs_Invoice_Currency_code, 
                 Warehouse_Identification, Warehouse_Delay, Number_of_packages, Total_number_of_packages, Deffered_payment_reference, AttachedDocumentCode, 6 AS Expr1
FROM    [Rouge-EnterpriseDB].dbo.ExportTemplate AS IWW

---SystemDocSets
---manual
INSERT INTO AsycudaDocumentSet
SELECT
FROM    [Rouge-EnterpriseDB].dbo.AsycudaDocumentSet AS IWW

---------------------------EntryData

--INSERT INTO EntryData
--                 (ApplicationSettingsId,EntryDataId, EntryDataDate, InvoiceTotal, ImportedLines, Currency, UpgradeKey)
--SELECT 6,IWW.EntryDataId, IWW.EntryDataDate, IWW.ImportedTotal, IWW.ImportedLines, IWW.Currency, IWW.EntryDataId
--FROM    [Rouge-EnterpriseDB].dbo.EntryData AS IWW INNER JOIN
--                 [Rouge-EnterpriseDB].dbo.EntryData_Sales ON IWW.EntryDataId = [Rouge-EnterpriseDB].dbo.EntryData_Sales.EntryDataId


--INSERT INTO EntryData
--                 (ApplicationSettingsId,EntryDataId, EntryDataDate, InvoiceTotal, ImportedLines, Currency, UpgradeKey)
--SELECT 6,IWW.EntryDataId, IWW.EntryDataDate, IWW.ImportedTotal, IWW.ImportedLines, IWW.Currency, IWW.EntryDataId
--FROM    [Rouge-EnterpriseDB].dbo.EntryData AS IWW INNER JOIN
--                 [Rouge-EnterpriseDB].dbo.EntryData_OpeningStock ON IWW.EntryDataId = [Rouge-EnterpriseDB].dbo.EntryData_OpeningStock.EntryDataId

--INSERT INTO EntryData_Sales
--                 (INVNumber, CustomerName, EntryData_Id)
--SELECT EntryData_Sales_1.INVNumber, EntryData_Sales_1.CustomerName, EntryData.EntryData_Id
--FROM    EntryData INNER JOIN
--                 [Rouge-EnterpriseDB].dbo.EntryData_Sales AS EntryData_Sales_1 ON EntryData.UpgradeKey = EntryData_Sales_1.INVNumber
--WHERE (EntryData.ApplicationSettingsId = 6)


--INSERT INTO EntryData_OpeningStock
--                 (OPSNumber, EntryData_Id)
--SELECT EntryData_Sales_1.OPSNumber, EntryData.EntryData_Id
--FROM    EntryData INNER JOIN
--                 [Rouge-EnterpriseDB].dbo.EntryData_OpeningStock AS EntryData_Sales_1 ON EntryData.UpgradeKey = EntryData_Sales_1.OPSNumber
--WHERE (EntryData.ApplicationSettingsId = 6)

---------------------Inventoryitems

--INSERT INTO InventoryItems
--                 (ItemNumber, Description, Category, TariffCode, EntryTimeStamp, UpgradeKey, ApplicationSettingsId)
--SELECT ItemNumber, Description, Category, TariffCode, EntryTimeStamp, ItemNumber AS Expr1, 6 AS Expr2
--FROM    [Rouge-EnterpriseDB].dbo.InventoryItems AS IWW


---------------------EntryDataDetails
INSERT INTO EntryDataDetails
                 (UpgradeKey, EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, QtyAllocated, UnitWeight, DoNotAllocate, Freight, Weight, InternalFreight, Status, InvoiceQty, ReceivedQty, 
                 PreviousInvoiceNumber, CNumber, Comment, EffectiveDate, IsReconciled, TaxAmount, EntryData_Id, InventoryItemId)
SELECT IWW.EntryDataDetailsId, IWW.EntryDataId, IWW.LineNumber, IWW.ItemNumber, IWW.Quantity, IWW.Units, IWW.ItemDescription, IWW.Cost, IWW.QtyAllocated, IWW.UnitWeight, IWW.DoNotAllocate, IWW.Freight, 
                 IWW.Weight, IWW.InternalFreight, IWW.Status, IWW.InvoiceQty, IWW.ReceivedQty, IWW.PreviousInvoiceNumber, IWW.CNumber, IWW.Comment, IWW.EffectiveDate, IWW.IsReconciled, IWW.TaxAmount, 
                 EntryData.EntryData_Id, InventoryItems.Id
FROM    [Rouge-EnterpriseDB].dbo.EntryDataDetails AS IWW INNER JOIN
                 EntryData ON IWW.EntryDataId = EntryData.EntryDataId INNER JOIN
                 InventoryItems ON EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND IWW.ItemNumber = InventoryItems.ItemNumber
WHERE (EntryData.ApplicationSettingsId = 6)



---------------------------------AsycudaDocumentSetEntryData
INSERT INTO AsycudaDocumentSetEntryData
                 (AsycudaDocumentSetId, EntryData_Id)
SELECT AsycudaDocumentSet.AsycudaDocumentSetId, EntryData.EntryData_Id
FROM    [Rouge-EnterpriseDB].dbo.AsycudaDocumentSetEntryData AS IWW INNER JOIN
                 EntryData ON IWW.EntryDataId = EntryData.EntryDataId INNER JOIN
                 AsycudaDocumentSet ON EntryData.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND IWW.AsycudaDocumentSetId = AsycudaDocumentSet.UpgradeKey
WHERE (AsycudaDocumentSet.ApplicationSettingsId = 6)


-------------------------------------------------

------------------------------xcuda_ASYCUDA
INSERT INTO xcuda_ASYCUDA
                 (id, EntryTimeStamp, UpgradeKey)
SELECT id, EntryTimeStamp, ASYCUDA_Id
FROM    [Rouge-EnterpriseDB].dbo.xcuda_ASYCUDA AS IWW


-----------------------------------xcuda_ASYCUDA_ExtendedProperties
INSERT INTO xcuda_ASYCUDA_ExtendedProperties
                 (ASYCUDA_Id, AsycudaDocumentSetId, FileNumber, IsManuallyAssessed, CNumber, RegistrationDate, ReferenceNumber, Customs_ProcedureId, Document_TypeId, Description, BLNumber, AutoUpdate, 
                 EffectiveRegistrationDate, ApportionMethod, DoNotAllocate, ImportComplete, DocumentLines, Cancelled, TotalWeight, TotalFreight, TotalInternalFreight, TotalPackages, EffectiveExpiryDate)
SELECT distinct xcuda_ASYCUDA.ASYCUDA_Id, 1 AS Expr1, IWW.FileNumber, IWW.IsManuallyAssessed, IWW.CNumber, IWW.RegistrationDate, IWW.ReferenceNumber, Customs_Procedure.Customs_ProcedureId, 
                 Customs_Procedure.Document_TypeId, IWW.Description, IWW.BLNumber, IWW.AutoUpdate, IWW.EffectiveRegistrationDate, IWW.ApportionMethod, IWW.DoNotAllocate, IWW.ImportComplete, IWW.DocumentLines, 
                 IWW.Cancelled, IWW.TotalWeight, IWW.TotalFreight, IWW.TotalInternalFreight, IWW.TotalPackages, IWW.EffectiveExpiryDate
FROM    [Rouge-EnterpriseDB].dbo.xcuda_ASYCUDA_ExtendedProperties AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey INNER JOIN
                 [Rouge-EnterpriseDB].dbo.Customs_Procedure AS Customs_Procedure_1 ON IWW.Customs_ProcedureId = Customs_Procedure_1.Customs_ProcedureId INNER JOIN
                 Customs_Procedure ON Customs_Procedure_1.Extended_customs_procedure = Customs_Procedure.Extended_customs_procedure AND 
                 Customs_Procedure_1.National_customs_procedure = Customs_Procedure.National_customs_procedure
where  Customs_Procedure.Customs_ProcedureId not in (113, 128) 

----------------------------------------------------------------------


-------------------------



---------------------xcuda_Office_segment

INSERT INTO xcuda_Identification
                 (ASYCUDA_Id, Manifest_reference_number)
SELECT DISTINCT xcuda_ASYCUDA.ASYCUDA_Id, IWW.Manifest_reference_number
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Identification AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey


INSERT INTO xcuda_Office_segment
                 (ASYCUDA_Id, Customs_clearance_office_code, Customs_Clearance_office_name)
SELECT DISTINCT xcuda_ASYCUDA.ASYCUDA_Id, IWW.Customs_clearance_office_code, IWW.Customs_Clearance_office_name
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Office_segment AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey

INSERT INTO xcuda_Registration
                 (ASYCUDA_Id, Number, Date)
SELECT DISTINCT xcuda_ASYCUDA.ASYCUDA_Id, IWW.Number, IWW.Date
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Registration AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey


INSERT INTO xcuda_Type
                 (ASYCUDA_Id, Type_of_declaration, Declaration_gen_procedure_code)
SELECT DISTINCT xcuda_ASYCUDA.ASYCUDA_Id, IWW.Type_of_declaration, IWW.Declaration_gen_procedure_code
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Type AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey


INSERT INTO xcuda_Valuation
                 (ASYCUDA_Id, Total_CIF, Total_cost, Calculation_working_mode)
SELECT DISTINCT xcuda_ASYCUDA.ASYCUDA_Id, IWW.Total_CIF, IWW.Total_cost, IWW.Calculation_working_mode
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Valuation AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey

INSERT INTO xcuda_Warehouse
                 (ASYCUDA_Id, Identification, Delay)
SELECT DISTINCT xcuda_ASYCUDA.ASYCUDA_Id, IWW.Identification, IWW.Delay
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Warehouse AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey

INSERT INTO xcuda_Declarant
                 (ASYCUDA_Id, Declarant_code, Declarant_name, Declarant_representative, Number)
SELECT DISTINCT xcuda_ASYCUDA.ASYCUDA_Id, IWW.Declarant_code, IWW.Declarant_name, IWW.Declarant_representative, IWW.Number
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Declarant AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey
				 
select * from xcuda_Declarant INNER JOIN
                 xcuda_ASYCUDA ON xcuda_Declarant.ASYCUDA_Id = xcuda_ASYCUDA.ASYCUDA_Id
				 where xcuda_ASYCUDA.UpgradeKey is not null

-----------------------------------------------------------------------------

-----------------------xcuda_items

INSERT INTO xcuda_Item
                 (ASYCUDA_Id, Amount_deducted_from_licence, Quantity_deducted_from_licence, UpgradeKey, Licence_number, Free_text_1, Free_text_2, EntryDataDetailsId, LineNumber, IsAssessed, DPQtyAllocated, 
                 DFQtyAllocated, EntryTimeStamp, AttributeOnlyAllocation, DoNotAllocate, DoNotEX, ImportComplete, WarehouseError, SalesFactor, PreviousInvoiceNumber, PreviousInvoiceLineNumber, PreviousInvoiceItemNumber)
SELECT DISTINCT 
                 xcuda_ASYCUDA.ASYCUDA_Id, IWW.Amount_deducted_from_licence, IWW.Quantity_deducted_from_licence, IWW.Item_Id, IWW.Licence_number, IWW.Free_text_1, IWW.Free_text_2, IWW.EntryDataDetailsId, 
                 IWW.LineNumber, IWW.IsAssessed, IWW.DPQtyAllocated, IWW.DFQtyAllocated, IWW.EntryTimeStamp, IWW.AttributeOnlyAllocation, IWW.DoNotAllocate, IWW.DoNotEX, IWW.ImportComplete, IWW.WarehouseError, 
                 IWW.SalesFactor, IWW.PreviousInvoiceNumber, IWW.PreviousInvoiceLineNumber, IWW.PreviousInvoiceItemNumber
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Item AS IWW INNER JOIN
                 xcuda_ASYCUDA ON IWW.ASYCUDA_Id = xcuda_ASYCUDA.UpgradeKey

------------------------xcuda_Goods_description
INSERT INTO xcuda_Goods_description
                 (Item_Id, Country_of_origin_code, Description_of_goods, Commercial_Description)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Country_of_origin_code, IWW.Description_of_goods, IWW.Commercial_Description
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Goods_description AS IWW INNER JOIN
                 xcuda_Item ON IWW.Item_Id = xcuda_Item.UpgradeKey

--------------------------xcuda_HScode
INSERT INTO xcuda_Tarification
                 (Item_Id, Extended_customs_procedure, National_customs_procedure, Item_price, Value_item, Attached_doc_item)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Extended_customs_procedure, IWW.National_customs_procedure, IWW.Item_price, IWW.Value_item, IWW.Attached_doc_item
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Tarification AS IWW INNER JOIN
                 xcuda_Item ON IWW.Item_Id = xcuda_Item.UpgradeKey

--------------------------xcuda_HScode
INSERT INTO xcuda_HScode
                 (Item_Id, Commodity_code, Precision_1, Precision_4)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Commodity_code, IWW.Precision_1, IWW.Precision_4
FROM    [Rouge-EnterpriseDB].dbo.xcuda_HScode AS IWW INNER JOIN
                 xcuda_Item ON IWW.Item_Id = xcuda_Item.UpgradeKey

-----------------------------

INSERT INTO xcuda_Packages
                 (Item_Id, Number_of_packages, Kind_of_packages_code, Kind_of_packages_name, Marks1_of_packages, Marks2_of_packages)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Number_of_packages, IWW.Kind_of_packages_code, IWW.Kind_of_packages_name, IWW.Marks1_of_packages, IWW.Marks2_of_packages
FROM    [Rouge-EnterpriseDB].dbo.xcuda_Packages AS IWW INNER JOIN
                 xcuda_Item ON IWW.Item_Id = xcuda_Item.UpgradeKey

INSERT INTO xcuda_Supplementary_unit
                 (Tarification_Id, Suppplementary_unit_quantity, Suppplementary_unit_code, Suppplementary_unit_name, IsFirstRow)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Suppplementary_unit_quantity, IWW.Suppplementary_unit_code, IWW.Suppplementary_unit_name, IWW.IsFirstRow
FROM    xcuda_Item INNER JOIN
                 [Rouge-EnterpriseDB].dbo.xcuda_Supplementary_unit AS IWW ON xcuda_Item.UpgradeKey = IWW.Tarification_Id

INSERT INTO xcuda_Valuation_item
                 (Item_Id, Alpha_coeficient_of_apportionment, Statistical_value, Rate_of_adjustement, Total_CIF_itm, Total_cost_itm)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Alpha_coeficient_of_apportionment, IWW.Statistical_value, IWW.Rate_of_adjustement, IWW.Total_CIF_itm, IWW.Total_cost_itm
FROM    xcuda_Item INNER JOIN
                 [Rouge-EnterpriseDB].dbo.xcuda_Valuation_item AS IWW ON xcuda_Item.UpgradeKey = IWW.Item_Id


INSERT INTO xcuda_Weight_itm
                 (Valuation_item_Id, Gross_weight_itm, Net_weight_itm)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Gross_weight_itm, IWW.Net_weight_itm
FROM    xcuda_Item INNER JOIN
                 [Rouge-EnterpriseDB].dbo.xcuda_Weight_itm AS IWW ON xcuda_Item.UpgradeKey = IWW.Valuation_item_Id

INSERT INTO xcuda_PreviousItem
                 (Packages_number, Previous_Packages_number, Hs_code, Commodity_code, Previous_item_number, Goods_origin, Net_weight, Prev_net_weight, Prev_reg_ser, Prev_reg_nbr, Prev_reg_dat, Prev_reg_cuo, 
                 Suplementary_Quantity, Preveious_suplementary_quantity, Current_value, Previous_value, Current_item_number, QtyAllocated, Prev_decl_HS_spec, PreviousItem_Id, ASYCUDA_Id)
SELECT DISTINCT 
                 IWW.Packages_number, IWW.Previous_Packages_number, IWW.Hs_code, IWW.Commodity_code, IWW.Previous_item_number, IWW.Goods_origin, IWW.Net_weight, IWW.Prev_net_weight, IWW.Prev_reg_ser, 
                 IWW.Prev_reg_nbr, IWW.Prev_reg_dat, IWW.Prev_reg_cuo, IWW.Suplementary_Quantity, IWW.Preveious_suplementary_quantity, IWW.Current_value, IWW.Previous_value, IWW.Current_item_number, IWW.QtyAllocated, 
                 IWW.Prev_decl_HS_spec, xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id
FROM    xcuda_Item INNER JOIN
                 [Rouge-EnterpriseDB].dbo.xcuda_Item AS xcuda_Item_1 ON xcuda_Item.UpgradeKey = xcuda_Item_1.Item_Id INNER JOIN
                 [Rouge-EnterpriseDB].dbo.xcuda_PreviousItem AS IWW ON xcuda_Item_1.Item_Id = IWW.PreviousItem_Id


INSERT INTO EntryPreviousItems
                 (PreviousItem_Id, Item_Id)
SELECT DISTINCT xcuda_Item_1.Item_Id, xcuda_Item.Item_Id AS Expr1
FROM    [Rouge-EnterpriseDB].dbo.EntryPreviousItems AS IWW INNER JOIN
                 xcuda_Item ON IWW.Item_Id = xcuda_Item.UpgradeKey INNER JOIN
                 xcuda_Item AS xcuda_Item_1 ON IWW.PreviousItem_Id = xcuda_Item_1.UpgradeKey


INSERT INTO xcuda_Item_Invoice
                 (Valuation_item_Id, Amount_national_currency, Amount_foreign_currency, Currency_code, Currency_rate)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Amount_national_currency, IWW.Amount_foreign_currency, IWW.Currency_code, IWW.Currency_rate
FROM    xcuda_Item INNER JOIN
                 [Rouge-EnterpriseDB].dbo.xcuda_Item_Invoice AS IWW ON xcuda_Item.UpgradeKey = IWW.Valuation_item_Id

INSERT INTO xcuda_item_external_freight
                 (Valuation_item_Id, Amount_national_currency, Amount_foreign_currency, Currency_code, Currency_rate)
SELECT DISTINCT xcuda_Item.Item_Id, IWW.Amount_national_currency, IWW.Amount_foreign_currency, IWW.Currency_code, IWW.Currency_rate
FROM    xcuda_Item INNER JOIN
                 [Rouge-EnterpriseDB].dbo.xcuda_item_external_freight AS IWW ON xcuda_Item.UpgradeKey = IWW.Valuation_item_Id
---------------------------not a upgrade
INSERT INTO xcuda_Inventory_Item
                 (Item_Id, InventoryItemId)
SELECT DISTINCT xcuda_Item.Item_Id, InventoryItems.Id
FROM    xcuda_Item INNER JOIN
                 xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
                 InventoryItems ON xcuda_HScode.Precision_4 = InventoryItems.UpgradeKey


INSERT INTO AsycudaSalesAllocations
                 (EntryDataDetailsId, PreviousItem_Id, xEntryItem_Id, Status, QtyAllocated, EntryTimeStamp, EANumber, SANumber)
SELECT DISTINCT EntryDataDetails.EntryDataDetailsId, xcuda_Item.Item_Id, xcuda_Item_1.Item_Id AS Expr1, IWW.Status, IWW.QtyAllocated, IWW.EntryTimeStamp, IWW.EANumber, IWW.SANumber
FROM    [Rouge-EnterpriseDB].dbo.AsycudaSalesAllocations AS IWW INNER JOIN
                 EntryDataDetails ON IWW.EntryDataDetailsId = EntryDataDetails.UpgradeKey INNER JOIN
                 xcuda_Item ON IWW.PreviousItem_Id = xcuda_Item.UpgradeKey LEFT OUTER JOIN
                 xcuda_Item AS xcuda_Item_1 ON IWW.xEntryItem_Id = xcuda_Item_1.UpgradeKey







SELECT xcuda_Goods_description.Commercial_Description, AsycudaDocumentSet.ApplicationSettingsId
FROM    AsycudaDocumentSet INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentSet.AsycudaDocumentSetId = xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId INNER JOIN
                 xcuda_Item ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id INNER JOIN
                 xcuda_Goods_description ON xcuda_Item.Item_Id = xcuda_Goods_description.Item_Id
WHERE (AsycudaDocumentSet.ApplicationSettingsId = 6)