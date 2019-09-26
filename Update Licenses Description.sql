SELECT TOP (100) PERCENT EntryDataDetailsEx.TariffCode + N'00' AS TariffCode, EntryDataDetailsEx.Quantity, EntryDataDetailsEx.Quantity * EntryDataDetailsEx.Cost AS Total, TariffCodes.LicenseDescription, 
                 AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentSet.Country_of_origin_code, 
                 [License-UnitOfMeasure].SuppUnitCode2 AS UOM, EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.ItemNumber, EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemDescription, EntryDataEx.Type, 
                 TariffCodes.Description, TariffCategory.Description AS Expr1
FROM    EntryDataDetailsEx INNER JOIN
                 AsycudaDocumentSetEx AS AsycudaDocumentSet ON EntryDataDetailsEx.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId INNER JOIN
                 TariffCodes ON EntryDataDetailsEx.TariffCode = TariffCodes.TariffCode INNER JOIN
                 [License-UnitOfMeasure] ON TariffCodes.TariffCode = [License-UnitOfMeasure].TariffCode INNER JOIN
                 EntryDataEx ON EntryDataDetailsEx.EntryDataId = EntryDataEx.InvoiceNo AND AsycudaDocumentSet.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId AND 
                 AsycudaDocumentSet.AsycudaDocumentSetId = EntryDataEx.AsycudaDocumentSetId INNER JOIN
                 TariffCategory ON TariffCodes.TariffCategoryCode = TariffCategory.TariffCategoryCode LEFT OUTER JOIN
                 AsycudaDocumentItemEntryDataDetails ON EntryDataDetailsEx.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (TariffCodes.LicenseRequired = 1) AND (AsycudaDocumentSet.ImportedInvoices = AsycudaDocumentSet.TotalInvoices) AND (AsycudaDocumentSet.ClassifiedLines = AsycudaDocumentSet.TotalLines) AND 
                 (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL)
GROUP BY EntryDataDetailsEx.TariffCode + N'00', TariffCodes.LicenseDescription, AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.ApplicationSettingsId, 
                 AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentSet.Country_of_origin_code, [License-UnitOfMeasure].SuppUnitCode2, EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.ItemNumber, 
                 EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemDescription, EntryDataEx.Type, EntryDataDetailsEx.Quantity, EntryDataDetailsEx.Quantity * EntryDataDetailsEx.Cost, TariffCodes.Description, 
                 TariffCategory.Description
HAVING (EntryDataEx.Type = N'PO')
ORDER BY AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSet.AsycudaDocumentSetId

update TariffCodes
set LicenseDescription = 'Cushions'
where TariffCode = '94049000'