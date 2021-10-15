SELECT EntryDataDetails.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, EntryDataDetails.Cost, PreviousItemsEx.Prev_reg_nbr as CNumber, PreviousItemsEx.Previous_item_number AS pLineNumber, 
                 PreviousItemsEx.CNumber as xCNumber,  PreviousItemsEx.Current_item_number as xLineNumber, EntryDataDetails.LineNumber, EntryDataDetails.EntryDataDetailsId
FROM    EntryData INNER JOIN
                 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 PreviousItemsEx ON EntryData.EntryDataId LIKE '%' + PreviousItemsEx.CNumber + '-' + PreviousItemsEx.Current_item_number AND EntryData.ApplicationSettingsId = PreviousItemsEx.ApplicationSettingsId INNER JOIN
                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
WHERE (PreviousItemsEx.ApplicationSettingsId = 7) AND (EntryData.ApplicationSettingsId = 7) AND (EntryDataDetails.EntryDataId LIKE N'Asycuda-C#%')
GROUP BY EntryDataDetails.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, EntryDataDetails.Cost, PreviousItemsEx.Prev_reg_nbr, 
                 PreviousItemsEx.CNumber, PreviousItemsEx.Current_item_number, EntryDataDetails.LineNumber, EntryDataDetails.EntryDataDetailsId, PreviousItemsEx.Previous_item_number

select * from PreviousItemsEx where Prev_reg_nbr = '46042'

select * from EntryDataDetails where EntryDataDetails.EntryDataId = 'Asycuda-C#2997'

-------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------

--Set the EffectiveDate and Invoice date Back to before start of Real sales

--UPDATE EntryDataDetails
--SET         EffectiveDate = '12/31/2020'
--FROM    EntryDataDetails INNER JOIN
--                 EntryData ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id INNER JOIN
--                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
--WHERE (EntryData.ApplicationSettingsId = 7)


--UPDATE EntryData
--SET         EntryDataDate = '12/31/2020'
--FROM    EntryDataDetails INNER JOIN
--                 EntryData ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id INNER JOIN
--                 EntryData_Adjustments ON EntryData.EntryData_Id = EntryData_Adjustments.EntryData_Id
--WHERE (EntryData.ApplicationSettingsId = 7)