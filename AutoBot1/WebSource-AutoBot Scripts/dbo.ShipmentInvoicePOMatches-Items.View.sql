USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOMatches-Items]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ShipmentInvoicePOMatches-Items]
AS
select * from (
SELECT DISTINCT PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, ROW_NUMBER() over (partition by PODetails.InvoiceNo   order by count(case when (INVDetails.ItemNumber = PODetails.SupplierItemNumber OR
                 INVDetails.ItemNumber LIKE '%' + PODetails.ItemNumber + '%' OR
                 PODetails.ItemNumber LIKE '%' + INVDetails.ItemNumber + '%') AND ROUND(INVDetails.Cost, 2) = ROUND(PODetails.Cost, 2) AND INVDetails.Quantity = PODetails.Quantity then 1 end) desc ) AS rownum, ROW_NUMBER() over (partition by INVDetails.InvoiceId order by INVDetails.InvoiceDate desc, ABS(PODetails.ImportedLine - INVDetails.ImportedLines)) as rownum1 ,  ABS(PODetails.ImportedLine - INVDetails.ImportedLines) as differences

--Select DISTINCT PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo
FROM    (SELECT ShipmentInvoiceDetailsEx.ApplicationSettingsId, ShipmentInvoiceDetailsEx.DetailId, ShipmentInvoiceDetailsEx.InvoiceId, ShipmentInvoiceDetailsEx.InvoiceNo, ShipmentInvoiceDetailsEx.ItemNumber, 
                                  ShipmentInvoiceDetailsEx.ItemDescription, ShipmentInvoiceDetailsEx.Cost, ShipmentInvoiceDetailsEx.Quantity, ShipmentInvoiceDetailsEx.TotalCost, ShipmentInvoiceDetailsEx.InventoryItemId, 
                                  ShipmentInvoiceDetailsEx.POInventoryItemId, ShipmentInvoiceDetailsEx.ImportedLines, ShipmentInvoiceDetailsEx.invoiceDate, emaildate
                 FROM     ShipmentInvoiceDetailsEx LEFT OUTER JOIN
                                  ShipmentInvoicePOs ON ShipmentInvoiceDetailsEx.InvoiceId = ShipmentInvoicePOs.InvoiceId
                 WHERE  (ShipmentInvoicePOs.Id IS NULL)) AS INVDetails INNER JOIN
                     (SELECT ShipmentPODetails.EntryData_Id, ShipmentPODetails.InvoiceNo, ShipmentPODetails.SupplierCode, ShipmentPODetails.ApplicationSettingsId, ShipmentPODetails.ItemNumber, ShipmentPODetails.Quantity, 
                                       ShipmentPODetails.Cost, ShipmentPODetails.TotalCost, ShipmentPODetails.InventoryItemId, ShipmentPODetails.SupplierItemId, ShipmentPODetails.SupplierItemNumber, 
                                       ShipmentPODetails.EntryDataDetailsId, ShipmentPODetails.ItemDescription, ShipmentPODetails.ImportedLine, ShipmentPODetails.InvoiceDate, emaildate
                      FROM     ShipmentPODetails LEFT OUTER JOIN
                                       ShipmentInvoicePOs AS ShipmentInvoicePOs_1 ON ShipmentPODetails.EntryData_Id = ShipmentInvoicePOs_1.EntryData_Id
                      WHERE  (ShipmentInvoicePOs_1.Id IS NULL)) AS PODetails ON INVDetails.ApplicationSettingsId = PODetails.ApplicationSettingsId 
						and (abs(datediff(hh,PODetails.emaildate, INVDetails.emaildate)) < 48)
						AND  (INVDetails.ItemNumber = PODetails.SupplierItemNumber OR
                 INVDetails.ItemNumber LIKE '%' + PODetails.ItemNumber + '%' OR
                 PODetails.ItemNumber LIKE '%' + INVDetails.ItemNumber + '%') AND ROUND(INVDetails.Cost, 2) = ROUND(PODetails.Cost, 2) AND INVDetails.Quantity = PODetails.Quantity
GROUP BY PODetails.EntryData_Id, INVDetails.InvoiceId, PODetails.InvoiceNo, INVDetails.InvoiceNo, ABS(PODetails.ImportedLine - INVDetails.ImportedLines), INVDetails.invoiceDate
) as data
where rownum = 1 and rownum1 = 1

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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoicePOMatches-Items'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoicePOMatches-Items'
GO
