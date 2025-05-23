USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[TarrifKeyWordCategory]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TarrifKeyWordCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[TariffCategoryCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TarrifKeyWordCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TarrifKeyWordCategory]  WITH CHECK ADD  CONSTRAINT [FK_TarrifKeyWordCategory_TariffCategory] FOREIGN KEY([TariffCategoryCode])
REFERENCES [dbo].[TariffCategory] ([TariffCategoryCode])
GO
ALTER TABLE [dbo].[TarrifKeyWordCategory] CHECK CONSTRAINT [FK_TarrifKeyWordCategory_TariffCategory]
GO
