USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetToAssessErrors]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO












CREATE VIEW [dbo].[TODO-PODocSetToAssessErrors]
AS
SELECT [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, ROUND(AsycudaDocumentSetEntryDataCIF.InvoiceTotal, 2) AS SpecifiedCIF, 
                 ROUND([TODO-PODocSet].TotalCIF + AsycudaDocumentSetTotals.InternalFreight + AsycudaDocumentSetTotals.Insurance + AsycudaDocumentSetTotals.OtherCost - AsycudaDocumentSetTotals.Deductions, 2) 
                 AS GeneratedCIF, CASE WHEN [TODO-PODocSet].InvoiceTotal > 185 AND [TODO-PODocSet].Currency_Code <> 'XCD' OR
                 ([TODO-PODocSet].InvoiceTotal > 500 AND [TODO-PODocSet].Currency_Code = 'XCD') THEN 1 ELSE 0 END AS NeedC71, CASE WHEN licenselines > 0 THEN 1 ELSE 0 END AS NeedLicense, 
                 [TODO-PODocSetToExport-C71].HasC71, [TODO-PODocSetToExport-License].HasLicense, [TODO-PODocSet].TotalFreight, [TODO-PODocSet].ClassifiedLines, [TODO-PODocSet].TotalLines, 
                 [TODO-PODocSet].TotalWeight, AsycudaDocumentSetFreight.TotalFreight AS SpecifiedFreight, [TODO-PODocSet].TotalFreight AS GeneratedFreight, ExpectedAttachments, COUNT(DISTINCT AsycudaDocumentAttachments.Id) as GeneratedAttachments, 
                 AsycudaDocumentSetLines.Lines AS GeneratedLines, [TODO-PODocSet].EntryPackages, AsycudaDocumentSetPackages.Packages AS GeneratedPackages, 
                 CASE WHEN ClassifiedLines <> TotalLines THEN 'Missing Classifications' WHEN [TODO-PODocSet].DocumentsCount = 0 THEN 'No Documents Generated'
				 WHEN [TODO-PODocSet].EntryPackages <> AsycudaDocumentSetPackages.Packages THEN 'Generated Packages <> EntryPackages' 
				 WHEN [TODO-PODocSet].EntryPackages <> [TODO-PODocSet].TotalPackages THEN 'EntryPackages <> DocSet Packages'
				 WHEN [TODO-PODocSet].TotalWeight = 0 THEN 'Docset weight not set' 
				 WHEN (abs(round([AsycudaDocumentSetEntryDataCIF].InvoiceTotal, 2) - round([TODO-PODocSet].TotalCIF + AsycudaDocumentSetTotals.InternalFreight + AsycudaDocumentSetTotals.Insurance + AsycudaDocumentSetTotals.OtherCost - AsycudaDocumentSetTotals.Deductions, 2)) > 0.01) 
                 THEN 'Specified CIF ' + CAST(([AsycudaDocumentSetEntryDataCIF].InvoiceTotal) AS nvarchar(50)) 
                 + ' <> Generated CIF' + CAST(([TODO-PODocSet].TotalCIF + AsycudaDocumentSetTotals.InternalFreight + AsycudaDocumentSetTotals.Insurance + AsycudaDocumentSetTotals.OtherCost - AsycudaDocumentSetTotals.Deductions)
                  AS nvarchar(50)) WHEN (round([AsycudaDocumentSetFreight].TotalFreight, 2) <> round([TODO-PODocSet].TotalFreight, 2)) 
                 THEN 'DocSet Freight <> Generated Freight' WHEN (CASE WHEN [TODO-PODocSet].InvoiceTotal > 185 AND [TODO-PODocSet].Currency_Code <> 'XCD' OR
                 ([TODO-PODocSet].InvoiceTotal > 500 AND [TODO-PODocSet].Currency_Code = 'XCD') THEN 1 ELSE 0 END) <> ISNULL([TODO-PODocSetToExport-C71].HasC71, 0) 
                 THEN 'C71 Issue' 
				 WHEN [TODO-PODocSet].TotalFreight is null THEN 'Freight is Missing'
				 --WHEN (COUNT(DISTINCT AsycudaDocumentAttachments.Id) < [TODO-PODocSet].DocumentsCount * 3) THEN 'Required Attachements not met'
				 when ([TODO-PODocSetAttachementErrors].Status is not null and [TODO-PODocSetAttachementErrors].Status <> 'Missing Required Attachment: FREIGHT INVOICE' ) or ([TODO-PODocSetAttachementErrors].Status = 'Missing Required Attachment: FREIGHT INVOICE' and [TODO-PODocSet].TotalFreight = 0 ) then [TODO-PODocSetAttachementErrors].Status
				 when (([TODO-PODocSet].Manifest_Number IS NULL) or ([TODO-PODocSet].Manifest_Number = '')) then 'Manifest number is missing'
				 when (isnull([TODO-PODocSet].ExpectedEntries, 0) <>  [TODO-PODocSet].DocumentsCount) then 'Expected Entries different from Document Count'
				 when (isnull([TODO-PODocSet].TotalInvoices, 0) <>  AsycudaDocumentSetEntryDataCIF.ImportedInvoices) then 'Expected No. of Invoices different from DocSet Invoice Count'
				 WHEN (CASE WHEN licenselines > 0 THEN 1 ELSE 0 END) 
                 <> ISNULL(haslicense, 0) THEN 'License Issue' WHEN [TODO-PODocSet].TotalLines <> lines THEN 'All lines was not generated' ELSE 'No Errors' END AS Status
FROM    [TODO-PODocSetToExport-C71] RIGHT OUTER JOIN
                 [TODO-PODocSet] ON [TODO-PODocSetToExport-C71].AsycudaDocumentSetId = [TODO-PODocSet].AsycudaDocumentSetId LEFT OUTER JOIN
                 [TODO-PODocSetToExport-License] ON [TODO-PODocSet].AsycudaDocumentSetId = [TODO-PODocSetToExport-License].AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetLines ON [TODO-PODocSet].AsycudaDocumentSetId = AsycudaDocumentSetLines.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetTotals ON [TODO-PODocSet].AsycudaDocumentSetId = AsycudaDocumentSetTotals.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetPackages ON [TODO-PODocSet].AsycudaDocumentSetId = AsycudaDocumentSetPackages.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentAttachments ON [TODO-PODocSet].AsycudaDocumentSetId = AsycudaDocumentAttachments.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetEntryDataCIF ON [TODO-PODocSet].AsycudaDocumentSetId = AsycudaDocumentSetEntryDataCIF.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetFreight ON [TODO-PODocSet].AsycudaDocumentSetId = AsycudaDocumentSetFreight.AsycudaDocumentSetId left outer join
				 [TODO-PODocSetAttachementErrors] on [TODO-PODocSet].AsycudaDocumentSetId = [TODO-PODocSetAttachementErrors].AsycudaDocumentSetId left outer join				 
				 [TODO-PODocSetAttachementExpected] on [TODO-PODocSet].AsycudaDocumentSetId = [TODO-PODocSetAttachementExpected].AsycudaDocumentSetId

GROUP BY [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, AsycudaDocumentSetEntryDataCIF.InvoiceTotal, 
                  
                 [TODO-PODocSetToExport-C71].HasC71, [TODO-PODocSetToExport-License].HasLicense, [TODO-PODocSet].TotalFreight, [TODO-PODocSet].ClassifiedLines, [TODO-PODocSet].TotalLines, 
                 [TODO-PODocSet].TotalWeight, AsycudaDocumentSetFreight.TotalFreight, [TODO-PODocSet].DocumentsCount , AsycudaDocumentSetLines.Lines, [TODO-PODocSet].EntryPackages, 
                 AsycudaDocumentSetPackages.Packages, AsycudaDocumentSetEntryDataCIF.InvoiceTotal, AsycudaDocumentSetTotals.InternalFreight, AsycudaDocumentSetTotals.Insurance, 
                 AsycudaDocumentSetTotals.OtherCost, AsycudaDocumentSetTotals.Deductions,[TODO-PODocSet].ExpectedEntries, [TODO-PODocSet].TotalPackages, [TODO-PODocSetAttachementErrors].Status, ExpectedAttachments, AsycudaDocumentSetEntryDataCIF.ImportedInvoices
HAVING ( [TODO-PODocSet].TotalFreight is null /*and not ([TODO-PODocSetAttachementErrors].Status = 'Missing Required Attachment: FREIGHT INVOICE' and [TODO-PODocSet].TotalFreight = 0 )*/) OR
                 ([TODO-PODocSet].ClassifiedLines <> [TODO-PODocSet].TotalLines) OR
                 ([TODO-PODocSet].TotalLines <> AsycudaDocumentSetLines.Lines) OR
                 (CASE WHEN [TODO-PODocSet].InvoiceTotal > 185 AND [TODO-PODocSet].Currency_Code <> 'XCD' OR
                 ([TODO-PODocSet].InvoiceTotal > 500 AND [TODO-PODocSet].Currency_Code = 'XCD') THEN 1 ELSE 0 END <> ISNULL([TODO-PODocSetToExport-C71].HasC71, 0)) OR
                 (CASE WHEN licenselines > 0 THEN 1 ELSE 0 END <> ISNULL([TODO-PODocSetToExport-License].HasLicense, 0)) OR
                 ([TODO-PODocSet].EntryPackages <> AsycudaDocumentSetPackages.Packages) OR
				 ([TODO-PODocSet].EntryPackages <> [TODO-PODocSet].TotalPackages) OR
                 ([TODO-PODocSet].TotalWeight = 0) OR
                 ([TODO-PODocSet].DocumentsCount = 0) OR
                 (COUNT(DISTINCT AsycudaDocumentAttachments.Id) < [TODO-PODocSetAttachementExpected].ExpectedAttachments) OR
				
				(([TODO-PODocSetAttachementErrors].Status is not null and [TODO-PODocSetAttachementErrors].Status <> 'Missing Required Attachment: FREIGHT INVOICE' ) 
						or ([TODO-PODocSetAttachementErrors].Status = 'Missing Required Attachment: FREIGHT INVOICE' and [TODO-PODocSet].TotalFreight is null )) or
                 (Abs(ROUND(AsycudaDocumentSetEntryDataCIF.InvoiceTotal, 2) 
                 - ROUND([TODO-PODocSet].TotalCIF + AsycudaDocumentSetTotals.InternalFreight + AsycudaDocumentSetTotals.Insurance + AsycudaDocumentSetTotals.OtherCost - AsycudaDocumentSetTotals.Deductions, 2)) > 0.01) OR
                 (ROUND(AsycudaDocumentSetFreight.TotalFreight, 2) <> ROUND(isnull([TODO-PODocSet].TotalFreight,0), 2)) or
				  (([TODO-PODocSet].Manifest_Number IS NULL) or ([TODO-PODocSet].Manifest_Number = '')) or
				  (isnull([TODO-PODocSet].ExpectedEntries, 0) <>  [TODO-PODocSet].DocumentsCount) or
				  (isnull([TODO-PODocSet].TotalInvoices, 0) <>  AsycudaDocumentSetEntryDataCIF.ImportedInvoices)
GO
