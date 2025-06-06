USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[TariffKeyWords]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TariffKeyWords](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TariffCode] [nvarchar](50) NOT NULL,
	[Keyword] [nvarchar](50) NOT NULL,
	[IsException] [bit] NOT NULL,
	[Weight] [int] NULL,
	[ApplicationSettingsId] [int] NULL,
 CONSTRAINT [PK_TariffKeyWords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TariffKeyWords]  WITH CHECK ADD  CONSTRAINT [FK_TariffKeyWords_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TariffKeyWords] CHECK CONSTRAINT [FK_TariffKeyWords_ApplicationSettings]
GO
ALTER TABLE [dbo].[TariffKeyWords]  WITH CHECK ADD  CONSTRAINT [FK_TariffKeyWords_TariffCodes] FOREIGN KEY([TariffCode])
REFERENCES [dbo].[TariffCodes] ([TariffCode])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TariffKeyWords] CHECK CONSTRAINT [FK_TariffKeyWords_TariffCodes]
GO
