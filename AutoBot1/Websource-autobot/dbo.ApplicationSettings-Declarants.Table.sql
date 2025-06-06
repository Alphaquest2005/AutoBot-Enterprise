USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ApplicationSettings-Declarants]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationSettings-Declarants](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[DeclarantCode] [nvarchar](50) NOT NULL,
	[IsDefault] [bit] NOT NULL,
 CONSTRAINT [PK_ApplicationSettings-Declarants] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ApplicationSettings-Declarants] ON 

INSERT [dbo].[ApplicationSettings-Declarants] ([Id], [ApplicationSettingsId], [DeclarantCode], [IsDefault]) VALUES (2, 3, N'162319', 1)
SET IDENTITY_INSERT [dbo].[ApplicationSettings-Declarants] OFF
GO
ALTER TABLE [dbo].[ApplicationSettings-Declarants]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationSettings-Declarants_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ApplicationSettings-Declarants] CHECK CONSTRAINT [FK_ApplicationSettings-Declarants_ApplicationSettings]
GO
