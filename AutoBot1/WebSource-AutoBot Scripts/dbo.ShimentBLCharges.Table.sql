USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShimentBLCharges]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShimentBLCharges](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BLId] [int] NOT NULL,
	[Description] [nvarchar](255) NOT NULL,
	[Amount] [float] NOT NULL,
	[Currency] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ShimentBLCharges] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShimentBLCharges]  WITH CHECK ADD  CONSTRAINT [FK_ShimentBLCharges_ShipmentBL1] FOREIGN KEY([BLId])
REFERENCES [dbo].[ShipmentBL] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShimentBLCharges] CHECK CONSTRAINT [FK_ShimentBLCharges_ShipmentBL1]
GO
