USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Attached_documents]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Attached_documents](
	[Attached_document_date] [nvarchar](255) NULL,
	[Attached_documents_Id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NULL,
	[Attached_document_code] [nvarchar](100) NULL,
	[Attached_document_name] [nvarchar](100) NULL,
	[Attached_document_reference] [nvarchar](100) NULL,
	[Attached_document_from_rule] [int] NULL,
 CONSTRAINT [PK_xcuda_Attached_documents] PRIMARY KEY CLUSTERED 
(
	[Attached_documents_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_xcuda_Attached_documents]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [IX_xcuda_Attached_documents] ON [dbo].[xcuda_Attached_documents]
(
	[Item_Id] ASC,
	[Attached_document_code] ASC,
	[Attached_document_reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Attached_documents_130_129]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Attached_documents_130_129] ON [dbo].[xcuda_Attached_documents]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Attached_documents_169_168]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Attached_documents_169_168] ON [dbo].[xcuda_Attached_documents]
(
	[Attached_document_code] ASC
)
INCLUDE([Item_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Attached_documents_2_1]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Attached_documents_2_1] ON [dbo].[xcuda_Attached_documents]
(
	[Attached_document_reference] ASC
)
INCLUDE([Item_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Attached_documents_4_3]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Attached_documents_4_3] ON [dbo].[xcuda_Attached_documents]
(
	[Attached_document_code] ASC,
	[Attached_document_reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Attached_documents]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Attached_documents] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Attached_documents] CHECK CONSTRAINT [FK_Item_Attached_documents]
GO
