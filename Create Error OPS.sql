declare @startDate datetime = '10/1/2021', @endDate datetime = '10/31/2021', @appSettingId int = 6

select 'ERROPS-'+ FORMAT (@startDate, 'MMMyy') as [Invoice #], @startDate as [Date], status as Comments, QtyAllocated as Quantity, InvoiceNo as [Previous Invoice Number],SalesLineNumber as [Previous Invoice Line Number], invoicedate as [Effective Date], ItemNumber as [Item Code], ItemDescription as [Description], DutyFreePaid , Cost as USD, TaxAmount as Tax, TariffCode 
from AsycudaSalesAndAdjustmentAllocationsEx
where (InvoiceDate between @startDate and @endDate) and ApplicationSettingsId = @appSettingId
and (status is not null) 


select 'xERROPS-'+ FORMAT (@startDate, 'MMMyy') as [Invoice #], @startDate as [Date], xstatus as Comments, QtyAllocated as Quantity, InvoiceNo as [Previous Invoice Number],SalesLineNumber as [Previous Invoice Line Number], invoicedate as [Effective Date], ItemNumber as [Item Code], ItemDescription as [Description], DutyFreePaid , Cost as USD, TaxAmount as Tax, TariffCode 
from AsycudaSalesAndAdjustmentAllocationsEx
where (InvoiceDate between @startDate and @endDate) and ApplicationSettingsId = @appSettingId
and (status is null) and (xStatus is not null) and xReferenceNumber is null
