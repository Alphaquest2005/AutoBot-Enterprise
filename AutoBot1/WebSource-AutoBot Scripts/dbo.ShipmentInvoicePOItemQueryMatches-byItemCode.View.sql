USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemQueryMatches-byItemCode]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

Create view [dbo].[ShipmentInvoicePOItemQueryMatches-byItemCode]
as 

WITH cte AS
(
   SELECT *,
         ROW_NUMBER() OVER (PARTITION BY poitemcode ORDER BY rankno DESC) AS rn
   FROM (select * from [dbo].[ShipmentInvoicePOItemQueryMatches-Alias]
union
select * from [dbo].[ShipmentInvoicePOItemQueryMatches-Match])as t
) 
SELECT *
FROM cte
WHERE rn = 1 -- and poitemcode = 'AAA/50071'

GO
