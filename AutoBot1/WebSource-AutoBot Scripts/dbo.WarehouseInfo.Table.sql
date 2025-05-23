USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[WarehouseInfo]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WarehouseInfo](
	[WarehouseNo] [nvarchar](50) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Packages] [int] NOT NULL,
 CONSTRAINT [PK_WarehouseInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[WarehouseInfo]  WITH CHECK ADD  CONSTRAINT [FK_WarehouseInfo_EntryData_PurchaseOrders] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData_PurchaseOrders] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[WarehouseInfo] CHECK CONSTRAINT [FK_WarehouseInfo_EntryData_PurchaseOrders]
GO
