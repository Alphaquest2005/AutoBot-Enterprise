USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaDocumentEntryData]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaDocumentEntryData](
	[AsycudaDocumentId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
 CONSTRAINT [PK_AsycudaDocumentEntryData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentEntryData_690_689]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentEntryData_690_689] ON [dbo].[AsycudaDocumentEntryData]
(
	[EntryData_Id] ASC
)
INCLUDE([AsycudaDocumentId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_AsycudaDocumentEntryData_784_783]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_AsycudaDocumentEntryData_784_783] ON [dbo].[AsycudaDocumentEntryData]
(
	[AsycudaDocumentId] ASC
)
INCLUDE([EntryData_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AsycudaDocumentEntryData]  WITH NOCHECK ADD  CONSTRAINT [FK_AsycudaDocumentEntryData_EntryData1] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocumentEntryData] CHECK CONSTRAINT [FK_AsycudaDocumentEntryData_EntryData1]
GO
ALTER TABLE [dbo].[AsycudaDocumentEntryData]  WITH NOCHECK ADD  CONSTRAINT [FK_AsycudaDocumentEntryData_xcuda_ASYCUDA] FOREIGN KEY([AsycudaDocumentId])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AsycudaDocumentEntryData] CHECK CONSTRAINT [FK_AsycudaDocumentEntryData_xcuda_ASYCUDA]
GO
