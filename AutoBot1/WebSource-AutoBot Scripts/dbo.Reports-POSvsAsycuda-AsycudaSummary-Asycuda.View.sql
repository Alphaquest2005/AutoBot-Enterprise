USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-AsycudaSummary-Asycuda]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[Reports-POSvsAsycuda-AsycudaSummary-Asycuda]
AS

select ItemNumber,InventoryItemId , Max(Description) as Description, sum(quantity) as Quantity, sum(amount) as Amount, sum(PiQuantity) as PiQuantity, avg(cost) as Cost
--into [#AsycudaSummary-Asycuda]
from dbo.[Reports-POSvsAsycuda-AsycudaData] as [#AsycudaData]
group by ItemNumber,InventoryItemId
GO
