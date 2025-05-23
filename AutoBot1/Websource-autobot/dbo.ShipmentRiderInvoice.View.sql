USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentRiderInvoice]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE VIEW [dbo].[ShipmentRiderInvoice]
AS

select cast(row_number() OVER ( ORDER BY t.RiderLineID) AS int) AS id,t.* from 

(

SELECT DISTINCT 
                  MAX(ShipmentRiderDetails.Id) AS RiderLineID, ShipmentRider.Id AS RiderID, isnull(ShipmentInvoice.Id,0) AS InvoiceId, ShipmentInvoiceRiderManualMatches.WarehouseCode, 
                 MIN(ShipmentRiderDetails.InvoiceNumber) AS InvoiceNumber, ShipmentInvoiceRiderManualMatches.InvoiceNo,  sum(ShipmentInvoiceRiderManualMatches.Packages) as Packages, 1 as rowNumber, 1 as RowNumber2
FROM    ShipmentInvoiceRiderManualMatches INNER JOIN
                 ShipmentRiderDetails ON trim(ShipmentInvoiceRiderManualMatches.WarehouseCode) = trim(ShipmentRiderDetails.WarehouseCode) and trim(ShipmentInvoiceRiderManualMatches.RiderInvoiceNumber) = trim(ShipmentRiderDetails.InvoiceNumber) INNER JOIN
                 ShipmentInvoice ON trim(ShipmentInvoiceRiderManualMatches.InvoiceNo) = trim(ShipmentInvoice.InvoiceNo) INNER JOIN
                 ShipmentRider ON ShipmentRider.Id = ShipmentRiderDetails.RiderId AND ShipmentInvoice.ApplicationSettingsId = ShipmentRider.ApplicationSettingsId
--where ShipmentInvoiceRiderManualMatches.Packages <> 0
GROUP BY  ShipmentRider.Id, ShipmentInvoice.Id, ShipmentInvoiceRiderManualMatches.WarehouseCode, ShipmentInvoiceRiderManualMatches.InvoiceNo
--having sum(ShipmentInvoiceRiderManualMatches.Packages) > 0	took out because of combined entries	


union all
select matches.* from 
(SELECT ShipmentRiderDetails.Id AS RiderLineID, ShipmentRider.Id AS RiderID, ShipmentInvoice.Id AS InvoiceId, ShipmentRiderDetails.WarehouseCode, ShipmentRiderDetails.InvoiceNumber, ShipmentInvoice.InvoiceNo, 
                 ShipmentRiderDetails.Pieces,

				 ROW_NUMBER() over(partition by shipmentriderdetails.id order by difference(shipmentriderdetails.invoicenumber, shipmentinvoice.invoiceno)  desc ) as RowNumber
				,ROW_NUMBER() over(partition by ShipmentRider.Id, shipmentinvoice.invoiceno order by case when ISNUMERIC(shipmentriderdetails.invoicenumber) = 1 and ISNUMERIC(shipmentinvoice.invoiceno) = 1 then abs(cast(shipmentriderdetails.invoicenumber as decimal(18,4))-cast(shipmentinvoice.invoiceno as decimal(18,4))) else  difference(shipmentriderdetails.invoicenumber, shipmentinvoice.invoiceno) end asc ) as RowNumber2


FROM    ShipmentRider INNER JOIN
                 ShipmentRiderDetails ON ShipmentRider.Id = ShipmentRiderDetails.RiderId INNER JOIN
                 Emails AS RiderEmail ON ShipmentRider.EmailId = RiderEmail.EmailId LEFT OUTER JOIN
                 ShipmentInvoiceExtraInfo RIGHT OUTER JOIN
                 ShipmentInvoice INNER JOIN
                 Emails AS InvoiceEmail 
					ON ShipmentInvoice.EmailId = InvoiceEmail.EmailId 
					ON ShipmentInvoiceExtraInfo.InvoiceId = ShipmentInvoice.Id 
					
				ON 
				(
					ShipmentRiderDetails.InvoiceNumber = ShipmentInvoice.InvoiceNo 
				/*
				trim(ShipmentRiderDetails.TrackingNumber) + trim(ShipmentRiderDetails.InvoiceNumber) LIKE N'%' + trim(ShipmentInvoice.InvoiceNo) + N'%' 
				OR trim(ShipmentRiderDetails.InvoiceNumber) LIKE N'%' + trim(ShipmentInvoice.InvoiceNo) + N'%' 
				OR ( ShipmentInvoice.InvoiceNo is null and (ShipmentInvoice.InvoiceNo is not null and (SUBSTRING(ShipmentRiderDetails.InvoiceNumber, PATINDEX('%[^0]%', ShipmentRiderDetails.InvoiceNumber + '.'), LEN(ShipmentRiderDetails.InvoiceNumber)) LIKE N'%' + SUBSTRING(ShipmentInvoice.InvoiceNo, PATINDEX('%[^0]%', ShipmentInvoice.InvoiceNo + '.'), LEN(ShipmentInvoice.InvoiceNo)))))
				OR trim(ShipmentRiderDetails.TrackingNumber) + trim(ShipmentRiderDetails.InvoiceNumber) LIKE N'%' + trim(ShipmentInvoiceExtraInfo.Value) + N'%'
				OR trim(ShipmentInvoiceExtraInfo.Value) LIKE N'%' + CASE WHEN trim(ShipmentRiderDetails.InvoiceNumber) = '' THEN 'unknown' ELSE trim(ShipmentRiderDetails.InvoiceNumber) END + N'%'
				OR trim(ShipmentInvoiceExtraInfo.Value) LIKE N'%' + CASE WHEN trim(ShipmentRiderDetails.TrackingNumber) = '' THEN 'unknown' ELSE trim(ShipmentRiderDetails.TrackingNumber) END + N'%'*/
				) 
				AND  
				 (ShipmentRider.ApplicationSettingsId = ShipmentInvoice.ApplicationSettingsId)
WHERE (
--(ShipmentInvoiceExtraInfo.Info <> 'FileLineNumber' OR   ShipmentInvoiceExtraInfo.Info IS NULL) 
--			AND (trim(ShipmentInvoiceExtraInfo.Value) <> '') AND
			 (ABS(DATEDIFF(day, RiderEmail.EmailDate, InvoiceEmail.EmailDate)) < 15) 
			OR  (ShipmentInvoiceExtraInfo.Info <> 'FileLineNumber' OR   ShipmentInvoiceExtraInfo.Info IS NULL)
				 AND (ShipmentInvoiceExtraInfo.Value IS NULL)
				 ) --and ShipmentRider.Id = 2552
GROUP BY ShipmentRider.Id, ShipmentRiderDetails.Id, ShipmentInvoice.Id, ShipmentRiderDetails.WarehouseCode, ShipmentInvoice.InvoiceNo, ShipmentRiderDetails.Pieces, ShipmentRiderDetails.InvoiceNumber
) as matches
left outer join ShipmentInvoiceRiderManualMatches on trim(matches.WarehouseCode) = trim(ShipmentInvoiceRiderManualMatches.WarehouseCode) and trim(matches.InvoiceNumber) = trim(ShipmentInvoiceRiderManualMatches.InvoiceNo)

where ShipmentInvoiceRiderManualMatches.id is null and matches.RowNumber = 1  ----- putting this back because 57 harken derm matching twice /*and matches.RowNumber2 = 1 */

) as t
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentRiderInvoice'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentRiderInvoice'
GO
