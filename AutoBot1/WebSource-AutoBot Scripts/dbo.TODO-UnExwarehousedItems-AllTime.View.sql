USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-UnExwarehousedItems-AllTime]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-UnExwarehousedItems-AllTime]
AS
SELECT  EX9AsycudaSalesAllocations.ItemNumber, 
                 EX9AsycudaSalesAllocations.ItemDescription, ApplicationSettings.ApplicationSettingsId, SUM(EX9AsycudaSalesAllocations.QtyAllocated) AS QtyAllocated, EX9AsycudaSalesAllocations.Status, 
                 EX9AsycudaSalesAllocations.xStatus, EX9AsycudaSalesAllocations.pReferenceNumber, EX9AsycudaSalesAllocations.pLineNumber, EX9AsycudaSalesAllocations.pCNumber, EX9AsycudaSalesAllocations.EmailId, 
                 sum(AsycudaItemPiQuantityData.PiQuantity) AS PiQuantity
FROM    EX9AsycudaSalesAllocations INNER JOIN
                 ApplicationSettings ON EX9AsycudaSalesAllocations.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId AND 
                 EX9AsycudaSalesAllocations.pRegistrationDate >= ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                 AsycudaItemPiQuantityData ON EX9AsycudaSalesAllocations.PreviousItem_Id = AsycudaItemPiQuantityData.Item_Id  AND 
                 ApplicationSettings.ApplicationSettingsId = AsycudaItemPiQuantityData.ApplicationSettingsId AND 
                 EX9AsycudaSalesAllocations.ApplicationSettingsId = AsycudaItemPiQuantityData.ApplicationSettingsId LEFT OUTER JOIN
                 AllocationErrors ON ApplicationSettings.ApplicationSettingsId = AllocationErrors.ApplicationSettingsId AND EX9AsycudaSalesAllocations.ItemNumber = AllocationErrors.ItemNumber
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.Customs_ProcedureId = EX9AsycudaSalesAllocations.Customs_ProcedureId
WHERE (EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND 
                 (EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (EX9AsycudaSalesAllocations.Status IS NULL OR
                 EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND 
                 (ISNULL(EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (EX9AsycudaSalesAllocations.WarehouseError IS NULL) 

				 --AND (EX9AsycudaSalesAllocations.DocumentType = 'IM7' OR  EX9AsycudaSalesAllocations.DocumentType = 'OS7') 
				 AND (AllocationErrors.ItemNumber IS NULL)
GROUP BY EX9AsycudaSalesAllocations.ItemNumber, ApplicationSettings.ApplicationSettingsId, EX9AsycudaSalesAllocations.Status, EX9AsycudaSalesAllocations.xStatus, EX9AsycudaSalesAllocations.ItemDescription, EX9AsycudaSalesAllocations.pReferenceNumber, 
                 EX9AsycudaSalesAllocations.pLineNumber, EX9AsycudaSalesAllocations.pCNumber, EX9AsycudaSalesAllocations.EmailId--, AsycudaItemPiQuantityData.PiQuantity
HAVING (SUM(EX9AsycudaSalesAllocations.QtyAllocated) > 0) AND (MAX(EX9AsycudaSalesAllocations.xStatus) IS NULL) AND (sum(AsycudaItemPiQuantityData.PiQuantity) < SUM(EX9AsycudaSalesAllocations.QtyAllocated))
GO
