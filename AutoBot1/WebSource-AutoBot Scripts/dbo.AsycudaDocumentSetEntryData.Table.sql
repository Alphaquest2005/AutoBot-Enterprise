USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaDocumentSetEntryData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaDocumentSetEntryData](
	[AsycudaDocumentSetId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
 CONSTRAINT [PK_AsycudaDocumentSetEntryData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSetEntryData_27_26]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSetEntryData_27_26] ON [dbo].[AsycudaDocumentSetEntryData]
(
	[AsycudaDocumentSetId] ASC
)
INCLUDE([EntryData_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSetEntryData_79_78]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSetEntryData_79_78] ON [dbo].[AsycudaDocumentSetEntryData]
(
	[EntryData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AsycudaDocumentSetEntryData]  WITH NOCHECK ADD  CONSTRAINT [FK_AsycudaDocumentSetEntryData_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocumentSetEntryData] CHECK CONSTRAINT [FK_AsycudaDocumentSetEntryData_AsycudaDocumentSet]
GO
ALTER TABLE [dbo].[AsycudaDocumentSetEntryData]  WITH NOCHECK ADD  CONSTRAINT [FK_AsycudaDocumentSetEntryData_EntryData] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocumentSetEntryData] CHECK CONSTRAINT [FK_AsycudaDocumentSetEntryData_EntryData]
GO
