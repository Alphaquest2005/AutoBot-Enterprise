USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaItemPiQuantityDataEx]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[AsycudaItemPiQuantityDataEx]
AS
SELECT EntryPreviousItems.Item_Id,cast( ISNULL(xcuda_PreviousItem.Suplementary_Quantity, 0) as float) AS PiQuantity, cast(xcuda_PreviousItem.Net_weight as float)  AS PiWeight, xItemDocument.AssessmentDate, 
                 xItemBasicInfo.IsAssessed, xcuda_PreviousItem.PreviousItem_Id AS xItem_Id, xItemDocument.DocumentType, CASE WHEN AsycudaDocumentCustomsProcedures.IsPaid = 1 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, xItemBasicInfo.ItemNumber, xItemDocument.CNumber, xItemDocument.Reference, xItemDocument.ApplicationSettingsId, 
                 PreviousDocument.CNumber AS pCNumber, PreviousDocumentItem.LineNumber as pLineNumber, PreviousDocument.Reference AS pReference, cast(ROW_NUMBER() OVER (ORDER BY EntryPreviousItems.Item_Id,
                  xcuda_PreviousItem.PreviousItem_Id) AS int) AS Id, isnull(AsycudaDocumentItemEntryDataDetails.EntryDataType,'Sales') as EntryDataType, xItemBasicInfo.LineNumber as xLineNumber, PreviousDocumentItem.DFQtyAllocated, PreviousDocumentItem.DPQtyAllocated, previousdocument.ExpiryDate as pExpiryDate

FROM    EntryPreviousItems WITH (NOLOCK) INNER JOIN
                 xcuda_PreviousItem WITH (NOLOCK) ON EntryPreviousItems.PreviousItem_Id = xcuda_PreviousItem.PreviousItem_Id INNER JOIN
                 AsycudaItemBasicInfo AS xItemBasicInfo WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentBasicInfo AS xItemDocument WITH (NOLOCK) ON xItemBasicInfo.ASYCUDA_Id = xItemDocument.ASYCUDA_Id ON xcuda_PreviousItem.PreviousItem_Id = xItemBasicInfo.Item_Id INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) ON xcuda_PreviousItem.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 AsycudaItemBasicInfo AS PreviousDocumentItem ON EntryPreviousItems.Item_Id = PreviousDocumentItem.Item_Id INNER JOIN
                 AsycudaDocumentBasicInfo AS PreviousDocument ON PreviousDocumentItem.ASYCUDA_Id = PreviousDocument.ASYCUDA_Id
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = xItemDocument.ASYCUDA_Id
				 LEFT OUTER JOIN AsycudaDocumentItemEntryDataDetails on AsycudaDocumentItemEntryDataDetails.Item_Id = xItemBasicInfo.Item_Id
WHERE ((xcuda_ASYCUDA_ExtendedProperties.Cancelled IS NULL) OR
                 (xcuda_ASYCUDA_ExtendedProperties.Cancelled = 0)) and (EntryPreviousItems.Item_Id <> xcuda_PreviousItem.PreviousItem_Id)
GROUP BY xItemDocument.AssessmentDate, EntryPreviousItems.Item_Id, xcuda_PreviousItem.PreviousItem_Id, xItemBasicInfo.IsAssessed, xItemDocument.DocumentType, 
                 xItemDocument.Extended_customs_procedure, xItemBasicInfo.ItemNumber, xItemDocument.CNumber, xItemDocument.Reference, xItemDocument.ApplicationSettingsId, PreviousDocument.CNumber, 
                 PreviousDocument.Reference,
				 xcuda_PreviousItem.Suplementary_Quantity, xcuda_PreviousItem.Net_weight, previousdocument.ExpiryDate,
				 PreviousDocumentItem.LineNumber, AsycudaDocumentCustomsProcedures.IsPaid, AsycudaDocumentItemEntryDataDetails.EntryDataType,xItemBasicInfo.LineNumber, PreviousDocumentItem.DFQtyAllocated, PreviousDocumentItem.DPQtyAllocated
GO
