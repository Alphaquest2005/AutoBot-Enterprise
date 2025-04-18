USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ToDo-POToXML-Recreate]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 

CREATE VIEW [dbo].[ToDo-POToXML-Recreate]

AS

SELECT EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL

                 THEN 0 ELSE 1 END AS bit) AS IsClassified

FROM    [TODO-PODocSet] AS AsycudaDocumentSet INNER JOIN

                 ApplicationSettings ON AsycudaDocumentSet.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId INNER JOIN

                 AsycudaDocumentSetEntryData ON AsycudaDocumentSet.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId INNER JOIN

                 InventoryItems ON ApplicationSettings.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN

                 EntryData_PurchaseOrders INNER JOIN

                 EntryDataDetails ON EntryData_PurchaseOrders.EntryData_Id = EntryDataDetails.EntryData_Id ON InventoryItems.Id = EntryDataDetails.InventoryItemId INNER JOIN

                 [ToDo-POToXML-EntryDataEx] AS EntryDataEx ON ApplicationSettings.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId AND EntryData_PurchaseOrders.EntryData_Id = EntryDataEx.EntryData_Id AND

                 AsycudaDocumentSetEntryData.EntryData_Id = EntryDataEx.EntryData_Id

WHERE (ABS(EntryDataEx.ExpectedTotal - EntryDataEx.InvoiceTotal) < 0.01)

GROUP BY EntryDataDetails.EntryDataDetailsId, ApplicationSettings.ApplicationSettingsId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, InventoryItems.TariffCode

GO
