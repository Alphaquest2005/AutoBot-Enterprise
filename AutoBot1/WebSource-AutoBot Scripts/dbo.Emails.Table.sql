USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Emails]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Emails](
	[Subject] [nvarchar](max) NOT NULL,
	[EmailDate] [datetime] NOT NULL,
	[EmailId] [nvarchar](255) NOT NULL,
	[MachineName] [nvarchar](50) NULL,
	[EmailUniqueId] [int] NULL,
	[ApplicationSettingsId] [int] NULL,
 CONSTRAINT [PK_Emails_1] PRIMARY KEY CLUSTERED 
(
	[EmailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'sample shipment', CAST(N'2025-03-17T14:06:34.000' AS DateTime), N'sample shipment--2025-03-17-14:06:34', N'MINIJOE', 8, 3)
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Shipment: HAWB9595443', CAST(N'2025-04-03T07:13:53.000' AS DateTime), N'Shipment: HAWB9595443--2025-04-03-07:13:53', N'MINIJOE', 478, 3)
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Shipment: HAWB9595443', CAST(N'2025-04-03T21:34:29.000' AS DateTime), N'Shipment: HAWB9595443--2025-04-03-21:34:29', N'MINIJOE', 492, 3)
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Shipment: HAWB9595459', CAST(N'2025-04-03T21:34:32.000' AS DateTime), N'Shipment: HAWB9595459--2025-04-03-21:34:32', N'MINIJOE', 493, 3)
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Shipment: HAWB9596948', CAST(N'2025-04-03T21:34:36.000' AS DateTime), N'Shipment: HAWB9596948--2025-04-03-21:34:36', N'MINIJOE', 494, 3)
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Unknown PDF Found', CAST(N'2025-04-03T20:15:02.000' AS DateTime), N'Unknown PDF Found--2025-04-03-20:15:02', N'MINIJOE', 488, 3)
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Unknown PDF Found', CAST(N'2025-04-03T20:18:59.000' AS DateTime), N'Unknown PDF Found--2025-04-03-20:18:59', N'MINIJOE', 489, 3)
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'waybill sample', CAST(N'2025-03-16T20:05:21.000' AS DateTime), N'waybill sample--2025-03-16-20:05:21', N'MINIJOE', 6, 3)
GO
