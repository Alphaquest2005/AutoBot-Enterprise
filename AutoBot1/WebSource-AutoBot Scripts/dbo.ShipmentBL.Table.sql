USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentBL]    Script Date: 4/8/2025 8:33:17 AM ******/
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
SET IDENTITY_INSERT [dbo].[ShipmentBL] ON 

INSERT [dbo].[ShipmentBL] ([BLNumber], [Vessel], [Voyage], [Container], [Seals], [Type], [PackagesNo], [PackagesType], [WeightKG], [VolumeM3], [WeightLB], [VolumeCF], [Reference], [EmailId], [SourceFile], [ApplicationSettingsId], [FileTypeId], [Id], [Freight], [FreightCurrency], [ConsigneeName]) VALUES (N'TSCW17748793', NULL, NULL, NULL, NULL, NULL, 0, N'PK', 181, 0.82, 400, 2.9, NULL, N'Fw: Invoices * BOL--2025-04-07-13:16:27', N'D:\OneDrive\Clients\WebSource\Emails\Shipments\5\TSCW17748793-BL.pdf', 3, 112, 4674, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[ShipmentBL] OFF
GO
ALTER TABLE [dbo].[ShipmentBL]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentBL_Consignees] FOREIGN KEY([ConsigneeName])
REFERENCES [dbo].[Consignees] ([ConsigneeName])
GO
ALTER TABLE [dbo].[ShipmentBL] CHECK CONSTRAINT [FK_ShipmentBL_Consignees]
GO
