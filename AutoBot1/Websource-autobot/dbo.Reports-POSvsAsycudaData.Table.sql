USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Reports-POSvsAsycudaData]    Script Date: 3/27/2025 1:48:24 AM ******/
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
