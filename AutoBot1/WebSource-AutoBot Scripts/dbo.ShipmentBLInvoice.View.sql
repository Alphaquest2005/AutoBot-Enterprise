USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentBLInvoice]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[ShipmentBLInvoice]
AS

select cast(row_number() OVER ( ORDER BY t.BLDetailsLineID) AS int) AS id,t.* from 

(

SELECT DISTINCT 
                  MAX(ShipmentBLDetails.Id) AS BLDetailsLineID, ShipmentBL.Id AS BLID, ShipmentBL.BLNumber, isnull(ShipmentInvoice.Id,0) AS InvoiceId, ShipmentInvoiceBlManualMatches.WarehouseCode, 
                 ShipmentInvoiceBlManualMatches.InvoiceNo,  sum(ShipmentInvoiceBlManualMatches.Packages) as Packages
FROM    ShipmentInvoiceBlManualMatches INNER JOIN
                 ShipmentBLDetails ON trim(ShipmentInvoiceBlManualMatches.WarehouseCode) = trim(ShipmentBLDetails.Marks)  INNER JOIN
                 ShipmentInvoice ON trim(ShipmentInvoiceBlManualMatches.InvoiceNo) = trim(ShipmentInvoice.InvoiceNo) INNER JOIN
                 ShipmentBL ON ShipmentBL.Id = ShipmentBLDetails.BLId AND ShipmentInvoice.ApplicationSettingsId = ShipmentBL.ApplicationSettingsId
--where ShipmentInvoiceRiderManualMatches.Packages <> 0
GROUP BY  ShipmentBL.Id, ShipmentInvoice.Id, ShipmentInvoiceBlManualMatches.WarehouseCode, ShipmentInvoiceBlManualMatches.InvoiceNo,  ShipmentBL.BLNumber
--having sum(ShipmentInvoiceRiderManualMatches.Packages) > 0	took out because of combined entries	


union all
select matches.* from 
(SELECT ShipmentBLDetails.Id AS BLDetailsLineID, ShipmentBL.Id AS BLID, ShipmentBL.BLNumber,  ShipmentInvoice.Id AS InvoiceId, ShipmentBLDetails.Marks,  ShipmentInvoice.InvoiceNo, 
                 ShipmentBLDetails.Quantity


FROM    ShipmentBL INNER JOIN
                 ShipmentBLDetails ON ShipmentBL.Id = ShipmentBLDetails.BLId INNER JOIN
                 Emails AS RiderEmail ON ShipmentBL.EmailId = RiderEmail.EmailId LEFT OUTER JOIN
                 ShipmentInvoiceExtraInfo RIGHT OUTER JOIN
                 ShipmentInvoice INNER JOIN
                 Emails AS InvoiceEmail 
					ON ShipmentInvoice.EmailId = InvoiceEmail.EmailId 
					ON ShipmentInvoiceExtraInfo.InvoiceId = ShipmentInvoice.Id 
					
				ON 
				(
			
				replace(Translate(trim(ShipmentBLDetails.Comments),'-. O0','###00'),'#','')  LIKE N'%' + replace(Translate(trim(ShipmentInvoice.InvoiceNo),'-. o0','###00'),'#','') + N'%' 
								
				--OR trim(ShipmentBLDetails.Comments) LIKE N'%' + trim(ShipmentInvoiceExtraInfo.Value) + N'%'
				
				) 
				AND  
				 (ShipmentBL.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId)
WHERE (
--(ShipmentInvoiceExtraInfo.Info <> 'FileLineNumber' OR   ShipmentInvoiceExtraInfo.Info IS NULL) 
--			AND (trim(ShipmentInvoiceExtraInfo.Value) <> '') AND
			 (ABS(DATEDIFF(day, RiderEmail.EmailDate, InvoiceEmail.EmailDate)) < 15) 
			OR  (ShipmentInvoiceExtraInfo.Info <> 'FileLineNumber' OR   ShipmentInvoiceExtraInfo.Info IS NULL)
				 AND (ShipmentInvoiceExtraInfo.Value IS NULL)
				 ) --and ShipmentRider.Id = 2552
GROUP BY ShipmentBL.Id, ShipmentBLDetails.Id, ShipmentInvoice.Id, ShipmentBLDetails.Marks, ShipmentInvoice.InvoiceNo, ShipmentBLDetails.Quantity, ShipmentBL.BLNumber
) as matches
where matches.InvoiceId is not null


) as t
GO
