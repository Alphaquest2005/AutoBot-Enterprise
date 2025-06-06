USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PreDiscrepancyErrors-AsycudaDocumentItem]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[TODO-PreDiscrepancyErrors-AsycudaDocumentItem]
AS
SELECT xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id AS AsycudaDocumentId, CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) AS ItemQuantity, 
                 Primary_Supplementary_Unit.Suppplementary_unit_code, PiQuantities.PiWeight, PiQuantities.PiQuantity
FROM    (SELECT  Amount_deducted_from_licence, Quantity_deducted_from_licence, Item_Id, ASYCUDA_Id, Licence_number, Free_text_1, Free_text_2, EntryDataDetailsId, LineNumber, IsAssessed, 
                                  DPQtyAllocated, DFQtyAllocated, EntryTimeStamp, AttributeOnlyAllocation, DoNotAllocate, DoNotEX, ImportComplete, WarehouseError, SalesFactor, PreviousInvoiceNumber, PreviousInvoiceLineNumber, 
                                  PreviousInvoiceItemNumber
                 FROM     xcuda_Item AS xcuda_Item_1 WITH (NOLOCK)
                ) AS xcuda_Item LEFT OUTER JOIN
                    dbo.[AscyudaItemPiQuantity-basic] AS PiQuantities ON xcuda_Item.Item_Id = PiQuantities.Item_Id LEFT OUTER JOIN
                 Primary_Supplementary_Unit WITH (NOLOCK) RIGHT OUTER JOIN
                 xcuda_Tarification WITH (NOLOCK) ON Primary_Supplementary_Unit.Tarification_Id = xcuda_Tarification.Item_Id ON xcuda_Item.Item_Id = xcuda_Tarification.Item_Id CROSS JOIN
                 xcuda_Inventory_Item
GROUP BY xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id, xcuda_Item.LineNumber, Primary_Supplementary_Unit.Suppplementary_unit_code, PiQuantities.PiWeight, PiQuantities.PiQuantity, 
                 Primary_Supplementary_Unit.Suppplementary_unit_quantity
GO
