USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Attachments]    Script Date: 3/27/2025 1:48:23 AM ******/
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

INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123333, N'D:\OneDrive\Clients\WebSource\Emails\Imports\Summary-Next Shipment-0.xlsx', N'NA', N'Summary', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123334, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03142025_7_24_24, 3_53 PM am^on.coiti‘\111-8019845-2302666.pdf', N'IV05', N'111-8019845-2302666', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123335, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03142025_7_24_24, 3_53 PM am^on.coiti‘\111-8019845-2302666.xlsx', N'NA', N'111-8019845-2302666', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123336, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03142025_7_24_24, 3_53 PM am^on.coiti‘\HAWB9595443-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123337, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03142025_7_24_24, 3_53 PM am^on.coiti‘\HAWB9595459-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123338, N'D:\OneDrive\Clients\WebSource\Emails\Documents\03142025_7_24_24, 3_53 PM am^on.coiti‘\HAWB9596948-Manifest.pdf', N'IV04', N'2024 28', NULL)
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123339, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\427\HAWB9595443-Manifest.pdf', N'BL07', N'HAWB9595443-Manifest', N'Shipment: HAWB9595443--2025-03-26-20:00:34')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123340, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\427\111-8019845-2302666.pdf', N'IV05', N'3-111-8019845-2302666', N'Shipment: HAWB9595443--2025-03-26-20:00:34')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123341, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\427\111-8019845-2302666.xlsx', N'NA', N'4-111-8019845-2302666', N'Shipment: HAWB9595443--2025-03-26-20:00:34')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123342, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\427\Info.txt', N'NA', N'Info', N'Shipment: HAWB9595443--2025-03-26-20:00:34')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123343, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\429\HAWB9595459-Manifest.pdf', N'BL07', N'HAWB9595459-Manifest', N'Shipment: HAWB9595459--2025-03-26-20:05:53')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123344, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\429\111-8019845-2302666.pdf', N'IV05', N'5-111-8019845-2302666', N'Shipment: HAWB9595459--2025-03-26-20:05:53')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123345, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\429\111-8019845-2302666.xlsx', N'NA', N'6-111-8019845-2302666', N'Shipment: HAWB9595459--2025-03-26-20:05:53')
INSERT [dbo].[Attachments] ([Id], [FilePath], [DocumentCode], [Reference], [EmailId]) VALUES (123346, N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\429\Info.txt', N'NA', N'2-Info', N'Shipment: HAWB9595459--2025-03-26-20:05:53')
SET IDENTITY_INSERT [dbo].[Attachments] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_Attachments_13_12]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_Attachments_13_12] ON [dbo].[Attachments]
(
	[FilePath] ASC,
	[DocumentCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_Attachments_466_465]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_Attachments_466_465] ON [dbo].[Attachments]
(
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_Attachments_482_481]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_Attachments_482_481] ON [dbo].[Attachments]
(
	[DocumentCode] ASC,
	[Reference] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
