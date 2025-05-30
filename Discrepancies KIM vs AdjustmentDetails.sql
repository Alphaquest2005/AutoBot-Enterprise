/****** Script for SelectTopNRows command from SSMS  ******/
// Todo List vs KimList

SELECT e.ITEM_CODE, s.ItemNumber, e.INV__QTY, s.InvoiceQty, e.REC_D_QTY, s.ReceivedQty, e.subject, s.Subject
FROM    (SELECT TOP (100) PERCENT ITEM_CODE, INV__QTY, REC_D_QTY, subject
                 FROM     [shipping Discrepancies April-October 2019 for budget-fromkim]
                 GROUP BY ITEM_CODE, INV__QTY, REC_D_QTY, subject
                 ORDER BY ITEM_CODE) AS e FULL OUTER JOIN
                     (SELECT TOP (100) PERCENT ItemNumber, InvoiceQty, ReceivedQty, subject
                      FROM     [AutoBot-EnterpriseDB].dbo.[TODO-DiscrepanciesToDoList]
                      GROUP BY ItemNumber, InvoiceQty, ReceivedQty, subject
                      ORDER BY ItemNumber
					  ) AS s ON e.ITEM_CODE = s.ItemNumber AND e.INV__QTY = s.InvoiceQty AND e.REC_D_QTY = s.ReceivedQty
WHERE (e.ITEM_CODE IS NULL) OR
                 (s.ItemNumber IS NULL)

//////////////// todo list vs EntryDataFiles

SELECT e.ItemNumber, s.ItemNumber, e.InvoiceQty, s.InvoiceQty, e.ReceivedQty, s.ReceivedQty, e.subject, s.Subject
FROM    (SELECT TOP (100) PERCENT EntryDataFiles.ItemNumber, EntryDataFiles.InvoiceQty, EntryDataFiles.ReceivedQty, Emails.Subject
FROM    EntryDataFiles INNER JOIN
                 Emails ON EntryDataFiles.EmailId = Emails.EmailUniqueId
GROUP BY EntryDataFiles.ItemNumber, EntryDataFiles.InvoiceQty, EntryDataFiles.ReceivedQty, Emails.Subject
ORDER BY EntryDataFiles.ItemNumber) AS e FULL OUTER JOIN
                     (SELECT TOP (100) PERCENT ItemNumber, InvoiceQty, ReceivedQty, subject
                      FROM     [AutoBot-EnterpriseDB].dbo.[TODO-DiscrepanciesToDoList]
                      GROUP BY ItemNumber, InvoiceQty, ReceivedQty, subject
                      ORDER BY ItemNumber
					  ) AS s ON e.ItemNumber = s.ItemNumber AND e.InvoiceQty = s.InvoiceQty AND e.ReceivedQty = s.ReceivedQty
WHERE (e.ItemNumber IS NULL) OR
                 (s.ItemNumber IS NULL)

////////////////////// 

SELECT e.ITEM_CODE, s.ItemNumber, e.INV__QTY, s.InvoiceQty, e.REC_D_QTY, s.ReceivedQty, e.subject, s.Subject
FROM    (SELECT TOP (100) PERCENT ITEM_CODE, INV__QTY, REC_D_QTY, subject
                 FROM     [shipping Discrepancies April-October 2019 for budget-fromkim]
                 GROUP BY ITEM_CODE, INV__QTY, REC_D_QTY, subject
                 ORDER BY ITEM_CODE) AS e FULL OUTER JOIN
                     (SELECT TOP (100) PERCENT ItemNumber, InvoiceQty, ReceivedQty, subject
                      FROM     [dbo].[TODO-AllAdjustmentsAlreadyXMLed]
                      WHERE  (AdjustmentType = 'DIS')
                      GROUP BY ItemNumber, InvoiceQty, ReceivedQty, subject
                      ORDER BY ItemNumber
					  
					  union 
					  SELECT TOP (100) PERCENT ItemNumber, InvoiceQty, ReceivedQty, subject
                      FROM     AdjustmentDetails
                      WHERE  (AsycudaDocumentSetId <> 8) AND (Type = 'DIS')
                      GROUP BY ItemNumber, InvoiceQty, ReceivedQty, subject
                      ORDER BY ItemNumber	  
					  
					  
					  
					  ) AS s ON e.ITEM_CODE = s.ItemNumber AND e.INV__QTY = s.InvoiceQty AND e.REC_D_QTY = s.ReceivedQty
