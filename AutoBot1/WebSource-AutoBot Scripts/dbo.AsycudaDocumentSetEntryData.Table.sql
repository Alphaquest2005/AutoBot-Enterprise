USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaDocumentSetEntryData]    Script Date: 4/3/2025 10:23:54 PM ******/
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
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSetEntryData] ON 

INSERT [dbo].[AsycudaDocumentSetEntryData] ([AsycudaDocumentSetId], [Id], [EntryData_Id]) VALUES (5381, 576772, 507282)
INSERT [dbo].[AsycudaDocumentSetEntryData] ([AsycudaDocumentSetId], [Id], [EntryData_Id]) VALUES (5381, 576774, 507283)
INSERT [dbo].[AsycudaDocumentSetEntryData] ([AsycudaDocumentSetId], [Id], [EntryData_Id]) VALUES (5381, 576776, 507284)
INSERT [dbo].[AsycudaDocumentSetEntryData] ([AsycudaDocumentSetId], [Id], [EntryData_Id]) VALUES (7847, 576771, 507282)
INSERT [dbo].[AsycudaDocumentSetEntryData] ([AsycudaDocumentSetId], [Id], [EntryData_Id]) VALUES (7849, 576773, 507283)
INSERT [dbo].[AsycudaDocumentSetEntryData] ([AsycudaDocumentSetId], [Id], [EntryData_Id]) VALUES (7850, 576775, 507284)
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSetEntryData] OFF
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSetEntryData_27_26]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentSetEntryData_27_26] ON [dbo].[AsycudaDocumentSetEntryData]
(
	[AsycudaDocumentSetId] ASC
)
INCLUDE([EntryData_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentSetEntryData_79_78]    Script Date: 4/3/2025 10:23:55 PM ******/
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
