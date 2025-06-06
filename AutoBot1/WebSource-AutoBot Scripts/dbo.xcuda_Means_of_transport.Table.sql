USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Means_of_transport]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Means_of_transport](
	[Means_of_transport_Id] [int] IDENTITY(1,1) NOT NULL,
	[Transport_Id] [int] NULL,
	[Inland_mode_of_transport] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Means_of_transport] PRIMARY KEY CLUSTERED 
(
	[Means_of_transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Means_of_transport]  WITH NOCHECK ADD  CONSTRAINT [FK_Transport_Means_of_transport] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Means_of_transport] CHECK CONSTRAINT [FK_Transport_Means_of_transport]
GO
