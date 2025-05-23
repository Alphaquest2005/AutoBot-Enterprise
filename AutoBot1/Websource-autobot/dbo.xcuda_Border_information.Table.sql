USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Border_information]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Border_information](
	[Border_information_Id] [int] IDENTITY(1,1) NOT NULL,
	[Means_of_transport_Id] [int] NULL,
	[Identity] [nvarchar](100) NULL,
	[Nationality] [nvarchar](100) NULL,
	[Mode] [int] NULL,
 CONSTRAINT [PK_xcuda_Border_information] PRIMARY KEY CLUSTERED 
(
	[Border_information_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Border_information]  WITH NOCHECK ADD  CONSTRAINT [FK_Means_of_transport_Border_information] FOREIGN KEY([Means_of_transport_Id])
REFERENCES [dbo].[xcuda_Means_of_transport] ([Means_of_transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Border_information] CHECK CONSTRAINT [FK_Means_of_transport_Border_information]
GO
