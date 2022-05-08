--USE [AutoBot-EnterpriseDB]
GO
/****** Object:  Table [dbo].[FileFormats]    Script Date: 4/21/2022 2:38:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--CREATE TABLE [dbo].[FileFormats](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[Name] [nvarchar](50) NOT NULL,
-- CONSTRAINT [PK_FileFormats] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--GO
SET IDENTITY_INSERT [dbo].[FileFormats] ON 
GO
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (1, N'XML')
GO
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (2, N'CSV')
GO
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (3, N'PDF')
GO
INSERT [dbo].[FileFormats] ([Id], [Name]) VALUES (4, N'XLSX')
GO
SET IDENTITY_INSERT [dbo].[FileFormats] OFF
GO


--GO
--CREATE TABLE [dbo].[FileTypes-FileImporterInfo](
--	[Id] [int] IDENTITY(1,1) NOT NULL,
--	[EntryType] [nvarchar](50) NOT NULL,
--	[Format] [nvarchar](50) NOT NULL,
-- CONSTRAINT [PK_FileTypes-FileImporterInfo] PRIMARY KEY CLUSTERED 
--(
--	[Id] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]
--GO
SET IDENTITY_INSERT [dbo].[FileTypes-FileImporterInfo] ON 
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (1, N'XCUDA', N'XML')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (2, N'EX9', N'XML')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (3, N'LIC', N'XML')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (4, N'C71', N'XML')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (5, N'Sales', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (6, N'PO', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (7, N'DIS', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (8, N'ADJ', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (9, N'Tariff', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (10, N'Supplier', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (11, N'OPS', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (12, N'DiscrepancyExecution', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (13, N'ExpiredEntries', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (14, N'Unknown', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (15, N'Rider', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (16, N'CancelledEntries', N'CSV')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (17, N'INV', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (18, N'BL', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (19, N'PDF', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (20, N'Freight', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (21, N'Manifest', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (22, N'C14', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (23, N'PrevDoc', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (24, N'Unknown', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (25, N'Shipment Invoice', N'PDF')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (26, N'XLSX', N'XLSX')
GO
INSERT [dbo].[FileTypes-FileImporterInfo] ([Id], [EntryType], [Format]) VALUES (27, N'Shipment Summary', N'XLSX')
GO
SET IDENTITY_INSERT [dbo].[FileTypes-FileImporterInfo] OFF
GO


update filetypes
                set FileInfoId = [FileTypes-FileImporterInfo].id
--SELECT [FileTypes-FileImporterInfo].Id, FileFormats.Id AS Expr1, FileTypes.FilePattern, FileTypes.Type, FileFormats.Name
FROM    FileTypes INNER JOIN
                 FileFormats ON FileTypes.FilePattern LIKE '%' + FileFormats.Name + '%' INNER JOIN
                 [FileTypes-FileImporterInfo] ON FileFormats.Name = [FileTypes-FileImporterInfo].Format AND FileTypes.Type = [FileTypes-FileImporterInfo].EntryType


