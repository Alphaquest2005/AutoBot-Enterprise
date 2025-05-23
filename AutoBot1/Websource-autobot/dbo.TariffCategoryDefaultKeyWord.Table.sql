USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[TariffCategoryDefaultKeyWord]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TariffCategoryDefaultKeyWord](
	[CategoryId] [int] NOT NULL,
	[TariffCode] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_TariffCategoryDefaultKeyWord] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC,
	[TariffCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
