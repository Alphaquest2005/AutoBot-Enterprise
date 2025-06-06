USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Warehouse]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Warehouse](
	[Identification] [nvarchar](20) NULL,
	[Delay] [int] NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[Warehouse_Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_xcuda_Warehouse] PRIMARY KEY CLUSTERED 
(
	[Warehouse_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Warehouse_494_493]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Warehouse_494_493] ON [dbo].[xcuda_Warehouse]
(
	[ASYCUDA_Id] ASC
)
INCLUDE([Delay]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Warehouse] ADD  CONSTRAINT [DF_xcuda_Warehouse_Delay]  DEFAULT ((730)) FOR [Delay]
GO
ALTER TABLE [dbo].[xcuda_Warehouse]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Warehouse] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Warehouse] CHECK CONSTRAINT [FK_ASYCUDA_Warehouse]
GO
