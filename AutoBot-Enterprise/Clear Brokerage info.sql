delete from ShipmentBL
delete from ShipmentInvoice
delete from entrydata
delete from ShipmentRider
delete from ShipmentManifest
delete from inventoryitemalias

select * from ShipmentBL
select * from ShipmentInvoice
select * from entrydata
select * from ShipmentManifest


DELETE FROM ShipmentBL WHERE (EmailId = N'1')
delete from ShipmentInvoice WHERE (EmailId = N'1')
delete from entrydata WHERE (EmailId = N'1')
delete from ShipmentManifest WHERE (EmailId = N'1')