USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations-xDocumentItem-Known-3]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create VIEW [dbo].[AdjustmentShortAllocations-xDocumentItem-Known-3]
AS
SELECT        AsycudaSalesAllocations.AllocationId, XBondItem.LineNumber AS xLineNumber, XBondItem.Item_Id AS xBond_Item_Id, xReg.Number AS xCNumber, xReg.xRegistrationDate, 
                         XBondItem_SuppUnit.Suppplementary_unit_quantity AS xQuantity, XBondItem.ASYCUDA_Id AS xASYCUDA_Id, xDeclarant.Number AS xReferenceNumber
FROM            Primary_Supplementary_Unit AS XBondItem_SuppUnit WITH (NOLOCK) INNER JOIN
                             (SELECT        Number, Date AS xRegistrationDate, ASYCUDA_Id
                               FROM            xcuda_Registration WITH (NOLOCK)) AS xReg INNER JOIN
                         xcuda_Item AS XBondItem WITH (NOLOCK) ON xReg.ASYCUDA_Id = XBondItem.ASYCUDA_Id INNER JOIN
                         xcuda_Declarant AS xDeclarant WITH (Nolock) ON XBondItem.ASYCUDA_Id = xDeclarant.ASYCUDA_Id ON XBondItem_SuppUnit.Tarification_Id = XBondItem.Item_Id INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON xReg.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                         AsycudaSalesAllocations ON XBondItem.Item_Id = AsycudaSalesAllocations.xEntryItem_Id LEFT OUTER JOIN
                         [AsycudaDocumentItemEntryDataDetails-Basic] AS AsycudaDocumentItemEntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId AND 
                         XBondItem.Item_Id = AsycudaDocumentItemEntryDataDetails.Item_Id
WHERE        (xReg.Number IS NOT NULL) AND (xcuda_ASYCUDA_ExtendedProperties.Cancelled IS NULL OR
                         xcuda_ASYCUDA_ExtendedProperties.Cancelled = 0) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL)
GO
