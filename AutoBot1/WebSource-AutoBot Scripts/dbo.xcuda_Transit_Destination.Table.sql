USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Transit_Destination]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Transit_Destination](
	[Destination_Id] [int] IDENTITY(1,1) NOT NULL,
	[Office] [nvarchar](10) NULL,
	[Country] [nvarchar](10) NULL,
	[Transit_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Transit_Destination] PRIMARY KEY CLUSTERED 
(
	[Destination_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Transit_Destination]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Transit_Destination_xcuda_Transit] FOREIGN KEY([Transit_Id])
REFERENCES [dbo].[xcuda_Transit] ([Transit_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Transit_Destination] CHECK CONSTRAINT [FK_xcuda_Transit_Destination_xcuda_Transit]
GO
