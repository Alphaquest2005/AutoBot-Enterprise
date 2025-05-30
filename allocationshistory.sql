/****** Script for SelectTopNRows command from SSMS  ******/

declare @itemnumber varchar(50)
set @itemnumber = 're00531'


SELECT WaterNutDB.dbo.EntryDataDetails.EntryDataDetailsId, WaterNutDB.dbo.EntryDataDetails.EntryDataId, EntryData.EntryDataDate, 
                  WaterNutDB.dbo.EntryDataDetails.LineNumber, WaterNutDB.dbo.EntryDataDetails.ItemNumber, WaterNutDB.dbo.EntryDataDetails.Quantity, 
                  WaterNutDB.dbo.EntryDataDetails.Units, WaterNutDB.dbo.EntryDataDetails.ItemDescription, WaterNutDB.dbo.EntryDataDetails.Cost, 
                  WaterNutDB.dbo.EntryDataDetails.QtyAllocated, WaterNutDB.dbo.EntryDataDetails.UnitWeight
FROM     WaterNutDB.dbo.EntryDataDetails INNER JOIN
                  EntryData ON WaterNutDB.dbo.EntryDataDetails.EntryDataId = EntryData.EntryDataId
WHERE  (WaterNutDB.dbo.EntryDataDetails.ItemNumber = @itemnumber)
order by EntryDataDate


SELECT AsycudaEntries.*
FROM     AsycudaEntries
WHERE  (ItemNumber = @itemnumber)
order by coalesce(EffectiveRegistrationDate,Registrationdate)