USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-AdjustmentsToXMLDataEntry]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-AdjustmentsToXMLDataEntry]
AS
SELECT TOP (100) PERCENT EntryDataDetailsId, EntryData_Id, Item_Id, ItemNumber, [key], DocumentType, Quantity, ImportComplete
                                       FROM     AsycudaDocumentItemEntryDataDetails AS AsycudaDocumentItemEntryDataDetails_1
									   inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentItemEntryDataDetails_1.ASYCUDA_Id
                                       WHERE  (AsycudaDocumentCustomsProcedures.Discrepancy = 1)
                                       ORDER BY EntryDataDetailsId

GO
