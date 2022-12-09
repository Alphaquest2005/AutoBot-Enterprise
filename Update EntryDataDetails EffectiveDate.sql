
UPDATE       EntryDataDetails
SET                EffectiveDate = EntryData.EntryDataDate
FROM            EntryData_Adjustments INNER JOIN
                         EntryData ON EntryData_Adjustments.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                         EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id
WHERE        (EntryDataDetails.EffectiveDate IS NULL)