USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemQueryMatches]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [dbo].[ShipmentInvoicePOItemQueryMatches]
as 

WITH cte AS
(
   SELECT *,
         ROW_NUMBER() OVER (PARTITION BY podetailsid,invdetailsid,poitemcode ORDER BY rankno DESC) AS rn
   FROM (select * from [dbo].[ShipmentInvoicePOItemQueryMatches-Alias]
union
select * from [dbo].[ShipmentInvoicePOItemQueryMatches-Match])as t
) 
SELECT *
FROM cte
WHERE rn = 1 -- and poitemcode = 'AAA/50071'

GO
