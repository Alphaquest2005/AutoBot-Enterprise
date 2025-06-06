USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-LICToAssess]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-LICToAssess]
AS

SELECT [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, [TODO-PODocSet].QtyLicensesRequired
FROM    [TODO-PODocSet] INNER JOIN
                 ApplicationSettings ON [TODO-PODocSet].ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId
				  INNER JOIN        [TODO-LicenseRequired] ON [TODO-PODocSet].ApplicationSettingsId = [TODO-LicenseRequired].ApplicationSettingsId AND 
                 [TODO-PODocSet].AsycudaDocumentSetId = [TODO-LicenseRequired].AsycudaDocumentSetId
where ([TODO-PODocSet].LicenseLines > 0) AND ([TODO-PODocSet].Country_of_origin_code IS NOT NULL)
--GROUP BY [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
--                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
--                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, [TODO-PODocSet].QtyLicensesRequired
GO
