USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[FileTypes-FileImporterInfo]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileTypes-FileImporterInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryType] [nvarchar](50) NOT NULL,
	[Format] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_FileTypes-FileInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FileTypes-FileImporterInfo] ON 

INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (1, N'XCUDA', N'XML')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (2, N'EX9', N'XML')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (3, N'LIC', N'XML')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (4, N'C71', N'XML')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (5, N'Sales', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (6, N'PO', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (7, N'DIS', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (8, N'ADJ', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (9, N'Tariff', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (10, N'Supplier', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (11, N'OPS', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (12, N'DiscrepancyExecution', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (13, N'ExpiredEntries', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (14, N'Unknown', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (15, N'Rider', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (16, N'CancelledEntries', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (17, N'INV', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (18, N'BL', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (19, N'PDF', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (20, N'Freight', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (21, N'Manifest', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (22, N'C14', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (23, N'PrevDoc', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (24, N'Unknown', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (25, N'Shipment Invoice', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (26, N'Unknown', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (27, N'Shipment Summary', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (28, N'Info', N'TXT')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (29, N'POTemplate', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (30, N'Shipment Invoice', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (32, N'Rider', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (33, N'Rider', N'PDF')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (34, N'Sales', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (35, N'ADJ', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (36, N'PO', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (37, N'DIS', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (38, N'OPS', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (39, N'Info', N'TXT')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (40, N'xSales', N'CSV')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (41, N'Rider', N'XLSX')
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (42, N'Simplified Declaration', N'PDF')
SET IDENTITY_INSERT [dbo].[FileTypes-FileImporterInfo] OFF
GO
