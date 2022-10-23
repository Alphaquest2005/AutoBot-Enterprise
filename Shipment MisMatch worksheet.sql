
declare @poNumber nvarchar(50) =  '03394', @invNumber nvarchar(50) = '500546522'

select * from ShipmentInvoiceDetailsEx where InvoiceNo = @invNumber

select * from ShipmentPODetails where InvoiceNo = @poNumber

select * from ShipmentInvoicePOItemQueryMatches where PONumber = @poNumber

select * from ShipmentInvoicePOItemMISPODetails where InvoiceNo  = @poNumber

select * from ShipmentInvoicePOItemMISINVdetails where InvoiceNo = @invNumber

select * from ShipmentInvoicePOItemMISData where  PONumber = @poNumber or InvoiceNo = @invNumber

select * from ShipmentInvoicePOItemMISMatches where  PONumber = @poNumber or InvoiceNo = @invNumber

select * from ShipmentInvoice where InvoiceNo = @invNumber


