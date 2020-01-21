
update xcuda_Item
set WarehouseError = null
where WarehouseError = 'Cant find in Warehouse HSCode'

update xcuda_Item
set WarehouseError = 'impossible to find the given line in warehouse', DoNotEX = 1
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where number = '25008'
and LineNumber =3)

update xcuda_Item
set WarehouseError = 'Product Code Too Long', DoNotEX = 1 , DoNotAllocate = 1
where Item_id in 
(
SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where len(Precision_4) > 17)

update xcuda_Item
set WarehouseError = null, DoNotEX = null
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where Number = '12922' )
and LineNumber =6)

update xcuda_Item
set WarehouseError = 'impossible to find the given line in warehouse'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where Number = '24997')

update xcuda_Item
set WarehouseError = 'Delay Extenstion'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where Number = '18336')

update xcuda_Item
set WarehouseError = 'Customs Release', DoNotEX = 1
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where Number = '12922'
and lineNumber = 70)

update xcuda_Item
set WarehouseError = 'Server Error'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where Commodity_code = '70194000')

update xcuda_Item
set WarehouseError = 'Invalid HSCode'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where Commodity_code = '56089020')


update xcuda_Item
set WarehouseError = 'Not Enough Product available in Warehouse'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where number = '32490'
and LineNumber =16)

update xcuda_Item
set WarehouseError = 'Quantities'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where number = '16475'
and LineNumber =45)

update xcuda_Item
set WarehouseError = 'Quantities'
where Item_id in 
(SELECT AsycudaDocumentItem.Item_Id
FROM   AsycudaDocumentItem INNER JOIN
             AsycudaDocument ON AsycudaDocumentItem.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id
WHERE --(AsycudaDocumentItem.InvalidHSCode = 1) AND
	 ((AsycudaDocumentItem.ItemQuantity <> AsycudaDocumentItem.PiQuantity) or (AsycudaDocumentItem.ItemQuantity = 0)) 
	AND (AsycudaDocument.DocumentType = 'IM7')
	and (AsycudaDocument.Cnumber = '9670')
	and (isnull(AsycudaDocument.cancelled,0) <> 1 ))
	--and (isnull(AsycudaDocument.DoNotAllocate,0) = 0 ))

update xcuda_Item
set WarehouseError = 'Cant find in Warehouse HSCode'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where number = '18909' and Commodity_code = '61099010')

update xcuda_Item
set WarehouseError = 'Cancel verification'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where number = '3633'
and LineNumber = 17)

update xcuda_Item
set WarehouseError = 'Cancel verification'
where Item_id in 
(SELECT AsycudaDocumentItem.Item_Id
FROM   AsycudaDocumentItem INNER JOIN
             AsycudaDocument ON AsycudaDocumentItem.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id
WHERE --(AsycudaDocumentItem.InvalidHSCode = 1) AND
	 ((AsycudaDocumentItem.ItemQuantity <> AsycudaDocumentItem.PiQuantity) or (AsycudaDocumentItem.ItemQuantity = 0)) 
	AND (AsycudaDocument.DocumentType = 'IM7')
	and (AsycudaDocument.Cnumber = '388')
	and (isnull(AsycudaDocument.cancelled,0) <> 1 ))
	--and (isnull(AsycudaDocument.DoNotAllocate,0) = 0 ))

update xcuda_Item
set WarehouseError = 'Weight Issue'
where Item_id in 
(SELECT xcuda_Item.Item_Id
FROM   xcuda_Item INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id INNER JOIN
             xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
where number = '15622'
and LineNumber = 7)

select * from asycudadocumentitem where warehouseerror is not null and itemquantity <> piquantity

