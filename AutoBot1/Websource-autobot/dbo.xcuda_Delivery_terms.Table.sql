USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Delivery_terms]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Delivery_terms](
	[Delivery_terms_Id] [int] IDENTITY(1,1) NOT NULL,
	[Transport_Id] [int] NULL,
	[Code] [nvarchar](50) NULL,
	[Place] [nvarchar](50) NULL,
	[Situation] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Delivery_terms] PRIMARY KEY CLUSTERED 
(
	[Delivery_terms_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Delivery_terms]  WITH NOCHECK ADD  CONSTRAINT [FK_Transport_Delivery_terms] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Delivery_terms] CHECK CONSTRAINT [FK_Transport_Delivery_terms]
GO