WHERE (e.ITEM_CODE IS NULL) OR
                 (s.ItemNumber IS NULL)


/////////////////////// compare kim item with adjustment details

SELECT e.ITEM_CODE, s.ItemNumber, e.INV__QTY, s.InvoiceQty, e.REC_D_QTY, s.ReceivedQty, e.subject, s.Subject
FROM    (SELECT TOP (100) PERCENT ITEM_CODE, INV__QTY, REC_D_QTY, subject
                 FROM     [shipping Discrepancies April-October 2019 for budget-fromkim]
                 GROUP BY ITEM_CODE, INV__QTY, REC_D_QTY, subject
                 ORDER BY ITEM_CODE) AS e FULL OUTER JOIN
                     (SELECT TOP (100) PERCENT ItemNumber, InvoiceQty, ReceivedQty, subject
                      FROM     AdjustmentDetails
                      WHERE  (AsycudaDocumentSetId <> 8) AND (Type = 'DIS')
                      GROUP BY ItemNumber, InvoiceQty, ReceivedQty, subject
                      ORDER BY ItemNumber) AS s ON e.ITEM_CODE = s.ItemNumber AND e.INV__QTY = s.InvoiceQty AND e.REC_D_QTY = s.ReceivedQty
WHERE (e.ITEM_CODE IS NULL) OR
                 (s.ItemNumber IS NULL)

select * from AdjustmentDetails where ItemNumber = 'JAB/37072-0092'
SELECT EntryDataDetailsEx.EntryDataDetailsId, EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemNumber, EntryDataDetailsEx.Quantity, EntryDataDetailsEx.Units, 
                 EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.Cost, EntryDataDetailsEx.QtyAllocated, EntryDataDetailsEx.UnitWeight, EntryDataDetailsEx.DoNotAllocate, EntryDataDetailsEx.TariffCode, 
                 EntryDataDetailsEx.CNumber, EntryDataDetailsEx.CLineNumber, EntryDataDetailsEx.Downloaded, EntryDataDetailsEx.DutyFreePaid, EntryDataDetailsEx.Total, EntryDataDetailsEx.AsycudaDocumentSetId, 
                 EntryDataDetailsEx.InvoiceQty, EntryDataDetailsEx.ReceivedQty, EntryDataDetailsEx.Status, EntryDataDetailsEx.PreviousInvoiceNumber, EntryDataDetailsEx.PreviousCNumber, EntryDataDetailsEx.Comment, 
                 EntryDataDetailsEx.EffectiveDate, EntryDataDetailsEx.IsReconciled, EntryDataDetailsEx.ApplicationSettingsId, EntryDataDetailsEx.LastCost, EntryDataDetailsEx.TaxAmount, EntryDataDetailsEx.EmailId, 
                 EntryDataDetailsEx.FileTypeId, EntryDataDetailsEx.Name, EntryDataEx.Type
FROM    EntryDataDetailsEx INNER JOIN
                 EntryDataEx ON EntryDataDetailsEx.EntryDataId = EntryDataEx.InvoiceNo
WHERE (EntryDataDetailsEx.ItemNumber = 'BLS/5138') and type = 'DIS'

select * from entrydataFiles


SELECT e.SourceFile AS FSourceFile, s.SourceFile AS DSourceFile, e.LineNumber AS FLineNumber, s.LineNumber AS DLineNumber, e.ItemNumber AS FItemNumber, s.ItemNumber AS dItemNumber, 
                 e.EntryDataId AS FEntryDataId, s.EntryDataId AS DEntryDataId
