USE [WebSource-AutoBot]
GO

/****** Object:  Table [dbo].[EmailProcessingQueue]    Script Date: 5/25/2025 9:01:19 PM ******/

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EmailProcessingQueue](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailId] [nvarchar](255) NOT NULL,
	[Subject] [nvarchar](max) NOT NULL,
	[ProcessingStatus] [nvarchar](50) NOT NULL DEFAULT 'Pending',
	[TotalInvoices] [int] NULL,
	[InvoicesWithIssues] [int] NULL,
	[TotalZeroSum] [float] NULL,
	[RetryCount] [int] NOT NULL DEFAULT 0,
	[MaxRetries] [int] NOT NULL DEFAULT 3,
	[LastProcessedDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL DEFAULT GETDATE(),
	[ErrorMessage] [nvarchar](max) NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[OriginalImportStatus] [nvarchar](50) NULL,
	[CorrectedImportStatus] [nvarchar](50) NULL,
	[ProcessingNotes] [nvarchar](max) NULL,
 CONSTRAINT [PK_EmailProcessingQueue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[EmailProcessingQueue]  WITH CHECK ADD  CONSTRAINT [FK_EmailProcessingQueue_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
GO

ALTER TABLE [dbo].[EmailProcessingQueue] CHECK CONSTRAINT [FK_EmailProcessingQueue_ApplicationSettings]
GO
