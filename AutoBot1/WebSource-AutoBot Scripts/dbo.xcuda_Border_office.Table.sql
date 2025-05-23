USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Border_office]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Border_office](
	[Border_office_Id] [int] IDENTITY(1,1) NOT NULL,
	[Transport_Id] [int] NULL,
	[Code] [nvarchar](10) NULL,
	[Name] [nvarchar](100) NULL,
 CONSTRAINT [PK_xcuda_Border_office] PRIMARY KEY CLUSTERED 
(
	[Border_office_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Border_office]  WITH NOCHECK ADD  CONSTRAINT [FK_Transport_Border_office] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Border_office] CHECK CONSTRAINT [FK_Transport_Border_office]
GO
