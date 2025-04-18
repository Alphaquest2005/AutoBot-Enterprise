USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-AsycudaSummary-Asycuda]    Script Date: 3/27/2025 1:48:24 AM ******/
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
