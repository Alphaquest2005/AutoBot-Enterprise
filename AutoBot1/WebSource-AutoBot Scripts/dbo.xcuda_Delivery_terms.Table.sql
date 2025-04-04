USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Delivery_terms]    Script Date: 4/3/2025 10:23:54 PM ******/
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
SET IDENTITY_INSERT [dbo].[xcuda_Delivery_terms] ON 

INSERT [dbo].[xcuda_Delivery_terms] ([Delivery_terms_Id], [Transport_Id], [Code], [Place], [Situation]) VALUES (38932, 42823, N'FOB', NULL, NULL)
INSERT [dbo].[xcuda_Delivery_terms] ([Delivery_terms_Id], [Transport_Id], [Code], [Place], [Situation]) VALUES (38933, 42824, N'FOB', NULL, NULL)
INSERT [dbo].[xcuda_Delivery_terms] ([Delivery_terms_Id], [Transport_Id], [Code], [Place], [Situation]) VALUES (38934, 42825, N'FOB', NULL, NULL)
SET IDENTITY_INSERT [dbo].[xcuda_Delivery_terms] OFF
GO
ALTER TABLE [dbo].[xcuda_Delivery_terms]  WITH NOCHECK ADD  CONSTRAINT [FK_Transport_Delivery_terms] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Delivery_terms] CHECK CONSTRAINT [FK_Transport_Delivery_terms]
GO
