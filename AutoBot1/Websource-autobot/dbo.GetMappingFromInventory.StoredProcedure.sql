USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[GetMappingFromInventory]    Script Date: 3/27/2025 1:48:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		<Author,,Name>
-- Alter date: <Alter Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetMappingFromInventory]
	-- Add the parameters for the stored procedure here
	@appsettingId int
AS
BEGIN
	insert into InventoryMapping (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
SELECT wd.ItemNumber, nd.ItemNumber AS Expr1, wd.ApplicationSettingsId
FROM    (SELECT ItemNumber, ApplicationSettingsId
                 FROM     InventoryItems
                 WHERE  (ApplicationSettingsId = @appsettingId)
                 GROUP BY ItemNumber, ApplicationSettingsId
                 HAVING (ItemNumber LIKE N'%-%') OR
                                  (ItemNumber LIKE N'%/%')) AS wd INNER JOIN
                     (SELECT ItemNumber
						  FROM     InventoryItems AS InventoryItems_1
						  WHERE  (ApplicationSettingsId = @appsettingId)
						  GROUP BY ItemNumber
						  HAVING (ItemNumber NOT LIKE N'%-%') or (ItemNumber NOT LIKE N'%/%')
					  ) AS nd
					   ON REPLACE(REPLACE(wd.ItemNumber, '-', ''), '/', '') = REPLACE(REPLACE(nd.ItemNumber, '-', ''), '/', '') and wd.ItemNumber <> nd.ItemNumber LEFT OUTER JOIN
                 InventoryMapping ON wd.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId AND wd.ItemNumber = InventoryMapping.POSItemCode AND nd.ItemNumber = InventoryMapping.AsycudaItemCode
WHERE (InventoryMapping.POSItemCode IS NULL)


insert into InventoryMapping (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
SELECT wd.ItemNumber, nd.ItemNumber AS Expr1, wd.ApplicationSettingsId
FROM    (SELECT ItemNumber, ApplicationSettingsId
                 FROM     InventoryItems
                 WHERE  (ApplicationSettingsId = @appsettingId)
                 GROUP BY ItemNumber, ApplicationSettingsId) AS wd INNER JOIN
                     (SELECT ItemNumber
						  FROM     InventoryItems AS InventoryItems_1
						  WHERE  (ApplicationSettingsId = @appsettingId)
						  GROUP BY ItemNumber
					  ) AS nd
					   ON REPLACE(LTRIM(REPLACE(wd.ItemNumber, '0', ' ')), ' ', '0') = REPLACE(LTRIM(REPLACE(nd.ItemNumber, '0', ' ')), ' ', '0') and wd.ItemNumber <> nd.ItemNumber LEFT OUTER JOIN
                 InventoryMapping ON wd.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId AND wd.ItemNumber = InventoryMapping.POSItemCode AND nd.ItemNumber = InventoryMapping.AsycudaItemCode
WHERE (InventoryMapping.POSItemCode IS NULL )

insert into InventoryMapping (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
SELECT wd.ItemNumber, nd.ItemNumber AS Expr1, wd.ApplicationSettingsId
FROM    (SELECT ItemNumber, ApplicationSettingsId
                 FROM     InventoryItems
                 WHERE  (ApplicationSettingsId = @appsettingId)
                 GROUP BY ItemNumber, ApplicationSettingsId) AS wd INNER JOIN
                     (SELECT ItemNumber
						  FROM     InventoryItems AS InventoryItems_1
						  WHERE  (ApplicationSettingsId = @appsettingId) 
						  GROUP BY ItemNumber
					  ) AS nd
					   ON  REPLACE(LTRIM(REPLACE(wd.ItemNumber, '0', ' ')), ' ', '0') = nd.ItemNumber and wd.ItemNumber <> nd.ItemNumber /*or wd.ItemNumber = REPLACE(LTRIM(REPLACE(nd.ItemNumber, '0', ' ')), ' ', '0')*/ LEFT OUTER JOIN
                InventoryMapping ON wd.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId AND wd.ItemNumber = InventoryMapping.POSItemCode AND nd.ItemNumber = InventoryMapping.AsycudaItemCode
WHERE (InventoryMapping.POSItemCode IS NULL )

insert into InventoryMapping (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
SELECT wd.ItemNumber, nd.ItemNumber AS Expr1, wd.ApplicationSettingsId
FROM    (SELECT ItemNumber, ApplicationSettingsId
                 FROM     InventoryItems
                 WHERE  (ApplicationSettingsId = @appsettingId)
                 GROUP BY ItemNumber, ApplicationSettingsId) AS wd INNER JOIN
                     (SELECT ItemNumber
						  FROM     InventoryItems AS InventoryItems_1
						  WHERE  (ApplicationSettingsId = @appsettingId) 
						  GROUP BY ItemNumber
					  ) AS nd
					   ON  wd.ItemNumber = REPLACE(LTRIM(REPLACE(nd.ItemNumber, '0', ' ')), ' ', '0') and wd.ItemNumber <> nd.ItemNumber  LEFT OUTER JOIN
                 InventoryMapping ON wd.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId AND wd.ItemNumber = InventoryMapping.POSItemCode AND nd.ItemNumber = InventoryMapping.AsycudaItemCode
WHERE (InventoryMapping.POSItemCode IS NULL )

insert into InventoryMapping (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
SELECT wd.ItemNumber, nd.ItemNumber AS Expr1, wd.ApplicationSettingsId
FROM    (SELECT ItemNumber, ApplicationSettingsId
                 FROM     InventoryItems
                 WHERE  (ApplicationSettingsId = @appsettingId)
                 GROUP BY ItemNumber, ApplicationSettingsId) AS wd INNER JOIN
                     (SELECT ItemNumber
						  FROM     InventoryItems AS InventoryItems_1
						  WHERE  (ApplicationSettingsId = @appsettingId) 
						  GROUP BY ItemNumber
					  ) AS nd
					   ON  REPLACE(REPLACE(REPLACE(LTRIM(REPLACE(wd.ItemNumber, '0', ' ')), ' ', '0'), '-', ''), '/', '')  = nd.ItemNumber and wd.ItemNumber <> nd.ItemNumber /*or wd.ItemNumber = REPLACE(LTRIM(REPLACE(nd.ItemNumber, '0', ' ')), ' ', '0')*/ LEFT OUTER JOIN
                InventoryMapping ON wd.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId AND wd.ItemNumber = InventoryMapping.POSItemCode AND nd.ItemNumber = InventoryMapping.AsycudaItemCode
WHERE (InventoryMapping.POSItemCode IS NULL )


	insert into InventoryMapping (POSItemCode, AsycudaItemCode, ApplicationsSettingsId)
SELECT wd.ItemNumber, nd.ItemNumber AS Expr1, wd.ApplicationSettingsId
FROM    (SELECT ItemNumber, ApplicationSettingsId
                 FROM     InventoryItems
                 WHERE  (ApplicationSettingsId = @appsettingId)
                 GROUP BY ItemNumber, ApplicationSettingsId
                 HAVING (trim(ItemNumber) LIKE N'% %')) AS wd INNER JOIN
                     (SELECT ItemNumber
						  FROM     InventoryItems AS InventoryItems_1
						  WHERE  (ApplicationSettingsId = @appsettingId)
						  GROUP BY ItemNumber
						  HAVING (trim(ItemNumber) NOT LIKE N'% %')
					  ) AS nd
					   ON REPLACE(wd.ItemNumber, ' ', '') = nd.ItemNumber and wd.ItemNumber <> nd.ItemNumber LEFT OUTER JOIN
                 InventoryMapping ON wd.ApplicationSettingsId = InventoryMapping.ApplicationsSettingsId AND wd.ItemNumber = InventoryMapping.POSItemCode AND nd.ItemNumber = InventoryMapping.AsycudaItemCode
WHERE (InventoryMapping.POSItemCode IS NULL);

---------- part item numbers

-- CTE for splitting item numbers
WITH RecursiveSplit AS (
    SELECT 
        4 AS StartLength
    UNION ALL
    SELECT 
        StartLength + 1
    FROM RecursiveSplit
    WHERE StartLength <= 20
), Split_ItemNumbers AS (
    SELECT 
        itemnumber,
        CAST(SUBSTRING(itemnumber, 1, StartLength) AS VARCHAR(50)) AS part,
        CAST(STUFF(itemnumber, 1, StartLength, '') AS VARCHAR(50)) AS remainder,
        StartLength AS part_length, ApplicationSettingsId
    FROM InventoryItems
    CROSS JOIN RecursiveSplit
    UNION ALL
    SELECT 
        itemnumber,
        CAST(SUBSTRING(remainder, 1, part_length + 1) AS VARCHAR(50)),
        CAST(STUFF(remainder, 1, part_length + 1, '') AS VARCHAR(50)),
        part_length + 1, ApplicationSettingsId
    FROM Split_ItemNumbers
    WHERE LEN(remainder) > part_length
)

insert into InventoryMapping (POSItemCode,AsycudaItemCode , ApplicationsSettingsId)
-- Select from the CTE
SELECT DISTINCT part,itemnumber, ApplicationSettingsId
FROM Split_ItemNumbers left outer join InventoryMapping on Split_ItemNumbers.part = inventorymapping.POSItemCode and Split_ItemNumbers.ItemNumber = inventorymapping.AsycudaItemCode
WHERE part IN (SELECT itemnumber FROM InventoryItems) and ItemNumber <> part and inventorymapping.AsycudaItemCode is null;

end
GO
