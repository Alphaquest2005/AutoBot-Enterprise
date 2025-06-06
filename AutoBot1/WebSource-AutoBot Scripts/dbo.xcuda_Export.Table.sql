USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Export]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Export](
	[Export_country_code] [nvarchar](50) NULL,
	[Export_country_name] [nvarchar](50) NULL,
	[Country_Id] [int] NOT NULL,
	[Export_country_region] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Export_1] PRIMARY KEY CLUSTERED 
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Export]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Export_xcuda_Country] FOREIGN KEY([Country_Id])
REFERENCES [dbo].[xcuda_Country] ([Country_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Export] CHECK CONSTRAINT [FK_xcuda_Export_xcuda_Country]
GO
