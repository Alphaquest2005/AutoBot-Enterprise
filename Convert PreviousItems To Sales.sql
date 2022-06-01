SELECT 'Asycuda-C#' + PreviousItemsEx.CNumber AS 'Invoice #', AsycudaDocument.RegistrationDate AS Date, PreviousItemsEx.Prev_decl_HS_spec AS [Item Code], InventoryItems.Description, 
                 PreviousItemsEx.Suplementary_Quantity AS Quantity, PreviousItemsEx.Current_value AS TotalCost, PreviousItemsEx.Prev_reg_nbr AS CNumber, PreviousItemsEx.Previous_item_number AS CLineNumber, 
                 PreviousItemsEx.CNumber AS xCNumber, PreviousItemsEx.Current_item_number AS xLineNumber
FROM    InventoryItems INNER JOIN
                 [InventoryItems-Lumped] ON InventoryItems.Id = [InventoryItems-Lumped].InventoryItemId INNER JOIN
                 PreviousItemsEx ON InventoryItems.ApplicationSettingsId = PreviousItemsEx.ApplicationSettingsId AND InventoryItems.ItemNumber = PreviousItemsEx.Prev_decl_HS_spec INNER JOIN
                 AsycudaDocument ON PreviousItemsEx.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id
WHERE (PreviousItemsEx.ApplicationSettingsId = 6) and AsycudaDocument.EffectiveRegistrationDate is null--AND (AsycudaDocument.RegistrationDate <= '10/1/2021')
GROUP BY PreviousItemsEx.Prev_reg_nbr, PreviousItemsEx.CNumber, PreviousItemsEx.Current_item_number, PreviousItemsEx.Previous_item_number, PreviousItemsEx.Prev_decl_HS_spec, InventoryItems.Description, 
                 PreviousItemsEx.Suplementary_Quantity, PreviousItemsEx.Current_value, AsycudaDocument.RegistrationDate
ORDER BY Date
---------------------------------------------------------------------------
SELECT 'Asycuda-C#' + PreviousItemsEx.CNumber AS 'Invoice #', AsycudaDocument.RegistrationDate AS Date, PreviousItemsEx.Prev_decl_HS_spec AS [Item Code], InventoryItems.Description, 
                 PreviousItemsEx.Suplementary_Quantity AS Quantity, PreviousItemsEx.Current_value AS TotalCost, PreviousItemsEx.Prev_reg_nbr AS CNumber, PreviousItemsEx.Previous_item_number AS CLineNumber, 
                 PreviousItemsEx.CNumber AS xCNumber, PreviousItemsEx.Current_item_number AS xLineNumber
FROM    InventoryItems INNER JOIN
                 [InventoryItems-Lumped] ON InventoryItems.Id = [InventoryItems-Lumped].InventoryItemId INNER JOIN
                 PreviousItemsEx ON InventoryItems.ApplicationSettingsId = PreviousItemsEx.ApplicationSettingsId AND InventoryItems.ItemNumber = PreviousItemsEx.Prev_decl_HS_spec INNER JOIN
                 AsycudaDocument ON PreviousItemsEx.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id
WHERE (PreviousItemsEx.ApplicationSettingsId = 6) AND (AsycudaDocument.DocumentType = 'IM9') --AND (AsycudaDocument.RegistrationDate >= '10/1/2021')
GROUP BY PreviousItemsEx.Prev_reg_nbr, PreviousItemsEx.CNumber, PreviousItemsEx.Current_item_number, PreviousItemsEx.Previous_item_number, PreviousItemsEx.Prev_decl_HS_spec, InventoryItems.Description, 
                 PreviousItemsEx.Suplementary_Quantity, PreviousItemsEx.Current_value, AsycudaDocument.RegistrationDate
ORDER BY Date

------------------------------------------------------------------------


select * from PreviousItemsEx where ApplicationSettingsId = 6

SELECT EntryDataDetails.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, EntryDataDetails.Cost, 
                 PreviousItemsEx.Prev_reg_nbr AS CNumber, PreviousItemsEx.Previous_item_number AS pLineNumber, PreviousItemsEx.CNumber AS xCNumber, PreviousItemsEx.Current_item_number AS xLineNumber, 
                 EntryDataDetails.LineNumber, EntryDataDetails.EntryDataDetailsId
FROM    InventoryItems INNER JOIN
                 [InventoryItems-Lumped] ON InventoryItems.Id = [InventoryItems-Lumped].InventoryItemId INNER JOIN
                 EntryData INNER JOIN
                 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 PreviousItemsEx ON EntryData.EntryDataId LIKE '%' + PreviousItemsEx.CNumber + '-' + PreviousItemsEx.Current_item_number AND EntryData.ApplicationSettingsId = PreviousItemsEx.ApplicationSettingsId INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id ON InventoryItems.ItemNumber = PreviousItemsEx.ItemNumber AND 
                 InventoryItems.ApplicationSettingsId = PreviousItemsEx.ApplicationSettingsId
