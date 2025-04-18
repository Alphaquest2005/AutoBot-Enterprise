USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations-xDocumentItem-Known-1]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AdjustmentShortAllocations-xDocumentItem-Known-1]
AS
SELECT xBondAllocations.AllocationId, XBondItem.LineNumber AS xLineNumber, XBondItem.Item_Id AS xBond_Item_Id, ISNULL(xReg.Number, N'Unassessed') AS xCNumber, isnull(xReg.Date,  cast('1900-01-01 00:00:00' as datetime))  AS xRegistrationDate, 
                 XBondItem_SuppUnit.Suppplementary_unit_quantity AS xQuantity, XBondItem.ASYCUDA_Id AS xASYCUDA_Id, xDeclarant.Number AS xReferenceNumber
FROM    Primary_Supplementary_Unit AS XBondItem_SuppUnit WITH (NOLOCK) INNER JOIN
                 xcuda_Registration AS xReg WITH (NOLOCK) INNER JOIN
                 xcuda_Item AS XBondItem WITH (NOLOCK) INNER JOIN
                     (SELECT xEntryItem_Id, AllocationId, Status
                      FROM     xBondAllocations AS xBondAllocations_1 WITH (NOLOCK)
                      /*GROUP BY xEntryItem_Id, AllocationId, Status*/) AS xBondAllocations ON XBondItem.Item_Id = xBondAllocations.xEntryItem_Id ON xReg.ASYCUDA_Id = XBondItem.ASYCUDA_Id INNER JOIN
                 xcuda_Declarant AS xDeclarant WITH (Nolock) ON XBondItem.ASYCUDA_Id = xDeclarant.ASYCUDA_Id ON XBondItem_SuppUnit.Tarification_Id = XBondItem.Item_Id INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON XBondItem.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE (xReg.Number IS NULL) AND (xcuda_ASYCUDA_ExtendedProperties.Cancelled = 0)
GO
