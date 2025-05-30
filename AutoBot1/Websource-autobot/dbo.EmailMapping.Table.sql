USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmailMapping]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[Pattern] [nvarchar](max) NOT NULL,
	[IsSingleEmail] [bit] NULL,
	[ReplacementValue] [nvarchar](50) NULL,
	[InfoFirst] [bit] NULL,
 CONSTRAINT [PK_EmailMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[EmailMapping] ON 

INSERT [dbo].[EmailMapping] ([Id], [ApplicationSettingsId], [Pattern], [IsSingleEmail], [ReplacementValue], [InfoFirst]) VALUES (22, 3, N'^Shipment:\s(?<Subject>.+)', NULL, NULL, NULL)
INSERT [dbo].[EmailMapping] ([Id], [ApplicationSettingsId], [Pattern], [IsSingleEmail], [ReplacementValue], [InfoFirst]) VALUES (27, 3, N'.*Error:\s?(?<Subject>.+)', NULL, NULL, NULL)
INSERT [dbo].[EmailMapping] ([Id], [ApplicationSettingsId], [Pattern], [IsSingleEmail], [ReplacementValue], [InfoFirst]) VALUES (36, 3, N'Submit Entries for: (?<Subject>.*)', NULL, NULL, NULL)
INSERT [dbo].[EmailMapping] ([Id], [ApplicationSettingsId], [Pattern], [IsSingleEmail], [ReplacementValue], [InfoFirst]) VALUES (37, 3, N'Delete PO from: (?<Subject>.*)', NULL, NULL, NULL)
INSERT [dbo].[EmailMapping] ([Id], [ApplicationSettingsId], [Pattern], [IsSingleEmail], [ReplacementValue], [InfoFirst]) VALUES (39, 3, N'^(?<Subject>((?!(CSVs for)|(Error:)|(Submit)|(Invoice Template)|(^Shipment:)).)*$)', NULL, N'Shipments', NULL)
INSERT [dbo].[EmailMapping] ([Id], [ApplicationSettingsId], [Pattern], [IsSingleEmail], [ReplacementValue], [InfoFirst]) VALUES (43, 3, N'.*(?<Subject>Invoice Template).*', NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[EmailMapping] OFF
GO
ALTER TABLE [dbo].[EmailMapping]  WITH CHECK ADD  CONSTRAINT [FK_EmailMapping_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailMapping] CHECK CONSTRAINT [FK_EmailMapping_ApplicationSettings]
GO
