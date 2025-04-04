USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Attachments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FilePath] [nvarchar](255) NOT NULL,
	[DocumentCode] [nvarchar](50) NULL,
	[Reference] [nvarchar](255) NULL,
	[EmailId] [nvarchar](255) NULL,
 CONSTRAINT [PK_Attachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Attachments] ON 

INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123474, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\6\Mackess Cox Waybill.pdf', N'NA', N'2-Mackess Cox Waybill', N'waybill sample--2025-03-16-20:05:21')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123475, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\6\Info.txt', N'NA', N'3-Info', N'waybill sample--2025-03-16-20:05:21')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123476, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\03142025-7-24-24- 3-53 PM am-on-coiti-.pdf', N'NA', N'2-03142025-7-24-24- 3-53 PM am-on-coiti-', N'sample shipment--2025-03-17-14:06:34')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123477, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\Info.txt', N'NA', N'6-Info', N'sample shipment--2025-03-17-14:06:34')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123478, N'D:\OneDrive\Clients\WebSource\Emails\Imports\Summary-Next Shipment-0.xlsx', N'NA', N'Summary', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123479, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\111-8019845-2302666.pdf', N'IV05', N'111-8019845-2302666', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123480, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\111-8019845-2302666.xlsx', N'NA', N'111-8019845-2302666', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123481, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\HAWB9595443-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123482, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\HAWB9595459-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123483, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\HAWB9596948-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123484, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\478\HAWB9595443-Manifest.pdf', N'BL07', N'2-HAWB9595443-Manifest', N'Shipment: HAWB9595443--2025-04-03-07:13:53')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123485, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\478\111-8019845-2302666.pdf', N'IV05', N'5-111-8019845-2302666', N'Shipment: HAWB9595443--2025-04-03-07:13:53')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123486, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\478\111-8019845-2302666.xlsx', N'NA', N'5-111-8019845-2302666', N'Shipment: HAWB9595443--2025-04-03-07:13:53')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123487, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\478\Info.txt', N'NA', N'4-Info', N'Shipment: HAWB9595443--2025-04-03-07:13:53')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123489, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\488\Info.txt', N'NA', N'4-Info', N'Unknown PDF Found--2025-04-03-20:15:02')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123490, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\489\Info.txt', N'NA', N'5-Info', N'Unknown PDF Found--2025-04-03-20:18:59')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123491, N'D:\OneDrive\Clients\WebSource\Emails\Imports\Summary-Next Shipment-0.xlsx', N'NA', N'Summary', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123492, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\111-8019845-2302666.pdf', N'IV05', N'111-8019845-2302666', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123493, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\111-8019845-2302666.xlsx', N'NA', N'111-8019845-2302666', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123494, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\HAWB9595443-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123495, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\HAWB9595459-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123496, N'D:\OneDrive\Clients\WebSource\Emails\Shipments\8\HAWB9596948-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123497, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\HAWB9595443-Manifest.pdf', N'BL07', N'2-HAWB9595443-Manifest', N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123498, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\111-8019845-2302666.pdf', N'IV05', N'7-111-8019845-2302666', N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123499, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\111-8019845-2302666.xlsx', N'NA', N'8-111-8019845-2302666', N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123500, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\Info.txt', N'NA', N'6-Info', N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123501, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\HAWB9595459-Manifest.pdf', N'BL07', N'HAWB9595459-Manifest', N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123502, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\111-8019845-2302666.pdf', N'IV05', N'9-111-8019845-2302666', N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123503, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\111-8019845-2302666.xlsx', N'NA', N'10-111-8019845-2302666', N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123504, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\Info.txt', N'NA', N'7-Info', N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123505, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\HAWB9596948-Manifest.pdf', N'BL07', N'HAWB9596948-Manifest', N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123506, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\111-8019845-2302666.pdf', N'IV05', N'11-111-8019845-2302666', N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123507, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\111-8019845-2302666.xlsx', N'NA', N'12-111-8019845-2302666', N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123508, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\Info.txt', N'NA', N'8-Info', N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123509, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\C71.xml', N'DC05', N'C71', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123510, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\HAWB9595443-Manifest.pdf', N'BL07', N'2-HAWB9595443-Manifest', N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123511, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\HAWB9595443-Manifest.pdf', N'BL07', N'2-HAWB9595443-Manifest', N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123512, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\HAWB9595443-Manifest.pdf', N'BL07', N'2-HAWB9595443-Manifest', N'Shipment: HAWB9595443--2025-04-03-21:34:29')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123513, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\C71.xml', N'DC05', N'C71', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123514, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\HAWB9595459-Manifest.pdf', N'BL07', N'HAWB9595459-Manifest', N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123515, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\HAWB9595459-Manifest.pdf', N'BL07', N'HAWB9595459-Manifest', N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123516, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\HAWB9595459-Manifest.pdf', N'BL07', N'HAWB9595459-Manifest', N'Shipment: HAWB9595459--2025-04-03-21:34:32')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123517, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\C71.xml', N'DC05', N'C71', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123518, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\HAWB9596948-Manifest.pdf', N'BL07', N'HAWB9596948-Manifest', N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123519, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\HAWB9596948-Manifest.pdf', N'BL07', N'HAWB9596948-Manifest', N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123520, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\HAWB9596948-Manifest.pdf', N'BL07', N'HAWB9596948-Manifest', N'Shipment: HAWB9596948--2025-04-03-21:34:36')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123521, N'D:\OneDrive\Clients\WebSource\Emails\Imports\Summary-Next Shipment-0.xlsx', N'NA', N'Summary', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123522, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03152025135001\111-7220046-1575437.pdf', N'IV05', N'111-7220046-1575437', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123523, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03152025135001\111-7220046-1575437.xlsx', N'NA', N'111-7220046-1575437', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123524, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03152025135001\HAWB39605575-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123525, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03152025135001\HAWB9597711-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123526, N'D:\OneDrive\Clients\WebSource\Emails\Imports\Summary-Next Shipment-0.xlsx', N'NA', N'Summary', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123527, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03152025_7_29_24, 3_57 PM amazon.com‘\111-9341313-9838630.pdf', N'IV05', N'111-9341313-9838630', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123528, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03152025_7_29_24, 3_57 PM amazon.com‘\111-9341313-9838630.xlsx', N'NA', N'111-9341313-9838630', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123529, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03152025_7_29_24, 3_57 PM amazon.com‘\HAWB9591107-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123530, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03152025_Payment information\03152025_Payment information.pdf', NULL, NULL, N'03152025_Payment information.pdf')
SET IDENTITY_INSERT [dbo].[Attachments] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_Attachments_13_12]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_Attachments_13_12] ON [dbo].[Attachments]
(
	[FilePath] ASC,
	[DocumentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_Attachments_466_465]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_Attachments_466_465] ON [dbo].[Attachments]
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_Attachments_482_481]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_Attachments_482_481] ON [dbo].[Attachments]
(
	[DocumentCode] ASC,
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
