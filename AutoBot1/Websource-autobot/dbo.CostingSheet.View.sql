USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[CostingSheet]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[CostingSheet] as
SELECT        CNumber, LineNumber, ItemNumber, Commercial_Description, ItemQuantity, [Received Qty], Total_CIF_itm, [Rate Factor], [Invoice Value], [External Freight], Insurance, [Internal Freight], [Other Cost], Deduction, CET, EVL, CSC, 
                         VAT, EXT
FROM            AsycudaItemCostingBase

GO
