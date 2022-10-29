
declare @poNumber nvarchar(50) =  '03315', @invNumber nvarchar(50) = '021032'
select 'ShipmentInvoiceDetailsEx'
select * from ShipmentInvoiceDetailsEx where InvoiceNo = @invNumber


select 'ShipmentPODetails'
select * from ShipmentPODetails where InvoiceNo = @poNumber

select 'ShipmentInvoicePOItemQueryMatches'
select * from ShipmentInvoicePOItemQueryMatches where PONumber = @poNumber

select 'ShipmentInvoicePOItemMISPODetails'
select * from ShipmentInvoicePOItemMISPODetails where InvoiceNo  = @poNumber

select 'ShipmentInvoicePOItemMISINVdetails'
select * from ShipmentInvoicePOItemMISINVdetails where InvoiceNo = @invNumber

select 'ShipmentInvoicePOItemMISData'
select * from ShipmentInvoicePOItemMISData where  PONumber = @poNumber or InvoiceNo = @invNumber

select 'ShipmentInvoicePOItemMISMatches'
select * from ShipmentInvoicePOItemMISMatches where  PONumber = @poNumber or InvoiceNo = @invNumber

select 'ShipmentInvoice'
select * from ShipmentInvoice where InvoiceNo = @invNumber

select '[ShipmentInvoicePOItemQueryMatches-Alias]'
select * from [ShipmentInvoicePOItemQueryMatches-Alias]  where  PONumber = @poNumber or InvoiceNo = @invNumber

select '[ShipmentInvoicePOItemQueryMatches-Match]'
select * from [ShipmentInvoicePOItemQueryMatches-Match]  where  PONumber = @poNumber or InvoiceNo = @invNumber

