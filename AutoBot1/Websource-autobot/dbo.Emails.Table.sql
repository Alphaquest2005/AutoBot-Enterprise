USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Emails]    Script Date: 3/27/2025 1:48:23 AM ******/
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
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Shipment: HAWB9595443', CAST(N'2025-03-26T20:00:34.000' AS DateTime), N'Shipment: HAWB9595443--2025-03-26-20:00:34', N'MINIJOE', 427, 3)
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Shipment: HAWB9595459', CAST(N'2025-03-26T20:05:53.000' AS DateTime), N'Shipment: HAWB9595459--2025-03-26-20:05:53', N'MINIJOE', 429, 3)
GO
