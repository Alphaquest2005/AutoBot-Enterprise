USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AscyudaItemPiQuantity-Basic]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[AscyudaItemPiQuantity-Basic]
AS
SELECT Item_Id, cast(SUM(PiQuantity) as float) AS PiQuantity, cast(SUM(PiWeight) as float) AS PiWeight
FROM    dbo.[AsycudaItemPiQuantityData-Basic] WITH (NOLOCK)
GROUP BY Item_Id 
GO
