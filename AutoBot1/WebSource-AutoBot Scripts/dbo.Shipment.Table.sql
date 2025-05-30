USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Shipment]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shipment](
	[Id] [int] NOT NULL,
	[ManifestNumber] [nvarchar](50) NULL,
	[ExpectedEntries] [int] NULL,
	[BLNumber] [nvarchar](50) NULL,
	[ShipmentName] [nvarchar](50) NOT NULL,
	[WeightKG] [float] NULL,
	[Currency] [nvarchar](3) NULL,
	[Origin] [nvarchar](2) NULL,
	[TotalInvoices] [int] NULL,
	[Packages] [int] NULL,
	[FreightCurrency] [nvarchar](3) NULL,
	[Location] [nvarchar](50) NULL,
	[Maxlines] [float] NULL,
	[Office] [nvarchar](50) NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[Freight] [float] NULL,
	[ConsigneeCode] [nvarchar](100) NULL,
	[ConsigneeName] [nvarchar](100) NULL,
	[ConsigneeAddress] [nvarchar](300) NULL,
 CONSTRAINT [PK_Shipment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Shipment]  WITH CHECK ADD  CONSTRAINT [FK_Shipment_Consignees] FOREIGN KEY([ConsigneeName])
REFERENCES [dbo].[Consignees] ([ConsigneeName])
GO
ALTER TABLE [dbo].[Shipment] CHECK CONSTRAINT [FK_Shipment_Consignees]
GO
