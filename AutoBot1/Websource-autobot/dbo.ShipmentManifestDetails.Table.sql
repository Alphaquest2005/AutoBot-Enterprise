USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentManifestDetails]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentManifestDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ManifestId] [int] NOT NULL,
	[ContainerID] [nvarchar](50) NULL,
	[Description] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_ShipmentManifestDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentManifestDetails]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentManifestDetails_ShipmentManifest] FOREIGN KEY([ManifestId])
REFERENCES [dbo].[ShipmentManifest] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentManifestDetails] CHECK CONSTRAINT [FK_ShipmentManifestDetails_ShipmentManifest]
GO
