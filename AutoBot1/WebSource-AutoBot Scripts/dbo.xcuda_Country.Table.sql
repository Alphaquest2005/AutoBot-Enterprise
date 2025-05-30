USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Country]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Country](
	[Country_first_destination] [nvarchar](255) NULL,
	[Country_of_origin_name] [nvarchar](255) NULL,
	[Country_Id] [int] NOT NULL,
	[Place_of_loading_Id] [int] NULL,
	[Trading_country] [nvarchar](255) NULL,
 CONSTRAINT [PK_xcuda_Country] PRIMARY KEY CLUSTERED 
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Country]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Country_xcuda_General_information] FOREIGN KEY([Country_Id])
REFERENCES [dbo].[xcuda_General_information] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Country] CHECK CONSTRAINT [FK_xcuda_Country_xcuda_General_information]
GO