FROM    (SELECT TOP (100) PERCENT EntryDataId, ItemNumber, LineNumber, SourceFile
                 FROM     EntryDataFiles
                 GROUP BY EntryDataId, ItemNumber, LineNumber, SourceFile
                 ORDER BY SourceFile, LineNumber) AS e LEFT OUTER JOIN
                     (SELECT TOP (100) PERCENT AdjustmentDetails.EntryDataId, AdjustmentDetails.ItemNumber, AdjustmentDetails.LineNumber, EntryData.SourceFile
                      FROM     AdjustmentDetails INNER JOIN
                                       EntryData ON AdjustmentDetails.EntryDataId = EntryData.EntryDataId
                      WHERE  (AdjustmentDetails.AsycudaDocumentSetId <> 8) AND (AdjustmentDetails.Type = 'DIS')
                      GROUP BY AdjustmentDetails.EntryDataId, AdjustmentDetails.ItemNumber, AdjustmentDetails.LineNumber, EntryData.SourceFile
                      ORDER BY AdjustmentDetails.ItemNumber) AS s ON e.ItemNumber = s.ItemNumber AND e.LineNumber = s.LineNumber AND e.SourceFile = s.SourceFile AND e.EntryDataId = s.EntryDataId
WHERE (e.LineNumber IS NULL) AND (s.SourceFile IS NULL) OR
                 (s.LineNumber IS NULL)

--update EntryDataDetails 
--set LineNumber = 1
--where entrydataid = '201842' and LineNumber = 2

select * from entrydatadetails where entrydataid = '201842' and LineNumber = 1

--//////// map DiscrepanciesToDoList to discrepancies reports
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED

select * into #test from [TODO-DiscrepanciesToDoList]

SELECT f.EntryDataDetailsId, f.EntryDataId, f.LineNumber, f.ItemNumber, f.Quantity, f.Units, f.ItemDescription, f.Cost, f.QtyAllocated, f.UnitWeight,  f.TariffCode, f.CNumber, f.CLineNumber, 
                 f.AsycudaDocumentSetId, f.InvoiceQty, f.ReceivedQty, f.PreviousInvoiceNumber, f.PreviousCNumber, f.Comment, f.Status, f.EffectiveDate, f.Currency, f.ApplicationSettingsId, f.Type, f.Declarant_Reference_Number, 
                 f.InvoiceDate, f.Subject, f.EmailDate, f.DutyFreePaid, reports.ReportBy
