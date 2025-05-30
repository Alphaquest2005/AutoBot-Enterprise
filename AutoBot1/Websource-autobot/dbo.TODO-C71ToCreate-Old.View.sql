USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71ToCreate-Old]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








Create VIEW [dbo].[TODO-C71ToCreate-Old]
AS

SELECT ISNULL(CAST((row_number() OVER (ORDER BY dbo.[TODO-PODocSet].AsycudaDocumentSetId)) AS int), 0) as Id,[TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].ExpectedEntries, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, ISNULL([TODO-PODocSetToExport-C71].C71Total, 
                 0) AS C71Total, [TODO-PODocSet].CurrencyRate AS Rate, C71Summary.Total AS GeneratedC71Total
FROM    C71Summary RIGHT OUTER JOIN
                 [TODO-PODocSet] LEFT OUTER JOIN
                 [TODO-PODocSetToExport-C71] ON [TODO-PODocSet].AsycudaDocumentSetId = [TODO-PODocSetToExport-C71].AsycudaDocumentSetId 
                ON  C71Summary.Reference = [TODO-PODocSet].Declarant_Reference_Number
where
(
--(([TODO-PODocSet].InvoiceTotal * [TODO-PODocSet].CurrencyRate >= 1) /*AND ([TODO-PODocSetToExport-C71].AsycudaDocumentSetId IS NULL and   ((C71Summary.Address IS not NULL) and (C71Summary.Value_declaration_form_Id IS NOT NULL) AND (C71Summary.RegisteredID IS NULL)))*/) OR
	   ([TODO-PODocSetToExport-C71].AsycudaDocumentSetId IS not NULL and ROUND([TODO-PODocSet].TotalCIF,2) <> ROUND([TODO-PODocSetToExport-C71].C71Total,2))OR
	   ([TODO-PODocSetToExport-C71].AsycudaDocumentSetId IS NULL and ROUND([TODO-PODocSet].InvoiceTotal,2) <> ROUND(C71Summary.Total,2)) OR
	    (ROUND(C71Summary.Total,2) <> ROUND(isnull([TODO-PODocSetToExport-C71].C71Total,c71summary.total),2)
	) 
and ([TODO-PODocSet].ExpectedEntries = [TODO-PODocSet].DocumentsCount or [TODO-PODocSet].DocumentsCount = 0)
and ([TODO-PODocSet].TotalInvoices > 0)
and ([TODO-PODocSet].InvoiceTotal * [TODO-PODocSet].CurrencyRate >= 1)
)
or
 (
 (C71Summary.Address IS NULL)
  OR
                ((C71Summary.Address IS not NULL) and (C71Summary.Value_declaration_form_Id IS NOT NULL) AND (C71Summary.RegisteredID IS NULL)))
GROUP BY [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, [TODO-PODocSetToExport-C71].AsycudaDocumentSetId, 
                 [TODO-PODocSetToExport-C71].C71Total, [TODO-PODocSet].CurrencyRate, [TODO-PODocSet].ExpectedEntries, C71Summary.Total
-- C71Summary.Value_declaration_form_Id,
GO
