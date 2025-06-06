USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentBL]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentBL](
	[BLNumber] [nvarchar](50) NOT NULL,
	[Vessel] [nvarchar](50) NULL,
	[Voyage] [nvarchar](50) NULL,
	[Container] [nvarchar](50) NULL,
	[Seals] [nvarchar](50) NULL,
	[Type] [nvarchar](50) NULL,
	[PackagesNo] [int] NOT NULL,
	[PackagesType] [nvarchar](50) NOT NULL,
	[WeightKG] [float] NOT NULL,
	[VolumeM3] [float] NOT NULL,
	[WeightLB] [float] NULL,
	[VolumeCF] [float] NULL,
	[Reference] [nvarchar](50) NULL,
	[EmailId] [nvarchar](255) NULL,
	[SourceFile] [nvarchar](max) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[FileTypeId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Freight] [float] NULL,
	[FreightCurrency] [nvarchar](10) NULL,
	[ConsigneeName] [nvarchar](100) NULL,
 CONSTRAINT [PK_ShipmentBL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentBL]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentBL_Consignees] FOREIGN KEY([ConsigneeName])
REFERENCES [dbo].[Consignees] ([ConsigneeName])
GO
ALTER TABLE [dbo].[ShipmentBL] CHECK CONSTRAINT [FK_ShipmentBL_Consignees]
GO