FROM   (select * from  #test) AS f LEFT OUTER JOIN
                     (
					 --SELECT EntryDataDetailsId, 'Already Executed' AS ReportBy
      --                FROM     [TODO-DiscrepanciesAlreadyXMLed]
      --                UNION ALL
                      SELECT Entrydatadetailsid, Status AS ReportBy
                      FROM    [TODO-SubmitDiscrepanciesErrorReport]
                      --UNION ALL
                      --SELECT EntryDataDetailsId, 'Discrepancies to Submit' AS ReportBy
                      --FROM    [TODO-DiscrepanciesToSubmit]
                      UNION ALL
                      SELECT EntryDataDetailsId, 'Pre Execution' AS ReportBy
                      FROM    [TODO-DiscrepancyPreExecutionReport]
                      --UNION ALL
                      --SELECT EntryDataDetailsId, 'Overs' AS ReportBy
                      --FROM    [TODO-SubmitOversToCustoms]
					  ) AS reports ON f.EntryDataDetailsId = reports.EntryDataDetailsId
--where ReportBy is null
GROUP BY f.EntryDataDetailsId, f.EntryDataId, f.LineNumber, f.ItemNumber, f.Quantity, f.Units, f.ItemDescription, f.Cost, f.QtyAllocated, f.UnitWeight, f.TariffCode, f.CNumber, f.CLineNumber, f.AsycudaDocumentSetId, 
                 f.InvoiceQty, f.ReceivedQty, f.PreviousInvoiceNumber, f.PreviousCNumber, f.Comment, f.Status, f.EffectiveDate, f.Currency, f.ApplicationSettingsId, f.Type, f.Declarant_Reference_Number, f.InvoiceDate, f.Subject, 
                 f.EmailDate, f.DutyFreePaid, reports.ReportBy

--//////// map Kim List to discrepancies reports
SELECT f.Over_Shorts, f.INVOICE__, f.Vendor, f.Supplier_ID, f.PO__, f.ITEM_CODE, f.DESCRIPTION, f.INV__QTY, f.REC_D_QTY, f.SYSTEM, f.ON_HAND, f.COST_USD, f.LOT_NUMBER, f.Subject, reports.ReportBy
FROM    [shipping Discrepancies April-October 2019 for budget-fromkim] AS f LEFT OUTER JOIN
                     (SELECT InvoiceNo,ItemNumber,subject, status AS ReportBy
                      FROM     [TODO-SubmitDiscrepanciesErrorReport]
                      UNION ALL
                      SELECT EntryDataId,ItemNumber,subject, 'Pre Execution' AS ReportBy
                      FROM    [TODO-DiscrepancyPreExecutionReport]) AS reports ON f.INVOICE__ = reports.InvoiceNo and f.ITEM_CODE = reports.ItemNumber and f.Subject = reports.subject
GROUP BY reports.ReportBy, f.Over_Shorts, f.INVOICE__, f.Vendor, f.Supplier_ID, f.PO__, f.ITEM_CODE, f.DESCRIPTION, f.INV__QTY, f.REC_D_QTY, f.SYSTEM, f.ON_HAND, f.COST_USD, f.LOT_NUMBER, f.Subject

----------------------------

select reports.ReportBy, f.*, ReportBy FROM (select * from [AutoBot-EnterpriseDB].[dbo].[TODO-DiscrepanciesToDoList] where ReceivedQty - InvoiceQty > 0 ) as f
left outer join
(select Entrydatadetailsid, 'Overs' as ReportBy FROM [dbo].[TODO-SubmitOversToCustoms]) as reports
on f.EntryDataDetailsId = reports.EntryDataDetailsId
order by f.ItemNumber

-------delete duplicate entrydatadetails
select *
from (
  select *, rn=row_number() over (partition by entrydataid,linenumber, ItemNumber order by EntryDataDetailsId)
  from entrydatadetails 
) x
where rn > 1;

--delete x
--from (
--  select *, rn=row_number() over (partition by entrydataid,linenumber, ItemNumber order by EntryDataDetailsId)
--  from entrydatadetails 
--) x
--where rn > 1;

select * from [shipping Discrepancies April-October 2019 for budget-fromkim] where ITEM_CODE like 'CLC/124445'

--delete from [shipping Discrepancies April-October 2019 for budget-fromkim] where INVOICE__ in ('1404','1023','1003','1001')

select * from AdjustmentShorts where ItemNumber = 'AOR/760500'
select * from AdjustmentShortAllocations where  ItemNumber = 'SPY/STS19432'
select * from Adjustmentovers where ItemNumber = 'INT/YBA063A/16'


select * from AsycudaDocumentItem where itemnumber = 'CRS/USX040000'

select * from [TODO-AdjustmentsToXML] where AdjustmentType = 'DIS'

select * from 
              (       
                      SELECT InvoiceNo, LineNumber, ItemNumber, InvoiceQty, ReceivedQty, Subject,  status, 'Errors' as source
                      FROM    [TODO-SubmitDiscrepanciesErrorReport]
                      --UNION ALL
                      --SELECT EntryDataDetailsId, 'Discrepancies to Submit' AS ReportBy
                      --FROM    [TODO-DiscrepanciesToSubmit]
                      UNION ALL
                      SELECT EntryDataId, LineNumber, ItemNumber, InvoiceQty, ReceivedQty, Subject, 'Pre Execution' as status,'Preexecution' as source
                      FROM    [TODO-DiscrepancyPreExecutionReport]) as t
where ItemNumber = 'TOH/3GP-70020-0'