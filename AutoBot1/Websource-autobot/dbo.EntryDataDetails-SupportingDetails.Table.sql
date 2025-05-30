USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryDataDetails-SupportingDetails]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryDataDetails-SupportingDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryDataDetailsId] [int] NOT NULL,
	[CLineNumber] [int] NULL,
	[Quantity] [float] NOT NULL,
	[Cost] [float] NOT NULL,
	[PreviousInvoiceNumber] [nvarchar](50) NULL,
	[CNumber] [nvarchar](50) NULL,
	[EffectiveDate] [datetime2](7) NULL,
	[TaxAmount] [float] NULL,
	[FileLineNumber] [int] NULL,
	[EntryDataDetailsKey]  AS ((replace([EntryDataId],' ','')+'|')+rtrim(ltrim(CONVERT([varchar](50),[LineNumber])))) PERSISTED,
	[EntryDataId] [nvarchar](50) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
	[PreviousEntryDataDetailsId] [int] NOT NULL,
	[LineNumber] [int] NULL,
	[Comment] [nvarchar](255) NULL,
 CONSTRAINT [PK_EntryDataDetails-SupportingDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EntryDataDetails-SupportingDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryDataDetails-SupportingDetails_EntryDataDetails] FOREIGN KEY([EntryDataDetailsId])
REFERENCES [dbo].[EntryDataDetails] ([EntryDataDetailsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryDataDetails-SupportingDetails] CHECK CONSTRAINT [FK_EntryDataDetails-SupportingDetails_EntryDataDetails]
GO
