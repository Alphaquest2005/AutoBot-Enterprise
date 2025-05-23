USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetPackagesDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AsycudaDocumentSetPackagesDetails]
AS
SELECT TOP (100) PERCENT xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, SUM(xcuda_Packages.Number_of_packages) AS Packages, xcuda_Declarant.Number, EntryData.EntryDataId
FROM    xcuda_Item INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON xcuda_Item.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 xcuda_Declarant ON xcuda_Item.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id INNER JOIN
                 AsycudaDocumentEntryData ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaDocumentEntryData.AsycudaDocumentId INNER JOIN
                 EntryData ON AsycudaDocumentEntryData.EntryData_Id = EntryData.EntryData_Id LEFT OUTER JOIN
                 xcuda_Packages ON xcuda_Item.Item_Id = xcuda_Packages.Item_Id
WHERE (xcuda_Packages.Number_of_packages IS NOT NULL)
GROUP BY xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, xcuda_Declarant.Number, xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id, EntryData.EntryDataId
ORDER BY xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
GO
