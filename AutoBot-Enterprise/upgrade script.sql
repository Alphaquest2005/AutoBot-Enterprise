--upgrade script


update Customs_Procedure
	set IsObsolete = 0 where IsObsolete is null

update Customs_Procedure
	set Stock = 0 where Stock is null

update Customs_Procedure
	set Discrepancy = 0 where Discrepancy is null

update Customs_Procedure
	set Adjustment = 0 where Adjustment is null

update Customs_Procedure
	set Sales = 0 where Sales is null

update Customs_Procedure
	set SubmitToCustoms = 0 where SubmitToCustoms is null
	

	update Customs_Procedure
	set IsDefault = 0 where IsDefault is null
	

	
	update Customs_Procedure
	set IsPaid = 0 where IsPaid is null

	

		
	update xcuda_ASYCUDA_ExtendedProperties
	set Cancelled = 0 where Cancelled is null

		update xcuda_ASYCUDA_ExtendedProperties
	set DoNotAllocate = 0 where DoNotAllocate is null


	
		update xcuda_ASYCUDA_ExtendedProperties
	set IsManuallyAssessed = 0 where IsManuallyAssessed is null