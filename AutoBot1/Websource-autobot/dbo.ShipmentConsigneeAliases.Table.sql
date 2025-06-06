USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentConsigneeAliases]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentConsigneeAliases](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConsigneeId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ShipmentConsigneeAliases] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentConsigneeAliases]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentConsigneeAliases_ShipmentConsignee] FOREIGN KEY([ConsigneeId])
REFERENCES [dbo].[ShipmentConsignee] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentConsigneeAliases] CHECK CONSTRAINT [FK_ShipmentConsigneeAliases_ShipmentConsignee]
GO
