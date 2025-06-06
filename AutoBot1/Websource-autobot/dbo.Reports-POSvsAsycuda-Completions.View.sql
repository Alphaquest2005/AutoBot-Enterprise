USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-Completions]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






Create VIEW [dbo].[Reports-POSvsAsycuda-Completions]
AS
select *  
--into [#Completions]
from dbo.[Reports-POSvsAsycuda-Results]
where ((isnull([Asycuda-Quantity], 0) = 0 and isnull([OPS-QuantityOnHand], 0) = 0) --- remain quantities is zero for both asycuda and ops
        --and (isnull([Asycuda-PiQuantity], 0)) > isnull([OPS-Quantity],0)
		)--- and pi is less than quantity in question
		and (diff = 0)
		and isnull([Asycuda-Quantity], 0) + isnull([OPS-Quantity],0) = 0
GO
