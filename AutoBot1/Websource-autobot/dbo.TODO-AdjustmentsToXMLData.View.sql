USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AdjustmentsToXMLData]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-AdjustmentsToXMLData]
AS
SELECT t.EntryDataDetailsId, t.ApplicationSettingsId, t.AsycudaDocumentSetId, t.IsClassified, t.AdjustmentType, t.InvoiceNo, t.InvoiceQty, t.ReceivedQty, t.InvoiceDate, t.ItemNumber, t.Status, t.CNumber, 
                                  t.Declarant_Reference_Number
                 FROM    [dbo].[TODO-AdjustmentsToXMLDataAdj]  AS t FULL OUTER JOIN
                                      dbo.[TODO-AdjustmentsToXMLDataEntry] AS z ON t.EntryDataDetailsId = z.EntryDataDetailsId AND 
                                  (z.EntryDataDetailsId IS NULL OR
                                  z.DocumentType <> CASE WHEN InvoiceQty > ReceivedQty THEN 'IM4' ELSE 'OS7' END)


GO
