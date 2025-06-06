USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xLIC_License_Registered]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xLIC_License_Registered](
	[LicenseId] [int] NOT NULL,
	[RegistrationNumber] [nvarchar](50) NOT NULL,
	[SourceFile] [nvarchar](300) NOT NULL,
	[DocumentReference] [nvarchar](50) NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_xLIC_RegisteredLicense] PRIMARY KEY CLUSTERED 
(
	[LicenseId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xLIC_License_Registered] ([LicenseId], [RegistrationNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (322, N'3910', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\LIC\3910-LIC.xml', N'Tobago5', 3)
INSERT [dbo].[xLIC_License_Registered] ([LicenseId], [RegistrationNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (323, N'3917', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\LIC\3917-LIC.xml', N'Tobago9', 3)
INSERT [dbo].[xLIC_License_Registered] ([LicenseId], [RegistrationNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (324, N'3918', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\LIC\3918-LIC.xml', N'Tobago9', 3)
INSERT [dbo].[xLIC_License_Registered] ([LicenseId], [RegistrationNumber], [SourceFile], [DocumentReference], [ApplicationSettingsId]) VALUES (1532, N'6216', N'C:\Users\josep\OneDrive\Clients\Portage\Emails\Imports\LIC\6216-LIC.xml', N'8526649', 3)
GO
ALTER TABLE [dbo].[xLIC_License_Registered]  WITH CHECK ADD  CONSTRAINT [FK_xLIC_License_Registered_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xLIC_License_Registered] CHECK CONSTRAINT [FK_xLIC_License_Registered_ApplicationSettings]
GO
ALTER TABLE [dbo].[xLIC_License_Registered]  WITH CHECK ADD  CONSTRAINT [FK_xLIC_RegisteredLicense_xLIC_License] FOREIGN KEY([LicenseId])
REFERENCES [dbo].[xLIC_License] ([LicenseId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xLIC_License_Registered] CHECK CONSTRAINT [FK_xLIC_RegisteredLicense_xLIC_License]
GO
