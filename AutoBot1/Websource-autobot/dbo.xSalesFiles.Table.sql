USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xSalesFiles]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xSalesFiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[SourceFile] [nvarchar](255) NULL,
	[EmailId] [nvarchar](50) NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[Asycuda_Id] [int] NULL,
	[IsImported] [bit] NULL,
 CONSTRAINT [PK_xSalesFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
