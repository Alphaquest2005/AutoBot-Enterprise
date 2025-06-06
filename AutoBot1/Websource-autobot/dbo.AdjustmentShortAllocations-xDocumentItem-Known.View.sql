USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations-xDocumentItem-Known]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AdjustmentShortAllocations-xDocumentItem-Known]
AS
SELECT AllocationId, xLineNumber, xBond_Item_Id, xCNumber, xRegistrationDate, xQuantity, xASYCUDA_Id, xReferenceNumber
FROM    [AdjustmentShortAllocations-xDocumentItem-Known-1]


union
/* AND (AsycudaSalesAllocations.AllocationId IN (74215, 74270, 74692, 75504, 75545))*/
/* AND (PreviousItemsEx.Prev_reg_nbr = '31508') AND (PreviousItemsEx.Previous_item_number = '229')*/
/*AND (PreviousItemsEx.Prev_reg_nbr = '31380') AND (PreviousItemsEx.Previous_item_number = '163')*/
SELECT AllocationId, xLineNumber, xBond_Item_Id, xCNumber, xRegistrationDate, xQuantity, xASYCUDA_Id, xReferenceNumber
FROM    [AdjustmentShortAllocations-xDocumentItem-Known-2]

union 

SELECT AllocationId, xLineNumber, xBond_Item_Id, xCNumber, xRegistrationDate, xQuantity, xASYCUDA_Id, xReferenceNumber
FROM    [AdjustmentShortAllocations-xDocumentItem-Known-3]
GO
