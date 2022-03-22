SELECT PreviousItems.xWarehouseError, AsycudaDocument.ImportComplete, AsycudaItemBasicInfo.ItemNumber, AsycudaItemBasicInfo.Commercial_Description, AsycudaItemBasicInfo.ItemQuantity, 
                 AsycudaDocument.ReferenceNumber
FROM    AsycudaItemBasicInfo INNER JOIN
                     (SELECT Packages_number, Previous_Packages_number, Hs_code, Commodity_code, Previous_item_number, Goods_origin, Net_weight, Prev_net_weight, Prev_reg_ser, Prev_reg_nbr, Prev_reg_dat, Prev_reg_cuo, 
                                       Suplementary_Quantity, Preveious_suplementary_quantity, Current_value, Previous_value, Current_item_number, PreviousItem_Id, ASYCUDA_Id, QtyAllocated, PreviousDocumentItemId, RndCurrent_Value, 
                                       ReferenceNumber, CNumber, RegistrationDate, AsycudaDocumentItemId, AssessmentDate, Prev_decl_HS_spec, SalesFactor, DocumentType, DutyFreePaid, ItemNumber, pLineNumber, 
                                       ApplicationSettingsId, TotalDutyLiablity, DutyLiablity
                      FROM     PreviousItemsEx) AS xcuda_PreviousItem ON AsycudaItemBasicInfo.Item_Id = xcuda_PreviousItem.PreviousItem_Id INNER JOIN
                 AsycudaDocument ON AsycudaItemBasicInfo.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id INNER JOIN
                 xcuda_Item AS PreviousItems ON xcuda_PreviousItem.PreviousDocumentItemId = PreviousItems.Item_Id
WHERE (PreviousItems.xWarehouseError IS NOT NULL) AND (AsycudaDocument.ImportComplete IS NULL OR
                 AsycudaDocument.ImportComplete = 0)


SELECT AsycudaDocumentItem.*, xcuda_Item.xWarehouseError, AsycudaDocumentItem.CNumber AS Expr1, AsycudaDocumentItem.LineNumber AS Expr2, xWarehouseError
FROM    AsycudaDocumentItem INNER JOIN
                 xcuda_Item ON AsycudaDocumentItem.Item_Id = xcuda_Item.Item_Id
WHERE (AsycudaDocumentItem.CNumber = N'46491') AND (AsycudaDocumentItem.LineNumber = 4)