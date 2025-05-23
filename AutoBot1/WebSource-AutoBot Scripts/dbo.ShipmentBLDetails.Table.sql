USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentBLDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentBLDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BLId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[PackageType] [nvarchar](50) NOT NULL,
	[Marks] [nvarchar](50) NOT NULL,
	[Comments] [nvarchar](1000) NULL,
	[GrossWeightKg] [float] NULL,
	[CubicFeet] [float] NULL,
 CONSTRAINT [PK_ShipmentBLDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentBLDetails]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentBLDetails_ShipmentBL1] FOREIGN KEY([BLId])
REFERENCES [dbo].[ShipmentBL] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentBLDetails] CHECK CONSTRAINT [FK_ShipmentBLDetails_ShipmentBL1]
GO
