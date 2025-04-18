USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Emails]    Script Date: 4/8/2025 8:33:17 AM ******/
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
INSERT [dbo].[Emails] ([Subject], [EmailDate], [EmailId], [MachineName], [EmailUniqueId], [ApplicationSettingsId]) VALUES (N'Fw: Invoices * BOL', CAST(N'2025-04-07T13:16:27.000' AS DateTime), N'Fw: Invoices * BOL--2025-04-07-13:16:27', N'MINIJOE', 5, 3)
GO
