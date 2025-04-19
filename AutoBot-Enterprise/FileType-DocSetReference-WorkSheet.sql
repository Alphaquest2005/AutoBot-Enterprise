
ALTER TABLE [dbo].[FileTypes]
    ADD [DocSetRefernece] NVARCHAR (50) NULL;

	go

UPDATE FileTypes
SET         DocSetRefernece = AsycudaDocumentSet.Declarant_Reference_Number
FROM    AsycudaDocumentSet INNER JOIN
                 FileTypes ON AsycudaDocumentSet.AsycudaDocumentSetId = FileTypes.AsycudaDocumentSetId

GO
PRINT N'Dropping Foreign Key [dbo].[FK_FileTypes_AsycudaDocumentSet]...';


GO
ALTER TABLE [dbo].[FileTypes] DROP CONSTRAINT [FK_FileTypes_AsycudaDocumentSet];


GO
PRINT N'Altering Table [dbo].[FileTypes]...';


GO
ALTER TABLE [dbo].[FileTypes] DROP COLUMN [AsycudaDocumentSetId];