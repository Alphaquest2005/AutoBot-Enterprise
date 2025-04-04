USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Attached_documents]    Script Date: 4/3/2025 10:23:54 PM ******/
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
SET IDENTITY_INSERT [dbo].[xcuda_Attached_documents] ON 

INSERT [dbo].[xcuda_Attached_documents] ([Attached_document_date], [Attached_documents_Id], [Item_Id], [Attached_document_code], [Attached_document_name], [Attached_document_reference], [Attached_document_from_rule]) VALUES (N'4/3/2025', 99223, 1004680, N'IV05', NULL, N'7-111-8019845-2302666', NULL)
INSERT [dbo].[xcuda_Attached_documents] ([Attached_document_date], [Attached_documents_Id], [Item_Id], [Attached_document_code], [Attached_document_name], [Attached_document_reference], [Attached_document_from_rule]) VALUES (N'4/3/2025', 99224, 1004680, N'BL07', NULL, N'2-HAWB9595443-Manifest', NULL)
INSERT [dbo].[xcuda_Attached_documents] ([Attached_document_date], [Attached_documents_Id], [Item_Id], [Attached_document_code], [Attached_document_name], [Attached_document_reference], [Attached_document_from_rule]) VALUES (N'4/3/2025', 99225, 1004681, N'IV05', NULL, N'9-111-8019845-2302666', NULL)
INSERT [dbo].[xcuda_Attached_documents] ([Attached_document_date], [Attached_documents_Id], [Item_Id], [Attached_document_code], [Attached_document_name], [Attached_document_reference], [Attached_document_from_rule]) VALUES (N'4/3/2025', 99226, 1004681, N'BL07', NULL, N'HAWB9595459-Manifest', NULL)
INSERT [dbo].[xcuda_Attached_documents] ([Attached_document_date], [Attached_documents_Id], [Item_Id], [Attached_document_code], [Attached_document_name], [Attached_document_reference], [Attached_document_from_rule]) VALUES (N'4/3/2025', 99227, 1004682, N'IV05', NULL, N'11-111-8019845-2302666', NULL)
INSERT [dbo].[xcuda_Attached_documents] ([Attached_document_date], [Attached_documents_Id], [Item_Id], [Attached_document_code], [Attached_document_name], [Attached_document_reference], [Attached_document_from_rule]) VALUES (N'4/3/2025', 99228, 1004682, N'BL07', NULL, N'HAWB9596948-Manifest', NULL)
SET IDENTITY_INSERT [dbo].[xcuda_Attached_documents] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_xcuda_Attached_documents]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [IX_xcuda_Attached_documents] ON [dbo].[xcuda_Attached_documents]
(
	[Item_Id] ASC,
	[Attached_document_code] ASC,
	[Attached_document_reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Attached_documents_130_129]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Attached_documents_130_129] ON [dbo].[xcuda_Attached_documents]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Attached_documents_169_168]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Attached_documents_169_168] ON [dbo].[xcuda_Attached_documents]
(
	[Attached_document_code] ASC
)
INCLUDE([Item_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Attached_documents_2_1]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Attached_documents_2_1] ON [dbo].[xcuda_Attached_documents]
(
	[Attached_document_reference] ASC
)
INCLUDE([Item_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Attached_documents_4_3]    Script Date: 4/3/2025 10:23:55 PM ******/
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
