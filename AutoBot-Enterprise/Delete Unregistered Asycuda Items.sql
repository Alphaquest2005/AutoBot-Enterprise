declare @itemId int
set @itemId = (select AsycudaDocumentId from AsycudaDocumentItem where ApplicationSettingsId = 2 and
																				ImportComplete = 0 and
																				linenumber = 190 and
																				ReferenceNumber = 'CleanUp2020-F69')
select @itemId
delete from xcuda_item where item_id = @itemId


select ItemQuantity - PiQuantity as diff, * from AsycudaDocumentItem 
WHERE (Item_Id IN
                     (SELECT PreviousDocumentItemId
                      FROM     PreviousItemsEx INNER JOIN
                                       AsycudaDocumentBasicInfo ON PreviousItemsEx.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
                      WHERE  (PreviousItemsEx.ApplicationSettingsId = 2) AND
					  (PreviousItemsEx.Prev_reg_nbr + '-' + PreviousItemsEx.Previous_item_number = '18356-8') 
								and ReferenceNumber = 'November 2020-F4'
					  AND (AsycudaDocumentBasicInfo.ImportComplete = 0)
                      GROUP BY PreviousItemsEx.PreviousDocumentItemId))


select Net_weight, Suplementary_Quantity, LineNumber 
FROM    xcuda_PreviousItem AS xcuda_PreviousItem_1 INNER JOIN
                 xcuda_Item ON xcuda_PreviousItem_1.PreviousItem_Id = xcuda_Item.Item_Id 
WHERE (xcuda_Item.Item_Id IN
                     (SELECT PreviousItemsEx.PreviousItem_Id
                      FROM     PreviousItemsEx INNER JOIN
                                       AsycudaDocumentBasicInfo ON PreviousItemsEx.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
                      WHERE  (PreviousItemsEx.ApplicationSettingsId = 2) AND
					  (PreviousItemsEx.Prev_reg_nbr + '-' + PreviousItemsEx.Previous_item_number = '18356-8') 
								and ReferenceNumber = 'November 2020-F4'
					  AND (AsycudaDocumentBasicInfo.ImportComplete = 0)
                      GROUP BY PreviousItemsEx.PreviousItem_Id))

UPDATE xcuda_PreviousItem
SET         Net_weight = 0.02
			,Suplementary_Quantity = 11
FROM    xcuda_PreviousItem AS xcuda_PreviousItem_1 INNER JOIN
                 xcuda_Item ON xcuda_PreviousItem_1.PreviousItem_Id = xcuda_Item.Item_Id 
WHERE (xcuda_Item.Item_Id IN
                     (SELECT PreviousItemsEx.PreviousItem_Id
                      FROM     PreviousItemsEx INNER JOIN
                                       AsycudaDocumentBasicInfo ON PreviousItemsEx.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
                      WHERE  (PreviousItemsEx.ApplicationSettingsId = 2) AND
					 (PreviousItemsEx.Prev_reg_nbr + '-' + PreviousItemsEx.Previous_item_number = '18356-8') 
								and ReferenceNumber = 'November 2020-F4'
					  AND (AsycudaDocumentBasicInfo.ImportComplete = 0)
                      GROUP BY PreviousItemsEx.PreviousItem_Id))

--delete from xcuda_Item
--where Item_id= @itemId

SELECT PreviousItemsEx.PreviousItem_Id, PreviousItemsEx.Prev_reg_ser, PreviousItemsEx.Prev_reg_nbr, PreviousItemsEx.Prev_reg_dat, PreviousItemsEx.Prev_reg_cuo, PreviousItemsEx.Suplementary_Quantity, 
                 PreviousItemsEx.Preveious_suplementary_quantity, PreviousItemsEx.Current_value, PreviousItemsEx.Previous_value, PreviousItemsEx.QtyAllocated, PreviousItemsEx.RndCurrent_Value, 
                 PreviousItemsEx.ReferenceNumber, PreviousItemsEx.CNumber, PreviousItemsEx.RegistrationDate, PreviousItemsEx.AssessmentDate, PreviousItemsEx.SalesFactor, PreviousItemsEx.Prev_decl_HS_spec, 
                 PreviousItemsEx.DocumentType, PreviousItemsEx.DutyFreePaid, PreviousItemsEx.ItemNumber
FROM    PreviousItemsEx INNER JOIN
                 AsycudaDocumentBasicInfo ON PreviousItemsEx.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE (PreviousItemsEx.ApplicationSettingsId = 2) AND (PreviousItemsEx.Prev_reg_nbr = '37794') AND (PreviousItemsEx.Previous_item_number = '119') AND (AsycudaDocumentBasicInfo.ImportComplete = 0)
GROUP BY PreviousItemsEx.PreviousItem_Id, PreviousItemsEx.Prev_reg_ser, PreviousItemsEx.Prev_reg_nbr, PreviousItemsEx.Prev_reg_dat, PreviousItemsEx.Prev_reg_cuo, PreviousItemsEx.Suplementary_Quantity, 
                 PreviousItemsEx.Preveious_suplementary_quantity, PreviousItemsEx.Current_value, PreviousItemsEx.Previous_value, PreviousItemsEx.QtyAllocated, PreviousItemsEx.RndCurrent_Value, 
                 PreviousItemsEx.ReferenceNumber, PreviousItemsEx.CNumber, PreviousItemsEx.RegistrationDate, PreviousItemsEx.AssessmentDate, PreviousItemsEx.SalesFactor, PreviousItemsEx.Prev_decl_HS_spec, 
                 PreviousItemsEx.DocumentType, PreviousItemsEx.DutyFreePaid, PreviousItemsEx.ItemNumber

