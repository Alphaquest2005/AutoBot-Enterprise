USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[CancelledEntriesLst]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CancelledEntriesLst](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Office] [nvarchar](50) NOT NULL,
	[RegistrationNumber] [nvarchar](8) NOT NULL,
	[RegistrationDate] [nvarchar](50) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_CancelledEntriesLst] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[CancelledEntriesLst] ON 

INSERT [dbo].[CancelledEntriesLst] ([Id], [Office], [RegistrationNumber], [RegistrationDate], [ApplicationSettingsId]) VALUES (1, N'GDFDX', N'3285', N'2021-05-27', 2)
INSERT [dbo].[CancelledEntriesLst] ([Id], [Office], [RegistrationNumber], [RegistrationDate], [ApplicationSettingsId]) VALUES (2, N'GDSGO', N'11394', N'2021-03-19', 2)
INSERT [dbo].[CancelledEntriesLst] ([Id], [Office], [RegistrationNumber], [RegistrationDate], [ApplicationSettingsId]) VALUES (3, N'GDSGO', N'1608', N'2021-01-09', 2)
INSERT [dbo].[CancelledEntriesLst] ([Id], [Office], [RegistrationNumber], [RegistrationDate], [ApplicationSettingsId]) VALUES (4, N'GDSGO', N'36383', N'10/07/2020', 2)
INSERT [dbo].[CancelledEntriesLst] ([Id], [Office], [RegistrationNumber], [RegistrationDate], [ApplicationSettingsId]) VALUES (7, N'GDSGO', N'11134', N'03/10/2019', 2)
SET IDENTITY_INSERT [dbo].[CancelledEntriesLst] OFF
GO
