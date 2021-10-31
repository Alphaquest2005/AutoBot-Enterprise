----Sales ------------ to im7

--1 import sales as normal
--2 run this query and save as csv import as opening stock
--have to handle RETURNS in sales sum sales
declare @startDate datetime = '03/1/2020', @endDate Datetime = '09/30/2020', @applicationSettingsId int = 6

----CHECK IF COST IS TOTAL COST OR JUST COST
----CHECK IF ZERO COST ITEMS
SELECT format(entrydata.entrydatadate, 'MMM-yy') + '-S2OS' AS [Invoice #], DATEADD(month, DATEDIFF(month, 0, entrydata.entrydatadate), 0) AS Date, EntryDataDetails.ItemNumber AS [Item Code], EntryDataDetails.ItemDescription AS Description, SUM(EntryDataDetails.Quantity) AS Quantity, 
                                  ABS(ROUND(CASE WHEN EntryDataDetails.Cost = 0 THEN 0.01 ELSE EntryDataDetails.Cost/Quantity END, 2)) AS Cost
                 FROM     EntryData_Sales INNER JOIN
                                  EntryData ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                                  EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                                  InventoryItems ON EntryDataDetails.InventoryItemId = InventoryItems.Id
                 WHERE  (EntryData.ApplicationSettingsId = 6) AND (EntryData.EntryDataDate BETWEEN CONVERT(DATETIME, @startDate, 102) AND CONVERT(DATETIME, @endDate, 102))
                 GROUP BY EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, ABS(ROUND(CASE WHEN EntryDataDetails.Cost = 0 THEN 0.01 ELSE EntryDataDetails.Cost/quantity END, 2)), format(entrydata.entrydatadate, 'MMM-yy'), DATEADD(month, DATEDIFF(month, 0, entrydata.entrydatadate), 0)
                 HAVING (SUM(EntryDataDetails.Quantity) > 0)
				 order by DATEADD(month, DATEDIFF(month, 0, entrydata.entrydatadate), 0)


--make im7
--assess
--re import 
--allocate sales 
--check for errors - should be perfect
--set the effective assessment date
