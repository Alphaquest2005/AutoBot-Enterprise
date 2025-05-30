USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR-FieldMappings]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCR-FieldMappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](50) NOT NULL,
	[Field] [nvarchar](50) NOT NULL,
	[EntityType] [nvarchar](50) NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[DataType] [nvarchar](50) NOT NULL,
	[AppendValues] [bit] NULL,
	[FileTypeId] [int] NOT NULL,
 CONSTRAINT [PK_OCR-FieldMappings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[OCR-FieldMappings] ON 

INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (1, N'InvoiceTotal', N'InvoiceTotal', N'Invoice', 1, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (2, N'InvoiceNo', N'InvoiceNo', N'Invoice', 1, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (3, N'InvoiceDate', N'InvoiceDate', N'Invoice', 1, N'Date', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (4, N'Freight', N'TotalInternalFreight', N'Invoice', 1, N'Number', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (5, N'SalesTax', N'TotalOtherCost', N'Invoice', 1, N'Number', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (6, N'SubTotal', N'SubTotal', N'Invoice', 1, N'Number', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (7, N'PONumber', N'PONumber', N'ExtraInfo', 0, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (8, N'SupplierCode', N'SupplierCode', N'Invoice', 1, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (9, N'SupplierCode', N'Name', N'Invoice', 1, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (10, N'ItemNumber', N'ItemNumber', N'InvoiceDetails', 1, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (11, N'Description', N'ItemDescription', N'InvoiceDetails', 1, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (12, N'Quantity', N'Quantity', N'InvoiceDetails', 1, N'Number', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (13, N'UnitPrice', N'Cost', N'InvoiceDetails', 1, N'Number', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (14, N'ExtendedPrice', N'TotalCost', N'InvoiceDetails', 1, N'Number', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (15, N'Currency', N'Currency', N'Invoice', 0, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (16, N'TrackingInfo', N'TrackingInfo', N'ExtraInfo', 0, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (17, N'Fees', N'TotalOtherCost', N'Invoice', 0, N'Number', 1, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (18, N'Description1', N'ItemDescription', N'InvoiceDetails', 1, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (19, N'Description2', N'ItemDescription', N'InvoiceDetails', 0, N'String', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (20, N'Vessel', N'Vessel', N'ShipmentBL', 0, N'String', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (21, N'BLNumber', N'BLNumber', N'ShipmentBL', 0, N'String', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (22, N'Reference', N'Voyage', N'ShipmentBL', 0, N'String', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (23, N'Packages', N'PackagesNo', N'ShipmentBL', 0, N'Number', 1, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (24, N'WeightKG', N'WeightKG', N'ShipmentBL', 0, N'Number', 1, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (25, N'VolumeM3', N'VolumeM3', N'ShipmentBL', 0, N'Number', 1, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (26, N'WeightLB', N'WeightLB', N'ShipmentBL', 0, N'Number', 1, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (27, N'VolumeFT3', N'VolumeCF', N'ShipmentBL', 0, N'Number', 1, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (28, N'Quantity', N'Quantity', N'ShipmentBLDetails', 0, N'Number', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (29, N'PackageType', N'PackageType', N'ShipmentBLDetails', 0, N'String', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (30, N'Marks', N'Marks', N'ShipmentBLDetails', 0, N'String', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (31, N'Weight', N'Weight', N'ShipmentBLDetails', 0, N'Number', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (32, N'Name', N'Name', N'ShipmentBL', 1, N'String', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (33, N'Freight', N'Freight', N'ShipmentBL', 1, N'Number', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (34, N'Currency', N'FreightCurrency', N'ShipmentBL', 0, N'String', NULL, 112)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (35, N'Tax', N'TotalOtherCost', N'Invoice', 1, N'Number', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (36, N'Shipped', N'Quantity', N'InvoiceDetails', 1, N'Number', NULL, 1147)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (37, N'ETA', N'ETA', N'ShipmentRider', 1, N'Date', NULL, 1174)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (38, N'PONumber', N'PONumber', N'ShipmentRiderDetails', 0, N'String', NULL, 1174)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (39, N'SalesOrder', N'SalesOrder', N'ShipmentRiderDetails', 0, N'String', NULL, 1174)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (40, N'Pieces', N'Pieces', N'ShipmentRiderDetails', 1, N'String', NULL, 1174)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (41, N'Description', N'Description', N'ShipmentRiderDetails', 1, N'String', NULL, 1174)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (42, N'InvoiceNo', N'InvoiceNo', N'ShipmentRiderDetails', 1, N'String', NULL, 1174)
INSERT [dbo].[OCR-FieldMappings] ([Id], [Key], [Field], [EntityType], [IsRequired], [DataType], [AppendValues], [FileTypeId]) VALUES (43, N'InvoiceTotal', N'InvoiceTotal', N'ShipmentRiderDetails', 1, N'Number', NULL, 1174)
SET IDENTITY_INSERT [dbo].[OCR-FieldMappings] OFF
GO
ALTER TABLE [dbo].[OCR-FieldMappings]  WITH CHECK ADD  CONSTRAINT [FK_OCR-FieldMappings_FileTypes] FOREIGN KEY([FileTypeId])
REFERENCES [dbo].[FileTypes] ([Id])
GO
ALTER TABLE [dbo].[OCR-FieldMappings] CHECK CONSTRAINT [FK_OCR-FieldMappings_FileTypes]
GO
