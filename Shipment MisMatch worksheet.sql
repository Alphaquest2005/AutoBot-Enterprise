
declare @poNumber nvarchar(50) =  '02261', @invNumber nvarchar(50) = '021331'
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


