USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[OCR-InvoiceRegExView]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[OCR-InvoiceRegExView]
AS
SELECT dbo.[OCR-Invoices].Name, dbo.[OCR-Invoices].ApplicationSettingsId, dbo.[OCR-Invoices].IsActive, dbo.[OCR-RegularExpressions].Id, dbo.[OCR-RegularExpressions].RegEx, 
                 [OCR-RegularExpressions_1].Id AS ReplaceRegExId, [OCR-RegularExpressions_1].RegEx AS ReplaceRegEx
FROM    dbo.[OCR-InvoiceRegEx] INNER JOIN
                 dbo.[OCR-RegularExpressions] ON dbo.[OCR-InvoiceRegEx].RegExId = dbo.[OCR-RegularExpressions].Id INNER JOIN
                 dbo.[OCR-Invoices] ON dbo.[OCR-InvoiceRegEx].InvoiceId = dbo.[OCR-Invoices].Id INNER JOIN
                 dbo.[OCR-RegularExpressions] AS [OCR-RegularExpressions_1] ON dbo.[OCR-InvoiceRegEx].ReplacementRegExId = [OCR-RegularExpressions_1].Id
GO
