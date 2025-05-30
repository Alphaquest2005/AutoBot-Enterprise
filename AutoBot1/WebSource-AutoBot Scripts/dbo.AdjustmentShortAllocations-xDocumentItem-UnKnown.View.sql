USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations-xDocumentItem-UnKnown]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AdjustmentShortAllocations-xDocumentItem-UnKnown]
AS

--union
/* AND (PreviousItemsEx.Prev_reg_nbr = '31380') AND (PreviousItemsEx.Previous_item_number = '163')*/
SELECT AsycudaSalesAllocations.AllocationId, XBondItem.LineNumber AS xLineNumber, XBondItem.Item_Id AS xBond_Item_Id, xReg.Number AS xCNumber, xReg.xRegistrationDate, 
                 XBondItem_SuppUnit.Suppplementary_unit_quantity AS xQuantity, XBondItem.ASYCUDA_Id AS xASYCUDA_Id, xDeclarant.Number AS xReferenceNumber
FROM    EntryPreviousItems INNER JOIN
                 Primary_Supplementary_Unit AS XBondItem_SuppUnit WITH (NOLOCK) INNER JOIN
                     (SELECT Number, CAST(Date AS DateTime) AS xRegistrationDate, ASYCUDA_Id
                      FROM     xcuda_Registration WITH (NOLOCK)) AS xReg INNER JOIN
                 xcuda_Item AS XBondItem WITH (NOLOCK) ON xReg.ASYCUDA_Id = XBondItem.ASYCUDA_Id INNER JOIN
                 xcuda_Declarant AS xDeclarant WITH (Nolock) ON XBondItem.ASYCUDA_Id = xDeclarant.ASYCUDA_Id ON XBondItem_SuppUnit.Tarification_Id = XBondItem.Item_Id INNER JOIN
                 xcuda_PreviousItem AS PreviousItemsEx ON XBondItem.Item_Id = PreviousItemsEx.PreviousItem_Id AND XBondItem_SuppUnit.Suppplementary_unit_quantity = PreviousItemsEx.Suplementary_Quantity AND 
                 XBondItem.LineNumber = PreviousItemsEx.Current_item_number AND xDeclarant.ASYCUDA_Id = PreviousItemsEx.ASYCUDA_Id AND xReg.ASYCUDA_Id = PreviousItemsEx.ASYCUDA_Id ON 
                 EntryPreviousItems.PreviousItem_Id = PreviousItemsEx.PreviousItem_Id AND EntryPreviousItems.PreviousItem_Id = XBondItem.Item_Id INNER JOIN
                 AsycudaSalesAllocations ON EntryPreviousItems.Item_Id = AsycudaSalesAllocations.PreviousItem_Id INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON XBondItem.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id LEFT OUTER JOIN
                 AsycudaDocumentItemEntryDataDetails ON EntryPreviousItems.Item_Id = AsycudaDocumentItemEntryDataDetails.Item_Id AND 
                 AsycudaSalesAllocations.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (xReg.Number IS NOT NULL) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL) AND (ISNULL(xcuda_ASYCUDA_ExtendedProperties.Cancelled, 0) = 0)
GO
