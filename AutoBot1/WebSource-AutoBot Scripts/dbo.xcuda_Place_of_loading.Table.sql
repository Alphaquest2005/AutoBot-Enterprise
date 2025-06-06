USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Place_of_loading]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Place_of_loading](
	[Place_of_loading_Id] [int] IDENTITY(1,1) NOT NULL,
	[Transport_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Place_of_loading] PRIMARY KEY CLUSTERED 
(
	[Place_of_loading_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Place_of_loading]  WITH NOCHECK ADD  CONSTRAINT [FK_Transport_Place_of_loading] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Place_of_loading] CHECK CONSTRAINT [FK_Transport_Place_of_loading]
GO
