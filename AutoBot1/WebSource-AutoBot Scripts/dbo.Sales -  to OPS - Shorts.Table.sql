USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Sales -> to OPS - Shorts]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sales -> to OPS - Shorts](
	[EntryDataId] [nvarchar](50) NOT NULL,
	[ItemNumber] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[Quantity] [float] NULL,
	[OPS-Quantity] [float] NOT NULL,
	[OPSCost] [float] NULL,
	[Asycuda-Quantity] [float] NULL,
	[AsycudaCost] [float] NULL,
	[Diff] [float] NOT NULL,
	[Cost] [float] NULL,
	[TotalCost] [float] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
