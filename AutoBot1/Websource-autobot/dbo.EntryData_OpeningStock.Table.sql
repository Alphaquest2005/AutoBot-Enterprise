USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryData_OpeningStock]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryData_OpeningStock](
	[OPSNumber] [nvarchar](50) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
 CONSTRAINT [PK_EntryData_OpeningStock] PRIMARY KEY CLUSTERED 
(
	[EntryData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EntryData_OpeningStock]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryData_OpeningStock_EntryData1] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryData_OpeningStock] CHECK CONSTRAINT [FK_EntryData_OpeningStock_EntryData1]
GO
