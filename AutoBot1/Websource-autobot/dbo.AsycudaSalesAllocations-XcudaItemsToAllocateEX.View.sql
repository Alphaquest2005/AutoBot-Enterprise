USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaSalesAllocations-XcudaItemsToAllocateEX]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


Create VIEW [dbo].[AsycudaSalesAllocations-XcudaItemsToAllocateEX]
AS
SELECT   [AsycudaSalesAllocations-XcudaItemsToAllocate].Item_Id, [AsycudaSalesAllocations-XcudaItemsToAllocate].ApplicationSettingsId, [AsycudaSalesAllocations-XcudaItemsToAllocate].AsycudaDocumentSetId, 
                [AsycudaSalesAllocations-XcudaItemsToAllocate].AssessmentDate, AsycudaItemBasicInfo.ItemNumber, AsycudaItemBasicInfo.ASYCUDA_Id, AsycudaItemBasicInfo.Commercial_Description, AsycudaItemBasicInfo.TariffCode, 
                AsycudaItemBasicInfo.DPQtyAllocated, AsycudaItemBasicInfo.ItemQuantity, AsycudaItemBasicInfo.DFQtyAllocated, AsycudaItemBasicInfo.IsAssessed, AsycudaItemBasicInfo.LineNumber, AsycudaItemBasicInfo.CNumber, 
                AsycudaItemBasicInfo.RegistrationDate, AsycudaItemBasicInfo.AsycudaDocumentSetId AS Expr1, AsycudaItemBasicInfo.EntryDataType
FROM      [AsycudaSalesAllocations-XcudaItemsToAllocate] INNER JOIN
                AsycudaItemBasicInfo ON [AsycudaSalesAllocations-XcudaItemsToAllocate].Item_Id = AsycudaItemBasicInfo.Item_Id
GO
