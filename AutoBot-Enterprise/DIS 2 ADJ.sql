------- Process for handling Discreapncy BackLog into Adjustments

------- Adjustment date is after last exwarehouse date



------- Select All errors except Zero Cost because this is Data issue
select * into #DiscrepancyData from [TODO-DiscrepanciesErrors] where status <> 'Zero Cost'

------ Create Datasheet for Adjustment csv file
Declare @AdjustmentDate Datetime = '12/31/2019'
select InvoiceNo as [INVOICE #], @AdjustmentDate as Date, ItemNumber as [ITEM CODE], ItemDescription as DESCRIPTION, InvoiceQty as [INV. QTY], ReceivedQty as [REC'D QTY], Cost as [COST USD],PreviousCNumber as [LOT NUMBER], PreviousInvoiceNumber as [PO #], 'DIS->ADJ: ' + status as Comments from #DiscrepancyData


------- Clean up Discrepancies
delete from Entrydatadetails where EntryDataDetailsId in (select EntryDataDetailsId from #DiscrepancyData)

select entrydata.* from entrydata left outer join EntryDataDetails on entrydata.EntryData_Id = EntryDataDetails.EntryData_Id where EntryDataDetailsId is null

delete from entrydata from entrydata left outer join EntryDataDetails on entrydata.EntryData_Id = EntryDataDetails.EntryData_Id where EntryDataDetailsId is null

-----------Import file now via email do not overwrite... due to partial invoices over different folders 