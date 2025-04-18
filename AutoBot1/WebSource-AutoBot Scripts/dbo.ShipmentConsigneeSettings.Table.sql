USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentConsigneeSettings]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentConsigneeSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConsigneeId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ShipmentConsigneeSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentConsigneeSettings]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentConsigneeSettings_ShipmentConsignee] FOREIGN KEY([ConsigneeId])
REFERENCES [dbo].[ShipmentConsignee] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentConsigneeSettings] CHECK CONSTRAINT [FK_ShipmentConsigneeSettings_ShipmentConsignee]
GO
