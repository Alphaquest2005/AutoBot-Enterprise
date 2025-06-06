USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentManifest]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentManifest](
	[RegistrationNumber] [nvarchar](50) NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RegistrationDate] [date] NOT NULL,
	[CustomsOffice] [nvarchar](50) NOT NULL,
	[Voyage] [nvarchar](50) NULL,
	[ETD] [date] NULL,
	[ETA] [date] NULL,
	[Vessel] [nvarchar](50) NULL,
	[WayBill] [nvarchar](50) NOT NULL,
	[LineNumber] [int] NULL,
	[LoadingPort] [nvarchar](50) NULL,
	[ModeOfTransport] [nvarchar](50) NULL,
	[TypeOfBL] [nvarchar](50) NULL,
	[CargoReporter] [nvarchar](255) NULL,
	[Exporter] [nvarchar](50) NULL,
	[ConsigneeName] [nvarchar](100) NULL,
	[Notify] [nvarchar](50) NULL,
	[Packages] [int] NOT NULL,
	[PackageType] [nvarchar](50) NOT NULL,
	[GrossWeightKG] [float] NOT NULL,
	[Volume] [float] NULL,
	[Freight] [float] NULL,
	[LocationOfGoods] [nvarchar](255) NULL,
	[Goods] [nvarchar](1000) NULL,
	[Marks] [nvarchar](1000) NULL,
	[Containers] [int] NULL,
	[EmailId] [nvarchar](255) NULL,
	[SourceFile] [nvarchar](max) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[FileTypeId] [int] NOT NULL,
	[FreightCurrency] [nvarchar](10) NULL,
 CONSTRAINT [PK_ShipmentManifest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ShipmentManifest] ON 

INSERT [dbo].[ShipmentManifest] ([RegistrationNumber], [Id], [RegistrationDate], [CustomsOffice], [Voyage], [ETD], [ETA], [Vessel], [WayBill], [LineNumber], [LoadingPort], [ModeOfTransport], [TypeOfBL], [CargoReporter], [Exporter], [ConsigneeName], [Notify], [Packages], [PackageType], [GrossWeightKG], [Volume], [Freight], [LocationOfGoods], [Goods], [Marks], [Containers], [EmailId], [SourceFile], [ApplicationSettingsId], [FileTypeId], [FreightCurrency]) VALUES (N'2024 28', 7, CAST(N'0001-01-01' AS Date), N'GDWBS', NULL, NULL, NULL, NULL, N'HAWB9595443', NULL, NULL, NULL, NULL, NULL, NULL, N'AARON WILSON', NULL, 1, N'Package', 8, NULL, 17, N'WEB SOURCE', NULL, NULL, NULL, N'03142025_7_24_24, 3_53 PM am^on.coiti‘.pdf', N'D:\OneDrive\Clients\WebSource\Emails\Documents\03142025_7_24_24, 3_53 PM am^on.coiti‘\HAWB9595443-Manifest.pdf', 3, 1185, N'US')
INSERT [dbo].[ShipmentManifest] ([RegistrationNumber], [Id], [RegistrationDate], [CustomsOffice], [Voyage], [ETD], [ETA], [Vessel], [WayBill], [LineNumber], [LoadingPort], [ModeOfTransport], [TypeOfBL], [CargoReporter], [Exporter], [ConsigneeName], [Notify], [Packages], [PackageType], [GrossWeightKG], [Volume], [Freight], [LocationOfGoods], [Goods], [Marks], [Containers], [EmailId], [SourceFile], [ApplicationSettingsId], [FileTypeId], [FreightCurrency]) VALUES (N'2024 28', 8, CAST(N'0001-01-01' AS Date), N'GDWBS', NULL, NULL, NULL, NULL, N'HAWB9595459', NULL, NULL, NULL, NULL, NULL, NULL, N'AARON WILSON', NULL, 1, N'Package', 8, NULL, 17, N'WEB SOURCE', NULL, NULL, NULL, N'03142025_7_24_24, 3_53 PM am^on.coiti‘.pdf', N'D:\OneDrive\Clients\WebSource\Emails\Documents\03142025_7_24_24, 3_53 PM am^on.coiti‘\HAWB9595459-Manifest.pdf', 3, 1185, N'US')
INSERT [dbo].[ShipmentManifest] ([RegistrationNumber], [Id], [RegistrationDate], [CustomsOffice], [Voyage], [ETD], [ETA], [Vessel], [WayBill], [LineNumber], [LoadingPort], [ModeOfTransport], [TypeOfBL], [CargoReporter], [Exporter], [ConsigneeName], [Notify], [Packages], [PackageType], [GrossWeightKG], [Volume], [Freight], [LocationOfGoods], [Goods], [Marks], [Containers], [EmailId], [SourceFile], [ApplicationSettingsId], [FileTypeId], [FreightCurrency]) VALUES (N'2024 28', 9, CAST(N'0001-01-01' AS Date), N'GDWBS', NULL, NULL, NULL, NULL, N'HAWB9596948', NULL, NULL, NULL, NULL, NULL, NULL, N'AARON S WILSON', NULL, 1, N'Package', 8, NULL, 17, N'WEB SOURCE', NULL, NULL, NULL, N'03142025_7_24_24, 3_53 PM am^on.coiti‘.pdf', N'D:\OneDrive\Clients\WebSource\Emails\Documents\03142025_7_24_24, 3_53 PM am^on.coiti‘\HAWB9596948-Manifest.pdf', 3, 1185, N'US')
SET IDENTITY_INSERT [dbo].[ShipmentManifest] OFF
GO
ALTER TABLE [dbo].[ShipmentManifest]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentManifest_Consignees] FOREIGN KEY([ConsigneeName])
REFERENCES [dbo].[Consignees] ([ConsigneeName])
GO
ALTER TABLE [dbo].[ShipmentManifest] CHECK CONSTRAINT [FK_ShipmentManifest_Consignees]
GO