/*and (isnull(AsycudaDocument.DoNotAllocate,0) = 0 )*/
SELECT AsycudaDocumentItem.Item_Id, AsycudaDocumentItem.AsycudaDocumentId, AsycudaDocumentItem.EntryDataDetailsId, AsycudaDocumentItem.LineNumber, AsycudaDocumentItem.IsAssessed, 
             AsycudaDocumentItem.DoNotAllocate, AsycudaDocumentItem.DoNotEX, AsycudaDocumentItem.AttributeOnlyAllocation, AsycudaDocumentItem.Description_of_goods, 
             AsycudaDocumentItem.Commercial_Description, AsycudaDocumentItem.Gross_weight_itm, AsycudaDocumentItem.Net_weight_itm, AsycudaDocumentItem.Item_price, AsycudaDocumentItem.ItemQuantity, 
             AsycudaDocumentItem.PiQuantity, AsycudaDocumentItem.Suppplementary_unit_code, AsycudaDocumentItem.ItemNumber, AsycudaDocumentItem.TariffCode, 
             AsycudaDocumentItem.TariffCodeLicenseRequired, AsycudaDocumentItem.TariffCategoryLicenseRequired, AsycudaDocumentItem.TariffCodeDescription, AsycudaDocumentItem.DutyLiability, 
             AsycudaDocumentItem.Total_CIF_itm, AsycudaDocumentItem.Freight, AsycudaDocumentItem.Statistical_value, AsycudaDocumentItem.DPQtyAllocated, AsycudaDocumentItem.DFQtyAllocated, 
             AsycudaDocumentItem.ImportComplete, AsycudaDocumentItem.CNumber, AsycudaDocumentItem.RegistrationDate, AsycudaDocumentItem.Number_of_packages, 
             AsycudaDocumentItem.Country_of_origin_code, AsycudaDocumentItem.PiWeight, AsycudaDocumentItem.Currency_rate, AsycudaDocumentItem.Currency_code, AsycudaDocumentItem.InvalidHSCode, 
             AsycudaDocumentItem.WarehouseError
FROM   AsycudaDocumentItem INNER JOIN
             AsycudaDocument ON AsycudaDocumentItem.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id INNER JOIN
             ApplicationSettings ON AsycudaDocumentItem.RegistrationDate <= ApplicationSettings.OpeningStockDate
WHERE (AsycudaDocumentItem.ItemQuantity <> AsycudaDocumentItem.PiQuantity OR
             AsycudaDocumentItem.ItemQuantity = 0) AND (AsycudaDocument.DocumentType = 'IM7') AND (AsycudaDocument.CNumber IS NOT NULL) AND (ISNULL(AsycudaDocument.Cancelled, 0) <> 1)
ORDER BY AsycudaDocumentItem.RegistrationDate


update xcuda_Item
set WarehouseError = 'Zero Quantity'
where Item_id in 
(select tarification_id from xcuda_Supplementary_unit where Suppplementary_unit_quantity = 0 and IsFirstRow = 1)

select * FROM   AsycudaDocumentItem INNER JOIN
             AsycudaDocument ON AsycudaDocumentItem.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id
where Item_id in 
(select tarification_id from xcuda_Supplementary_unit where Suppplementary_unit_quantity = 0 and IsFirstRow = 1)
 AND (AsycudaDocument.DocumentType = 'IM7')
	and (AsycudaDocument.Cnumber is not null)
	and (isnull(AsycudaDocument.cancelled,0) <> 1 )


SELECT AsycudaDocumentItem.CNumber, AsycudaDocumentItem.RegistrationDate, AsycudaDocumentItem.LineNumber, AsycudaDocumentItem.ItemNumber, AsycudaDocumentItem.Description_of_goods, 
             AsycudaDocumentItem.Commercial_Description, AsycudaDocumentItem.Item_price, AsycudaDocumentItem.ItemQuantity, AsycudaDocumentItem.PiQuantity, AsycudaDocumentItem.TariffCode, 
             AsycudaDocumentItem.PiWeight, AsycudaDocumentItem.Currency_rate, AsycudaDocumentItem.Currency_code, AsycudaDocumentItem.InvalidHSCode, AsycudaDocumentItem.WarehouseError
FROM   AsycudaDocumentItem INNER JOIN
             AsycudaDocument ON AsycudaDocumentItem.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id INNER JOIN
             ApplicationSettings ON AsycudaDocumentItem.RegistrationDate <= ApplicationSettings.OpeningStockDate
WHERE (AsycudaDocumentItem.ItemQuantity > AsycudaDocumentItem.PiQuantity OR
             AsycudaDocumentItem.ItemQuantity = 0) AND (AsycudaDocument.DocumentType = 'IM7') AND (AsycudaDocument.CNumber IS NOT NULL) AND (ISNULL(AsycudaDocument.Cancelled, 0) <> 1)
ORDER BY AsycudaDocumentItem.RegistrationDate