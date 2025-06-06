USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetEntryDataPackages]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentSetEntryDataPackages]
AS

--WITH DedupedEntryData AS (
--    SELECT 
--        *,
--        -- Partition by the business key (first column + EntryDataDate)
--        ROW_NUMBER() OVER (
--            PARTITION BY entrydataid, EntryDataDate  -- Replace [Column1] with the actual name (e.g., InvoiceNumber)
--            ORDER BY EntryData_Id  -- Keep the first occurrence of duplicates
--        ) AS RowNum
--    FROM EntryData
--)
--SELECT 
--    adse.AsycudaDocumentSetId,
--    SUM(ed.Packages) AS Packages
--FROM DedupedEntryData ed
--INNER JOIN AsycudaDocumentSetEntryData adse 
--    ON ed.EntryData_Id = adse.EntryData_Id
--WHERE ed.RowNum = 1  -- Keep only one row per business key
--GROUP BY adse.AsycudaDocumentSetId;


SELECT AsycudaDocumentSetEntryData.AsycudaDocumentSetId, SUM( EntryData.Packages) AS Packages
FROM    EntryData INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id
GROUP BY AsycudaDocumentSetEntryData.AsycudaDocumentSetId
GO
