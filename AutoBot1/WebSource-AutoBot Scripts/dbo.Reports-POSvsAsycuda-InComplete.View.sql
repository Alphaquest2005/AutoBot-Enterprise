USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-InComplete]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






Create VIEW [dbo].[Reports-POSvsAsycuda-InComplete]
AS

select *  
--into [#InComplete]
from dbo.[Reports-POSvsAsycuda-Results]
where --not ((isnull([Asycuda-Quantity], 0) = 0 and isnull([OPS-QuantityOnHand], 0) = 0) --- remain quantities is zero for both asycuda and ops
       -- and (isnull([Asycuda-PiQuantity], 0)) > isnull([OPS-Quantity],0)
		--)--- and pi is less than quantity in question
		--and 
		(Diff <> 0)
		and isnull([Asycuda-Quantity], 0) + isnull([OPS-Quantity],0) <> 0
GO
