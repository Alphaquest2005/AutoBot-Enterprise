USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Reports-POSvsAsycudaData]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reports-POSvsAsycudaData](
	[StartDate] [datetime2](7) NOT NULL,
	[EndDate] [datetime2](7) NOT NULL,
	[OPSNumber] [nvarchar](50) NOT NULL,
	[EntryData_Id] [int] NULL,
	[ApplicationSettingsId] [int] NOT NULL
) ON [PRIMARY]
GO
