USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[InfoMapping]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InfoMapping](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](50) NOT NULL,
	[Field] [nvarchar](50) NOT NULL,
	[EntityType] [nvarchar](255) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[EntityKeyField] [nvarchar](50) NULL,
 CONSTRAINT [PK_InfoMapping] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[InfoMapping] ON 

INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1024, N'Expected Entries', N'ExpectedEntries', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1026, N'Weight(Kg)', N'TotalWeight', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1027, N'Freight', N'TotalFreight', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1028, N'Currency', N'Currency_Code', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1029, N'BL', N'BLNumber', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1030, N'Country of Origin', N'Country_of_origin_code', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1031, N'Lot Number', N'CNumber', N'Discrepancy', 3, NULL)
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1032, N'Item Number', N'ItemNumber', N'Discrepancy', 3, NULL)
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1033, N'Description', N'ItemDescription', N'Discrepancy', 3, NULL)
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1034, N'Invoice Number', N'PreviousInvoiceNumber', N'Discrepancy', 3, NULL)
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1035, N'Credit Invoice Number', N'test', N'Discrepancy', 3, NULL)
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1036, N'Total Invoices', N'TotalInvoices', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1037, N'Packages', N'TotalPackages', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1038, N'MaxLines', N'MaxLines', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1039, N'Location of Goods', N'LocationOfGoods', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1040, N'FreightCurrency', N'FreightCurrencyCode', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1041, N'CNumber', N'CNumber', N'CNumber', 3, NULL)
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1042, N'Office', N'Office', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1043, N'PONumber', N'PONumber', N'PONumber', 3, NULL)
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1044, N'Package Type', N'PackageType', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1045, N'Manifest', N'Manifest_Number', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1046, N'Consignee Name', N'ConsigneeName', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1047, N'Consignee Code', N'ConsigneeCode', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
INSERT [dbo].[InfoMapping] ([Id], [Key], [Field], [EntityType], [ApplicationSettingsId], [EntityKeyField]) VALUES (1048, N'Consignee Address', N'ConsigneeAddress', N'AsycudaDocumentSet', 3, N'AsycudaDocumentSetId')
SET IDENTITY_INSERT [dbo].[InfoMapping] OFF
GO
ALTER TABLE [dbo].[InfoMapping]  WITH CHECK ADD  CONSTRAINT [FK_InfoMapping_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[InfoMapping] CHECK CONSTRAINT [FK_InfoMapping_ApplicationSettings]
GO
