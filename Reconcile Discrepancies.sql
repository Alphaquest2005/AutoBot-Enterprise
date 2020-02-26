update entrydatadetails
set IsReconciled = 1
where entrydatadetailsid in (select Entrydatadetailsid from [TODO-SubmitDiscrepanciesErrorReport])


update entrydatadetails
set IsReconciled = 1
where entrydatadetailsid in (select Entrydatadetailsid from AdjustmentDetails where InvoiceDate <= '1/5/2020')