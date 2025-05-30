USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[Stp_TODO_ImportCompleteEntries]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Aasim Abdullah (https://sortbynull.com)
-- Alter date: June 04, 2021
-- Description:	This stored procedure is to replace nested view [dbo].[TODO-ImportCompleteEntries]
-- Sample execution call:  EXEC DBO.Stp_TODO_ImportCompleteEntries @ApplicationSettingsId = 2
-- Suggestion: It would be more better to add more parameters and filters which can reduce data set.
-- =============================================
CREATE PROCEDURE [dbo].[Stp_TODO_ImportCompleteEntries]
	@ApplicationSettingsId INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

 --------------Get all new CompleteEntries. Data in temp table will be used later on to filter records.  
	SELECT EX.AsycudaDocumentSetId, 
	EX.ApplicationSettingsId, 
	EX.EntryDataId, 
	EX.EntryData_Id, 
	EX.EmailId, 
	EX.FileTypeId, 
	y.ASYCUDA_Id, 
	y.DocumentType, 
	EX.EntryDataDetailsId
	INTO #ImportCompleteEntriesNew
	FROM    [dbo].[TODO-ImportCompleteEntries-New-Data] AS y INNER JOIN
						dbo.[Stp_TODO_ImportCompleteEntries-EntryDataDetailsEx] ex WITH (NOLOCK) ON y.EntryDataDetailsId = EX.EntryDataDetailsId INNER JOIN
						dbo.AsycudaDocumentSetEntryData WITH (NOLOCK) ON y.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id AND 
						EX.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId 
	WHERE ApplicationSettingsId =@ApplicationSettingsId

	GROUP BY EX.AsycudaDocumentSetId, EX.ApplicationSettingsId, EX.EntryDataId, EX.EntryData_Id, EX.EmailId, 
						EX.FileTypeId, y.ASYCUDA_Id, y.DocumentType, EX.EntryDataDetailsId


	SELECT 
	EX.AsycudaDocumentSetId, 
	EX.ApplicationSettingsId, 
	EX.EntryDataId, 
	EX.EntryData_Id, 


	EX.EmailId, 
	EX.FileTypeId, 
	z.ASYCUDA_Id, 
	z.DocumentType, 
	EX.EntryDataDetailsId
	INTO #ImportCompleteEntriesAccessed
	FROM    dbo.AsycudaDocumentItemEntryDataDetails  AS z WITH (NOLOCK) INNER JOIN
						dbo.[Stp_TODO_ImportCompleteEntries-EntryDataDetailsEx] EX WITH (NOLOCK) ON z.EntryDataDetailsId = EX.EntryDataDetailsId INNER JOIN
						dbo.AsycudaDocumentSetEntryData WITH (NOLOCK) ON z.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id AND 
						EX.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId 
		WHERE (z.ImportComplete = 1)-- 1=1
		AND z.ApplicationSettingsId =@ApplicationSettingsId
		--AND  --In orignla view this condition is not commented. Update it accordingly.
----Following EXISTS condition will filter records based on NewCompletedEntries
		AND EXISTS (SELECT 1 FROM #ImportCompleteEntriesNew WHERE EX.EntryDataDetailsId    = EntryDataDetailsId)
	GROUP BY EX.AsycudaDocumentSetId, EX.ApplicationSettingsId, EX.EntryDataId, EX.EntryData_Id, EX.EmailId, 
						EX.FileTypeId, z.ASYCUDA_Id, z.DocumentType, EX.EntryDataDetailsId

----Final Result Set

	SELECT 
	o.AsycudaDocumentSetId, 
	o.ApplicationSettingsId, 
	o.EntryDataId,  
	o.EntryData_Id, 
	o.EmailId, 
	o.FileTypeId, 
	i.ASYCUDA_Id as NewAsycuda_Id, 
	o.ASYCUDA_Id as AssessedAsycuda_Id
	FROM #ImportCompleteEntriesNew AS i WITH (NOLOCK)   INNER JOIN
						#ImportCompleteEntriesAccessed  AS o WITH (NOLOCK)
						ON   o.AsycudaDocumentSetId =i.AsycudaDocumentSetId
						AND o.DocumentType =i.DocumentType
						AND o.EntryDataDetailsId  = i.EntryDataDetailsId
	--where o.AsycudaDocumentSetId = 5395
	GROUP BY o.AsycudaDocumentSetId, o.ApplicationSettingsId, o.EntryDataId, o.EntryData_Id, o.EmailId, o.FileTypeId, i.ASYCUDA_Id, o.ASYCUDA_Id

	DROP TABLE #ImportCompleteEntriesAccessed,#ImportCompleteEntriesNew
END
GO
