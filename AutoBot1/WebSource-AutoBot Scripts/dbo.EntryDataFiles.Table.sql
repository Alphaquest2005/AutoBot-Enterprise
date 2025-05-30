USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryDataFiles]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryDataFiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[EmailId] [nvarchar](255) NULL,
	[FileTypeId] [int] NULL,
	[FileType] [nvarchar](50) NOT NULL,
	[SourceFile] [nvarchar](max) NOT NULL,
	[SourceRow] [nvarchar](max) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
	[EntryDataId] [nvarchar](50) NULL,
	[EntryDataDate] [datetime2](7) NULL,
	[LineNumber] [int] NULL,
	[ItemNumber] [nvarchar](20) NULL,
	[Quantity] [float] NULL,
	[Units] [nvarchar](15) NULL,
	[ItemDescription] [nvarchar](255) NULL,
	[Cost] [float] NULL,
	[TotalCost] [float] NULL,
	[InvoiceQty] [float] NULL,
	[ReceivedQty] [float] NULL,
	[TaxAmount] [float] NULL,
 CONSTRAINT [PK_EntryDataFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
