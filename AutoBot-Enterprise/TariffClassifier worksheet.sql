USE [BudgetMarine-AutoBot]
GO

/****** Object:  View [dbo].[StMarteenInventory-Classifier]    Script Date: 3/21/2022 7:47:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





alter VIEW [dbo].[StMarteenInventory-Classifier]
AS

select * from 
(select *, row_number() over(partition by [Internal Reference] order by[Rank] desc) as row1 from 
(SELECT  dbo.StMarteenInventory.[Internal Reference], dbo.StMarteenInventory.Name,[Primary Desc],[Category ID],  isnull(dbo.TariffCodes.TariffCode, TariffCategoryDefaultKeyWord.TariffCode) as TariffCode, isnull(KeyWords.Keyword,'Category Default') as Keyword, KeyWords.IsException
,
				 (RANk() over (Partition by [Internal Reference] order by isnull(TarrifKeyWordCategory.TariffCategoryCode,'9999'),len(isnull(keywords.keyword, exceptions.keyword))))  as Rank
FROM    dbo.TariffCodes INNER JOIN
                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
                      FROM     dbo.TariffKeyWords
                      WHERE  (IsException = 0)) AS KeyWords ON dbo.TariffCodes.TariffCode = KeyWords.TariffCode LEFT OUTER JOIN
                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
                      FROM     dbo.TariffKeyWords AS TariffKeyWords_1
                      WHERE  (IsException = 1)) AS Exceptions ON dbo.TariffCodes.TariffCode = Exceptions.TariffCode RIGHT OUTER JOIN
                 dbo.StMarteenInventory ON (Exceptions.Keyword IS NULL or
                  CHARINDEX(Exceptions.Keyword, 
                 dbo.StMarteenInventory.Name) <= 0) AND CHARINDEX(KeyWords.Keyword, dbo.StMarteenInventory.Name) > 0 right OUTER JOIN
				 TarrifKeyWordCategory on StMarteenInventory.[Category ID] = TarrifKeyWordCategory.CategoryId
				 left outer join TariffCategoryDefaultKeyWord on dbo.StMarteenInventory.[Category ID] = TariffCategoryDefaultKeyWord.CategoryId
	where  (TarrifKeyWordCategory.TariffCategoryCode is null or TariffCodes.TariffCategoryCode is null  or TariffCodes.TariffCategoryCode = TarrifKeyWordCategory.TariffCategoryCode) 
				 ) as t) as tt
where row1 = 1

--SELECT StMarteenInventory.[Internal Reference], StMarteenInventory.Name, StMarteenInventory.[Primary Desc], StMarteenInventory.[Category ID], TariffCodes.TariffCode, KeyWords.Keyword, KeyWords.IsException
--,
--				 (RANk() over (Partition by [Internal Reference] order by isnull(TarrifKeyWordCategory.TariffCategoryCode,'9999'),len(isnull(keywords.keyword, exceptions.keyword))))  as Rank


--FROM    TarrifKeyWordCategory RIGHT OUTER JOIN
--                 StMarteenInventory ON TarrifKeyWordCategory.CategoryId = StMarteenInventory.[Category ID] LEFT OUTER JOIN
--                 TariffCodes INNER JOIN
--                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
--                      FROM     TariffKeyWords
--                      WHERE  (IsException = 0)) AS KeyWords ON TariffCodes.TariffCode = KeyWords.TariffCode LEFT OUTER JOIN
--                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
--                      FROM     TariffKeyWords AS TariffKeyWords_1
--                      WHERE  (IsException = 1)) AS Exceptions ON TariffCodes.TariffCode = Exceptions.TariffCode ON (Exceptions.Keyword IS NULL OR
--                 CHARINDEX(Exceptions.Keyword, StMarteenInventory.Name) <= 0) AND CHARINDEX(KeyWords.Keyword, StMarteenInventory.Name) > 0

	

GO

select * from [StMarteenInventory-Classifier] where [Category ID] = 735 and Name like '%Hand%'
--select * from [StMarteenInventory-Classifier-NoCategoryLimit] where [Category ID] = 735


INSERT INTO TarrifKeyWordCategory
                 (CategoryId, TariffCategoryCode)
SELECT 529 AS Expr1, TariffCategoryCode
FROM    TarrifKeyWordCategory AS TarrifKeyWordCategory_1
WHERE (CategoryId = 701)

