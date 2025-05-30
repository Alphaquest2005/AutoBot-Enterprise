USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Container]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Container](
	[Item_Number] [nvarchar](255) NULL,
	[Container_identity] [nvarchar](255) NULL,
	[Container_type] [nvarchar](255) NULL,
	[Empty_full_indicator] [nvarchar](255) NULL,
	[Gross_weight] [float] NOT NULL,
	[Goods_description] [nvarchar](255) NULL,
	[Packages_type] [nvarchar](255) NULL,
	[Packages_number] [nvarchar](255) NULL,
	[Packages_weight] [float] NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[Container_Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_xcuda_Container] PRIMARY KEY CLUSTERED 
(
	[Container_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Container]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Container] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Container] CHECK CONSTRAINT [FK_ASYCUDA_Container]
GO
