USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaSalesAllocations-ExpectedSales]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaSalesAllocations-ExpectedSales](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryDataId] [nvarchar](50) NOT NULL,
	[EntryDataDate] [datetime] NOT NULL,
	[ItemNumber] [nvarchar](50) NOT NULL,
	[Quantity] [float] NOT NULL,
	[DutyFreePaid] [bit] NOT NULL,
 CONSTRAINT [PK_AsycudaSalesAllocations-ExpectedSales] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
