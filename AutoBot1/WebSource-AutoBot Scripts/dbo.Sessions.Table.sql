USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Sessions]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sessions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[WindowInMinutes] [int] NOT NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Sessions] ON 

INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (1, N'Discrepancies', 10)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (2, N'Exwarehouse', 10)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (3, N'CleanUp', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (4, N'DiscrepanciesWithoutAllocations', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (7, N'AutoMatch', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (8, N'AssessDiscrepancies', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (9, N'DownloadAsycuda', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (10, N'Allocations', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (11, N'Submit Discrepanies to Customs', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (12, N'Ex without Allocations', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (13, N'AssessExwarehouse', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (14, N'DowloadPDFs', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (15, N'SubmitSalesToCustoms', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (16, N'ImportErrors', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (17, N'ReportErrors', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (18, N'ImportC71&License', 8)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (19, N'CreateC71&Lic', 8)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (20, N'AssessIM7', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (21, N'AssessEX', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (22, N'Re-Ex-Warehouse', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (23, N'TestInvoiceReader', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (24, N'Utilities', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (26, N'DiscrepanciesWithoutClearingAllocations', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (27, N'EOMEX&DIS', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (29, N'reEOMEX&DIS', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (30, N'Adjustments', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (31, N'ReGenPOEntries', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (32, N'C71', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (33, N'ReAssessIM7', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (34, N'LIC', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (35, N'ReImport', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (36, N'Container', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (39, N'Download&Submit Discrepancies', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (40, N'Download&SubmitPO', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (41, N'Dowload&SubmitSales', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (42, N'ExpiredEntries', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (43, N'Redownload Entries', 5)
INSERT [dbo].[Sessions] ([Id], [Name], [WindowInMinutes]) VALUES (45, N'Cancelled Entries', 5)
SET IDENTITY_INSERT [dbo].[Sessions] OFF
GO
