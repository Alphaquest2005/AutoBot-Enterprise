USE [WebSource-AutoBot]
GO

/****** Object:  Table [dbo].[ImportIssueLog]    Script Date: 5/25/2025 9:01:19 PM ******/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ImportIssueLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailProcessingQueueId] [int] NOT NULL,
	[ShipmentInvoiceId] [int] NOT NULL,
	[IssueType] [nvarchar](100) NOT NULL,
	[IssueDescription] [nvarchar](max) NOT NULL,
	[TotalZeroValue] [float] NOT NULL,
	[DetailLevelDifference] [float] NULL,
	[HeaderLevelDifference] [float] NULL,
	[ExpectedTotal] [float] NULL,
	[ActualTotal] [float] NULL,
	[CorrectionAttempted] [bit] NOT NULL DEFAULT 0,
	[CorrectionSuccessful] [bit] NULL,
	[CorrectionMethod] [nvarchar](255) NULL,
	[CorrectionNotes] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT GETDATE(),
	[ResolvedDate] [datetime] NULL,
	[OCRTemplateUsed] [nvarchar](255) NULL,
	[InvoiceType] [nvarchar](100) NULL,
	[OriginalOCRText] [nvarchar](max) NULL,
 CONSTRAINT [PK_ImportIssueLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ImportIssueLog]  WITH CHECK ADD  CONSTRAINT [FK_ImportIssueLog_EmailProcessingQueue] FOREIGN KEY([EmailProcessingQueueId])
REFERENCES [dbo].[EmailProcessingQueue] ([Id])
GO

ALTER TABLE [dbo].[ImportIssueLog] CHECK CONSTRAINT [FK_ImportIssueLog_EmailProcessingQueue]
GO

ALTER TABLE [dbo].[ImportIssueLog]  WITH CHECK ADD  CONSTRAINT [FK_ImportIssueLog_ShipmentInvoice] FOREIGN KEY([ShipmentInvoiceId])
REFERENCES [dbo].[ShipmentInvoice] ([Id])
GO

ALTER TABLE [dbo].[ImportIssueLog] CHECK CONSTRAINT [FK_ImportIssueLog_ShipmentInvoice]
GO
