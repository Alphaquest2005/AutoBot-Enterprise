USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaSalesAllocations-ExpectedSales]    Script Date: 3/27/2025 1:48:24 AM ******/
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
