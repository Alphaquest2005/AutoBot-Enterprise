USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryData-Summary]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryData-Summary](
	[EntryData_Id] [int] NOT NULL,
	[Total] [float] NOT NULL,
	[AllocatedTotal] [float] NOT NULL,
	[TotalLines] [float] NOT NULL,
	[DetailsTax] [float] NULL,
 CONSTRAINT [PK_EntryData-Summary] PRIMARY KEY CLUSTERED 
(
	[EntryData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[EntryData-Summary] ([EntryData_Id], [Total], [AllocatedTotal], [TotalLines], [DetailsTax]) VALUES (507245, 119.97000503540039, 0, 1, 0)
GO
ALTER TABLE [dbo].[EntryData-Summary]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryData-Summary_EntryData] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryData-Summary] CHECK CONSTRAINT [FK_EntryData-Summary_EntryData]
GO
