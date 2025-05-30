USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesAsycudaPiSummary-Asycuda]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ItemSalesAsycudaPiSummary-Asycuda]
AS
	SELECT   PreviousItem_Id, ItemNumber, SUM(PiQuantity) AS PiQuantity, pCNumber, pLineNumber, ApplicationSettingsId, Type, MonthYear, Max(EntryDataDate) as EntryDataDate, DutyFreePaid, EntryDataType
	FROM      (SELECT   AsycudaItemPiQuantityData.Item_Id AS PreviousItem_Id, AsycudaItemPiQuantityData.ItemNumber, ISNULL(AsycudaItemPiQuantityData.PiQuantity, 0) AS PiQuantity, AsycudaItemPiQuantityData.pCNumber, 
					AsycudaItemPiQuantityData.pLineNumber, AsycudaItemPiQuantityData.ApplicationSettingsId, NULL AS Type, CONVERT(varchar(7), AsycudaItemPiQuantityData.AssessmentDate, 126) AS MonthYear, 
					AsycudaItemPiQuantityData.AssessmentDate AS EntryDataDate, AsycudaItemPiQuantityData.DutyFreePaid, AsycudaItemPiQuantityData.EntryDataType, AsycudaItemPiQuantityData.xItem_Id
				FROM      AsycudaItemPiQuantityData LEFT OUTER JOIN
								[InventoryItemAliasEx-NoReverseMappings] AS InventoryItemAliasEx ON AsycudaItemPiQuantityData.ItemNumber = InventoryItemAliasEx.ItemNumber
				GROUP BY AsycudaItemPiQuantityData.Item_Id, AsycudaItemPiQuantityData.ItemNumber, AsycudaItemPiQuantityData.pCNumber, AsycudaItemPiQuantityData.pLineNumber, AsycudaItemPiQuantityData.ApplicationSettingsId, 
								AsycudaItemPiQuantityData.AssessmentDate, AsycudaItemPiQuantityData.DutyFreePaid, AsycudaItemPiQuantityData.EntryDataType, AsycudaItemPiQuantityData.xItem_Id, AsycudaItemPiQuantityData.PiQuantity
                 UNION
                 SELECT   AsycudaItemPiQuantityData_1.Item_Id AS PreviousItem_Id, InventoryItemAliasEx_1.ItemNumber, ISNULL(AsycudaItemPiQuantityData_1.PiQuantity, 0) AS PiQuantity, AsycudaItemPiQuantityData_1.pCNumber, 
                                 AsycudaItemPiQuantityData_1.pLineNumber, AsycudaItemPiQuantityData_1.ApplicationSettingsId, NULL AS Type, CONVERT(varchar(7), AsycudaItemPiQuantityData_1.AssessmentDate, 126) AS MonthYear, 
                                 AsycudaItemPiQuantityData_1.AssessmentDate AS EntryDataDate, AsycudaItemPiQuantityData_1.DutyFreePaid, AsycudaItemPiQuantityData_1.EntryDataType, AsycudaItemPiQuantityData_1.xItem_Id
                 FROM      AsycudaItemPiQuantityData AS AsycudaItemPiQuantityData_1 INNER JOIN
                              [InventoryItemAliasEx-NoReverseMappings] as  InventoryItemAliasEx_1 ON AsycudaItemPiQuantityData_1.ItemNumber = InventoryItemAliasEx_1.AliasName
                 GROUP BY AsycudaItemPiQuantityData_1.Item_Id, InventoryItemAliasEx_1.ItemNumber, AsycudaItemPiQuantityData_1.pCNumber, AsycudaItemPiQuantityData_1.pLineNumber, AsycudaItemPiQuantityData_1.ApplicationSettingsId, 
                                 AsycudaItemPiQuantityData_1.AssessmentDate, AsycudaItemPiQuantityData_1.DutyFreePaid, AsycudaItemPiQuantityData_1.EntryDataType, AsycudaItemPiQuantityData_1.PiQuantity, AsycudaItemPiQuantityData_1.xItem_Id) 
                AS t
GROUP BY PreviousItem_Id, ItemNumber, pCNumber, pLineNumber, ApplicationSettingsId, Type, MonthYear,  DutyFreePaid, EntryDataType
--select * from InventoryItemAliasEx where ItemNumber in ('SEH/1002GH','SEH/1002G', 'SEH-1002GH') 
GO