WHERE (PreviousItemsEx.ApplicationSettingsId = 6) AND (EntryData.ApplicationSettingsId = 6) AND (EntryDataDetails.EntryDataId LIKE N'Asycuda-C#%')
GROUP BY EntryDataDetails.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, EntryDataDetails.Cost, PreviousItemsEx.Prev_reg_nbr, 
                 PreviousItemsEx.CNumber, PreviousItemsEx.Current_item_number, EntryDataDetails.LineNumber, EntryDataDetails.EntryDataDetailsId, PreviousItemsEx.Previous_item_number


----//////////////////////////////// set the date to the date entry was done

UPDATE EntryData
SET         EntryDataDate = EntryDataDetails.EffectiveDate
FROM    EntryData_Adjustments INNER JOIN
                 EntryData ON EntryData_Adjustments.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id
WHERE (EntryData.ApplicationSettingsId = 6) AND (EntryDataDetails.EntryDataId LIKE N'Asycuda-C#%')


UPDATE EntryDataDetails
SET         IsReconciled = 1 --- to prevent it from actually trying to exwarehouse these
FROM    EntryData_Adjustments INNER JOIN
                 EntryData ON EntryData_Adjustments.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id
WHERE (EntryData.ApplicationSettingsId = 6) AND (EntryDataDetails.EntryDataId LIKE N'Asycuda-C#%')



SELECT InventoryItems.ItemNumber, InventoryItems.ApplicationSettingsId, InventoryItems.Description, InventoryItems.Category, InventoryItems.TariffCode, InventoryItems.EntryTimeStamp, InventoryItems.Id, 
                 InventoryItems.UpgradeKey, InventorySources.Name
FROM    InventoryItems INNER JOIN
                 InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
                 InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id
WHERE (InventoryItems.ApplicationSettingsId = 6) AND (InventorySources.Name = N'Asycuda')

select * from PreviousItemsEx where Prev_reg_nbr = '46042'

select * from EntryDataDetails where EntryDataDetails.EntryDataId = 'Asycuda-C#5062'

select * from AsycudaSalesAndAdjustmentAllocationsEx where invoiceno like 'Asycuda-C#5062'

-------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------


SELECT EntryDataDetails.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, EntryDataDetails.Cost, 
                 PreviousItemsEx.Prev_reg_nbr AS CNumber, PreviousItemsEx.Previous_item_number AS pLineNumber, PreviousItemsEx.CNumber AS xCNumber, PreviousItemsEx.Current_item_number AS xLineNumber, 
                 EntryDataDetails.LineNumber, EntryDataDetails.EntryDataDetailsId
FROM    InventoryItems INNER JOIN
                 [InventoryItems-Lumped] ON InventoryItems.Id = [InventoryItems-Lumped].InventoryItemId INNER JOIN
                 EntryData INNER JOIN
                 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 PreviousItemsEx ON EntryData.EntryDataId LIKE '%' + PreviousItemsEx.CNumber + '-' + PreviousItemsEx.Current_item_number AND EntryData.ApplicationSettingsId = PreviousItemsEx.ApplicationSettingsId INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id ON InventoryItems.ItemNumber = PreviousItemsEx.ItemNumber AND 
                 InventoryItems.ApplicationSettingsId = PreviousItemsEx.ApplicationSettingsId
WHERE (PreviousItemsEx.ApplicationSettingsId = 7) AND (EntryData.ApplicationSettingsId = 7) AND (EntryDataDetails.EntryDataId LIKE N'Asycuda-C#%')
GROUP BY EntryDataDetails.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, EntryDataDetails.Cost, PreviousItemsEx.Prev_reg_nbr, 
                 PreviousItemsEx.CNumber, PreviousItemsEx.Current_item_number, EntryDataDetails.LineNumber, EntryDataDetails.EntryDataDetailsId, PreviousItemsEx.Previous_item_number



--Set the EffectiveDate and Invoice date Back to before start of Real sales

--UPDATE EntryDataDetails
--SET         EffectiveDate = '12/31/2020'
--FROM    EntryDataDetails INNER JOIN
--                 EntryData ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id INNER JOIN
--                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
--WHERE (EntryData.ApplicationSettingsId = 7)


--UPDATE EntryData
--SET         EntryDataDate = '12/31/2020'
--FROM    EntryDataDetails INNER JOIN
--                 EntryData ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id INNER JOIN
--                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
--WHERE (EntryData.ApplicationSettingsId = 7)


SELECT InventoryItems.Description, InventoryItems.ItemNumber, InventoryItems.ApplicationSettingsId
FROM    InventoryItems INNER JOIN
                 [InventoryItems-Lumped] ON InventoryItems.Id = [InventoryItems-Lumped].InventoryItemId
GROUP BY InventoryItems.Description, InventoryItems.ItemNumber, InventoryItems.ApplicationSettingsId