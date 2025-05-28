-- OCR Correction Learning Table
-- This table stores detailed information about OCR regex corrections made by DeepSeek
-- for learning and analysis purposes

USE [WebSource-AutoBot]
GO

-- Create the OCRCorrectionLearning table
CREATE TABLE [dbo].[OCRCorrectionLearning](
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [FieldName] [nvarchar](100) NOT NULL,
    [OriginalError] [nvarchar](500) NOT NULL,
    [CorrectValue] [nvarchar](500) NOT NULL,
    [LineNumber] [int] NOT NULL,
    [LineText] [nvarchar](1000) NOT NULL,
    [WindowText] [nvarchar](max) NULL,
    [ExistingRegex] [nvarchar](1000) NULL,
    [CorrectionType] [nvarchar](50) NOT NULL, -- UpdateLineRegex, AddFieldFormatRegex, CreateNewRegex
    [NewRegexPattern] [nvarchar](1000) NULL,
    [ReplacementPattern] [nvarchar](500) NULL,
    [DeepSeekReasoning] [nvarchar](max) NULL,
    [Confidence] [decimal](3,2) NULL, -- 0.00 to 1.00
    [InvoiceType] [nvarchar](100) NULL,
    [FilePath] [nvarchar](500) NULL,
    [FieldId] [int] NULL, -- Foreign key to OCR Fields table
    [Success] [bit] NOT NULL DEFAULT(1),
    [ErrorMessage] [nvarchar](1000) NULL,
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT(GETUTCDATE()),
    [CreatedBy] [nvarchar](100) NULL DEFAULT('OCRCorrectionService'),
    [ProcessingTimeMs] [int] NULL,
    [DeepSeekPrompt] [nvarchar](max) NULL, -- Store the prompt used for debugging
    [DeepSeekResponse] [nvarchar](max) NULL, -- Store the full response for debugging
    
    CONSTRAINT [PK_OCRCorrectionLearning] PRIMARY KEY CLUSTERED ([Id] ASC)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- Create indexes for better performance
CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_FieldName] ON [dbo].[OCRCorrectionLearning]
(
    [FieldName] ASC
)
INCLUDE ([OriginalError], [CorrectValue], [CorrectionType], [CreatedDate])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_CorrectionType] ON [dbo].[OCRCorrectionLearning]
(
    [CorrectionType] ASC
)
INCLUDE ([FieldName], [Success], [CreatedDate])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_CreatedDate] ON [dbo].[OCRCorrectionLearning]
(
    [CreatedDate] DESC
)
INCLUDE ([FieldName], [CorrectionType], [Success])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_FieldId] ON [dbo].[OCRCorrectionLearning]
(
    [FieldId] ASC
)
INCLUDE ([FieldName], [CorrectionType], [Success], [CreatedDate])
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Add foreign key constraint to OCR Fields table (if it exists)
-- Uncomment this if you want to enforce referential integrity
-- ALTER TABLE [dbo].[OCRCorrectionLearning] WITH CHECK ADD CONSTRAINT [FK_OCRCorrectionLearning_Fields] 
-- FOREIGN KEY([FieldId]) REFERENCES [dbo].[Fields] ([Id])
-- GO

-- Add check constraints for data validation
ALTER TABLE [dbo].[OCRCorrectionLearning] WITH CHECK ADD CONSTRAINT [CK_OCRCorrectionLearning_CorrectionType] 
CHECK ([CorrectionType] IN ('UpdateLineRegex', 'AddFieldFormatRegex', 'CreateNewRegex'))
GO

ALTER TABLE [dbo].[OCRCorrectionLearning] WITH CHECK ADD CONSTRAINT [CK_OCRCorrectionLearning_Confidence] 
CHECK ([Confidence] IS NULL OR ([Confidence] >= 0.00 AND [Confidence] <= 1.00))
GO

-- Add some sample documentation
EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Stores detailed information about OCR regex corrections made by DeepSeek LLM for learning and analysis purposes', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'OCRCorrectionLearning'
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'The field name that was being corrected (e.g., TotalInternalFreight, TotalOtherCost)', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'OCRCorrectionLearning', 
    @level2type=N'COLUMN', @level2name=N'FieldName'
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'The incorrect value that was found in the OCR text', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'OCRCorrectionLearning', 
    @level2type=N'COLUMN', @level2name=N'OriginalError'
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'The correct value that should have been extracted', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'OCRCorrectionLearning', 
    @level2type=N'COLUMN', @level2name=N'CorrectValue'
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'The type of correction applied: UpdateLineRegex, AddFieldFormatRegex, or CreateNewRegex', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'OCRCorrectionLearning', 
    @level2type=N'COLUMN', @level2name=N'CorrectionType'
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'DeepSeek LLM explanation of why this correction approach was chosen', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'OCRCorrectionLearning', 
    @level2type=N'COLUMN', @level2name=N'DeepSeekReasoning'
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'DeepSeek confidence level in the correction (0.00 to 1.00)', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'OCRCorrectionLearning', 
    @level2type=N'COLUMN', @level2name=N'Confidence'
GO

-- Create a view for easy analysis
CREATE VIEW [dbo].[vw_OCRCorrectionAnalysis] AS
SELECT 
    FieldName,
    CorrectionType,
    COUNT(*) as TotalCorrections,
    AVG(Confidence) as AvgConfidence,
    SUM(CASE WHEN Success = 1 THEN 1 ELSE 0 END) as SuccessfulCorrections,
    SUM(CASE WHEN Success = 0 THEN 1 ELSE 0 END) as FailedCorrections,
    CAST(SUM(CASE WHEN Success = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) as SuccessRate,
    MIN(CreatedDate) as FirstCorrection,
    MAX(CreatedDate) as LastCorrection
FROM [dbo].[OCRCorrectionLearning]
GROUP BY FieldName, CorrectionType
GO

PRINT 'OCRCorrectionLearning table and related objects created successfully!'
PRINT 'Remember to:'
PRINT '1. Update your EDMX files to include the new table'
PRINT '2. Regenerate your Entity Framework models'
PRINT '3. Build the project to get the auto-generated services and view models'
