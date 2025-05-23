USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Contacts]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
	[EmailAddress] [varchar](255) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[CellPhone] [nvarchar](10) NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Contacts] ON 

INSERT [dbo].[Contacts] ([Id], [Role], [EmailAddress], [Name], [ApplicationSettingsId], [CellPhone]) VALUES (14, N'PO Clerk', N'WebSource@auto-brokerage.com', N'Joseph', 3, N'405-8243')
INSERT [dbo].[Contacts] ([Id], [Role], [EmailAddress], [Name], [ApplicationSettingsId], [CellPhone]) VALUES (15, N'Broker', N'WebSource@auto-brokerage.com', N'Joseph', 3, N'405-8243')
INSERT [dbo].[Contacts] ([Id], [Role], [EmailAddress], [Name], [ApplicationSettingsId], [CellPhone]) VALUES (16, N'PDF Entries', N'WebSource@auto-brokerage.com', N'Joseph', 3, N'405-8243')
INSERT [dbo].[Contacts] ([Id], [Role], [EmailAddress], [Name], [ApplicationSettingsId], [CellPhone]) VALUES (17, N'Clerk', N'WebSource@auto-brokerage.com', N'Joseph', 3, NULL)
INSERT [dbo].[Contacts] ([Id], [Role], [EmailAddress], [Name], [ApplicationSettingsId], [CellPhone]) VALUES (18, N'Billing', N'WebSource@auto-brokerage.com', N'Joseph', 3, NULL)
INSERT [dbo].[Contacts] ([Id], [Role], [EmailAddress], [Name], [ApplicationSettingsId], [CellPhone]) VALUES (19, N'Developer', N'WebSource@auto-brokerage.com', N'Joseph', 3, N'405-8243')
SET IDENTITY_INSERT [dbo].[Contacts] OFF
GO
