USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Suppliers_link]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Suppliers_link](
	[Suppliers_link_code] [nvarchar](100) NULL,
	[Item_Id] [int] NULL,
	[Suppliers_link_Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_xcuda_Suppliers_link] PRIMARY KEY CLUSTERED 
(
	[Suppliers_link_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Suppliers_link_470_469]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Suppliers_link_470_469] ON [dbo].[xcuda_Suppliers_link]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Suppliers_link]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Suppliers_link] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Suppliers_link] CHECK CONSTRAINT [FK_Item_Suppliers_link]
GO
