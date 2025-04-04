USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Transport]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Transport](
	[Container_flag] [bit] NOT NULL,
	[Single_waybill_flag] [bit] NOT NULL,
	[Transport_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[Location_of_goods] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Transport] PRIMARY KEY CLUSTERED 
(
	[Transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[xcuda_Transport] ON 

INSERT [dbo].[xcuda_Transport] ([Container_flag], [Single_waybill_flag], [Transport_Id], [ASYCUDA_Id], [Location_of_goods]) VALUES (0, 0, 42823, 48427, NULL)
INSERT [dbo].[xcuda_Transport] ([Container_flag], [Single_waybill_flag], [Transport_Id], [ASYCUDA_Id], [Location_of_goods]) VALUES (0, 0, 42824, 48428, NULL)
INSERT [dbo].[xcuda_Transport] ([Container_flag], [Single_waybill_flag], [Transport_Id], [ASYCUDA_Id], [Location_of_goods]) VALUES (0, 0, 42825, 48429, NULL)
SET IDENTITY_INSERT [dbo].[xcuda_Transport] OFF
GO
ALTER TABLE [dbo].[xcuda_Transport]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Transport] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Transport] CHECK CONSTRAINT [FK_ASYCUDA_Transport]
GO
