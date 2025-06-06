USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EntryDataDetailsAttachments-Required]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EntryDataDetailsAttachments-Required]
AS

SELECT AttachmentCodes.Code, AttachmentCodes.Description, EntryDataDetailsEx.EntryDataDetailsId, EntryDataDetailsEx.EntryData_Id, EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.ItemNumber, 
                 EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.Quantity, EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.TariffCode, EntryDataDetailsEx.ApplicationSettingsId, 
                 EntryDataDetailsEx.AsycudaDocumentSetId, AsycudaDocumentSet.Country_of_origin_code
FROM    EntryDataDetailsEx INNER JOIN
                 TariffCodeRequiredAttachments INNER JOIN
                 AttachmentCodes ON TariffCodeRequiredAttachments.AttachmentCodeId = AttachmentCodes.Id ON EntryDataDetailsEx.TariffCode = TariffCodeRequiredAttachments.TariffCode INNER JOIN
                 EntryData_PurchaseOrders ON EntryDataDetailsEx.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id INNER JOIN
                 AsycudaDocumentSet ON EntryDataDetailsEx.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId
GO
