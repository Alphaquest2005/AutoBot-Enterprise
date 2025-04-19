USE [AutoBot-EnterpriseDB]
GO

/****** Object:  View [dbo].[TODO-AdjustmentsToXML]    Script Date: 2/27/2020 10:38:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER VIEW [dbo].[TODO-AdjustmentsToXML]
AS
SELECT EntryDataDetailsId, ApplicationSettingsId, AsycudaDocumentSetId, IsClassified, AdjustmentType, InvoiceNo, InvoiceQty, ReceivedQty, InvoiceDate, ItemNumber, Status, CNumber, Declarant_Reference_Number
FROM    (SELECT t.EntryDataDetailsId, t.ApplicationSettingsId, t.AsycudaDocumentSetId, t.IsClassified, t.AdjustmentType, t.InvoiceNo, t.InvoiceQty, t.ReceivedQty, t.InvoiceDate, t.ItemNumber, t.Status, t.CNumber, 
                                  t.Declarant_Reference_Number
                 FROM     (SELECT TOP (100) PERCENT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
                                                    CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, 
                                                    EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, 
                                                    AsycudaDocumentSet.Declarant_Reference_Number
                                   FROM     ApplicationSettings INNER JOIN
                                                    EntryData ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                                                    AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                                                    AsycudaDocumentSet ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                                                    AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                                                    InventoryItems ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                                                    EntryDataDetails ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
                                                    EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                                                    AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                                                    SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
                                   WHERE  (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) AND (ISNULL(EntryDataDetails.DoNotAllocate, 0) <> 1) AND (ISNULL(EntryDataDetails.IsReconciled, 0) <> 1) AND 
                                                    (AsycudaSalesAllocations.xStatus IS NULL)
                                   GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                                                    THEN 0 ELSE 1 END AS bit), EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                                                    EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
                                   ORDER BY EntryDataDetails.EntryDataDetailsId) AS t FULL OUTER JOIN
                                      (SELECT TOP (100) PERCENT EntryDataDetailsId, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, Quantity, ImportComplete
                                       FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
                                       WHERE  (DocumentType NOT IN ('IM7', 'EX9'))
                                       ORDER BY EntryDataDetailsId) AS AsycudaDocumentItemEntryDataDetails ON t.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId AND 
                                  (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL OR
                                  AsycudaDocumentItemEntryDataDetails.DocumentType <> CASE WHEN InvoiceQty > ReceivedQty THEN 'IM4' ELSE 'OS7' END)) AS xx
GROUP BY EntryDataDetailsId, ApplicationSettingsId, AsycudaDocumentSetId, AdjustmentType, InvoiceNo, InvoiceQty, IsClassified, ReceivedQty, InvoiceDate, ItemNumber, Status, CNumber, 
                 Declarant_Reference_Number
HAVING (EntryDataDetailsId IS NOT NULL)

--SELECT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
--                 THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, 
--                 EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
--FROM    ApplicationSettings with(nolock) INNER JOIN
--                 EntryData with(nolock) ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
--                 AsycudaDocumentSetEntryData with(nolock) ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
--                 AsycudaDocumentSet with(nolock) ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
--                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
--                 InventoryItems with(nolock) ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
--                 EntryDataDetails with(nolock) ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
--                 EntryData_Adjustments with(nolock) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
--                 AsycudaSalesAllocations with(nolock) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
--                 SystemDocumentSets with(nolock) ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
--                  (select * from AsycudaDocumentItemEntryDataDetails with(nolock) where AsycudaDocumentItemEntryDataDetails.DocumentType not in ('IM7','EX9') ) as AsycudaDocumentItemEntryDataDetails ON EntryDataDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
--WHERE (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) and isnull(EntryDataDetails.DoNotAllocate,0) <> 1 and isnull(EntryDataDetails.IsReconciled,0) <> 1  AND (AsycudaSalesAllocations.xStatus IS NULL) and (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL or AsycudaDocumentItemEntryDataDetails.DocumentType <> case when InvoiceQty > ReceivedQty then 'IM4' else 'OS7' end) 

--GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
--                 THEN 0 ELSE 1 END AS bit), EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
--                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
GO

select * from [dbo].[TODO-AdjustmentsToXML]


select t.* from

(SELECT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, 
                 EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
FROM    ApplicationSettings with(nolock) INNER JOIN
                 EntryData with(nolock) ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 AsycudaDocumentSetEntryData with(nolock) ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet with(nolock) ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 InventoryItems with(nolock) ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                 EntryDataDetails with(nolock) ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
                 EntryData_Adjustments with(nolock) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                 AsycudaSalesAllocations with(nolock) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                 SystemDocumentSets with(nolock) ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id 
WHERE  (entrydatadetails.EntryDataDetailsId = 651519) and (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) and isnull(EntryDataDetails.DoNotAllocate,0) <> 1 and isnull(EntryDataDetails.IsReconciled,0) <> 1  AND (AsycudaSalesAllocations.xStatus IS NULL) 
GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit), EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
				 ) as t
LEFT OUTER JOIN
                  (select * from AsycudaDocumentItemEntryDataDetails with(nolock) where  AsycudaDocumentItemEntryDataDetails.DocumentType not in ('IM7','EX9') ) as AsycudaDocumentItemEntryDataDetails ON t.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId

				  where  (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL or AsycudaDocumentItemEntryDataDetails.DocumentType <> case when InvoiceQty > ReceivedQty then 'IM4' else 'OS7' end) 


SELECT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, 
                 EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
FROM    ApplicationSettings with(nolock) INNER JOIN
                 EntryData with(nolock) ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 AsycudaDocumentSetEntryData with(nolock) ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet with(nolock) ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 InventoryItems with(nolock) ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                 EntryDataDetails with(nolock) ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
                 EntryData_Adjustments with(nolock) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                 AsycudaSalesAllocations with(nolock) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                 SystemDocumentSets with(nolock) ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
                  (select * from AsycudaDocumentItemEntryDataDetails with(nolock) where AsycudaDocumentItemEntryDataDetails.DocumentType not in ('IM7','EX9') ) as AsycudaDocumentItemEntryDataDetails ON EntryDataDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (entrydatadetails.EntryDataId = 'ADJ-September 2018') and (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) and isnull(EntryDataDetails.DoNotAllocate,0) <> 1 and isnull(EntryDataDetails.IsReconciled,0) <> 1  AND (AsycudaSalesAllocations.xStatus IS NULL) and (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL or AsycudaDocumentItemEntryDataDetails.DocumentType <> case when InvoiceQty > ReceivedQty then 'IM4' else 'OS7' end) 

GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit), EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number



SELECT EntryDataDetailsId, ApplicationSettingsId, AsycudaDocumentSetId, IsClassified, AdjustmentType, InvoiceNo, InvoiceQty, ReceivedQty, InvoiceDate, ItemNumber, Status, CNumber, Declarant_Reference_Number
FROM    (SELECT t.EntryDataDetailsId, t.ApplicationSettingsId, t.AsycudaDocumentSetId, t.IsClassified, t.AdjustmentType, t.InvoiceNo, t.InvoiceQty, t.ReceivedQty, t.InvoiceDate, t.ItemNumber, t.Status, t.CNumber, 
                                  t.Declarant_Reference_Number
                 FROM     (SELECT TOP (100) PERCENT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 
                                                    CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, 
                                                    EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, 
                                                    AsycudaDocumentSet.Declarant_Reference_Number
                                   FROM     ApplicationSettings INNER JOIN
                                                    EntryData ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                                                    AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                                                    AsycudaDocumentSet ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                                                    AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                                                    InventoryItems ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                                                    EntryDataDetails ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
                                                    EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                                                    AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                                                    SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
                                   WHERE  (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) AND (ISNULL(EntryDataDetails.DoNotAllocate, 0) <> 1) AND (ISNULL(EntryDataDetails.IsReconciled, 0) <> 1) AND 
                                                    (AsycudaSalesAllocations.xStatus IS NULL)
                                   GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                                                    THEN 0 ELSE 1 END AS bit), EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                                                    EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
                                   ORDER BY EntryDataDetails.EntryDataDetailsId) AS t FULL OUTER JOIN
                                      (SELECT TOP (100) PERCENT EntryDataDetailsId, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, Quantity, ImportComplete
                                       FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
                                       WHERE  (DocumentType NOT IN ('IM7', 'EX9'))
                                       ORDER BY EntryDataDetailsId) AS AsycudaDocumentItemEntryDataDetails ON t.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId AND 
                                  (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL OR
                                  AsycudaDocumentItemEntryDataDetails.DocumentType <> CASE WHEN InvoiceQty > ReceivedQty THEN 'IM4' ELSE 'OS7' END)) AS xx
GROUP BY EntryDataDetailsId, ApplicationSettingsId, AsycudaDocumentSetId, AdjustmentType, InvoiceNo, InvoiceQty, IsClassified, ReceivedQty, InvoiceDate, ItemNumber, Status, CNumber, 
                 Declarant_Reference_Number
HAVING (EntryDataDetailsId IS NOT NULL) and EntryDataDetailsId = 651519



where xx.EntryDataDetailsId  is not null


= 651519





	select t.* from

(SELECT top 100 percent EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit) AS IsClassified, EntryData_Adjustments.Type AS AdjustmentType, EntryData.EntryDataId AS InvoiceNo, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, 
                 EntryData.EntryDataDate AS InvoiceDate, EntryDataDetails.ItemNumber, EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
FROM    ApplicationSettings with(nolock) INNER JOIN
                 EntryData with(nolock) ON ApplicationSettings.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 AsycudaDocumentSetEntryData with(nolock) ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSet with(nolock) ON ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId AND 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 InventoryItems with(nolock) ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                 EntryDataDetails with(nolock) ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN
                 EntryData_Adjustments with(nolock) ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id AND EntryDataDetails.EntryData_Id = EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                 AsycudaSalesAllocations with(nolock) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                 SystemDocumentSets with(nolock) ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id 
WHERE  (SystemDocumentSets.Id IS NULL) AND (EntryData_Adjustments.Type IS NOT NULL) and isnull(EntryDataDetails.DoNotAllocate,0) <> 1 and isnull(EntryDataDetails.IsReconciled,0) <> 1  AND (AsycudaSalesAllocations.xStatus IS NULL) 
GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit), EntryData_Adjustments.Type, EntryData.EntryDataId, EntryDataDetails.InvoiceQty, EntryDataDetails.ReceivedQty, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, 
                 EntryDataDetails.Status, EntryDataDetails.CNumber, AsycudaDocumentSet.Declarant_Reference_Number
order by EntryDataDetails.EntryDataDetailsId
				 ) as t

LEFT OUTER JOIN
                  (select top 100 percent * from AsycudaDocumentItemEntryDataDetails where AsycudaDocumentItemEntryDataDetails.DocumentType not in ('IM7','EX9') order by EntryDataDetailsId ) as AsycudaDocumentItemEntryDataDetails ON t.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId

				 and (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL  or AsycudaDocumentItemEntryDataDetails.DocumentType <> case when InvoiceQty > ReceivedQty then 'IM4' else 'OS7' end) 