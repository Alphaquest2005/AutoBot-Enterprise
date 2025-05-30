USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Destination]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Destination](
	[Destination_country_code] [nvarchar](max) NULL,
	[Destination_country_name] [nvarchar](max) NULL,
	[Country_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Destination_1] PRIMARY KEY CLUSTERED 
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Destination]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Destination_xcuda_Country] FOREIGN KEY([Country_Id])
REFERENCES [dbo].[xcuda_Country] ([Country_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Destination] CHECK CONSTRAINT [FK_xcuda_Destination_xcuda_Country]
GO
