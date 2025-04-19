USE [IWW-ENTERPRISEDB]
GO

/****** Object:  View [dbo].[CounterPointSales]    Script Date: 9/22/2020 12:26:49 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create VIEW [dbo].[CounterPointSales]
AS
SELECT        TOP (100) PERCENT Sales.INVNO, Sales.DATE, Sales.TAX_AMT, Sales.[CUSTOMER NAME], COUNT(Sales.INVNO) AS LIN_CNT, CAST(CASE WHEN INVNumber IS NULL THEN 0 ELSE 1 END AS bit) AS Downloaded
FROM            dbo.CounterPointSalesDetails AS Sales LEFT OUTER JOIN
                         dbo.EntryData_Sales ON Sales.INVNO COLLATE DATABASE_DEFAULT = dbo.EntryData_Sales.INVNumber COLLATE DATABASE_DEFAULT
WHERE        (CAST(Sales.DATE AS datetime) >=
                             (SELECT        OpeningStockDate
                               FROM            dbo.ApplicationSettings))
GROUP BY Sales.INVNO, Sales.DATE, Sales.TAX_AMT, Sales.[CUSTOMER NAME], dbo.EntryData_Sales.INVNumber
ORDER BY Sales.DATE DESC
GO


