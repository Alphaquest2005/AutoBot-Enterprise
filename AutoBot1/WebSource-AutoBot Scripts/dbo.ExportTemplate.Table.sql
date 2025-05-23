USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ExportTemplate]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExportTemplate](
	[ExportTemplateId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[Description] [nvarchar](100) NULL,
	[Exporter_code] [nvarchar](100) NULL,
	[Exporter_name] [nvarchar](255) NULL,
	[Consignee_code] [nvarchar](100) NULL,
	[Consignee_name] [nvarchar](255) NULL,
	[Consignee_Address] [nvarchar](500) NULL,
	[Financial_code] [nvarchar](100) NULL,
	[Customs_clearance_office_code] [nvarchar](100) NULL,
	[Customs_Procedure] [nvarchar](100) NULL,
	[Declarant_code] [nvarchar](100) NULL,
	[Country_first_destination] [nvarchar](100) NULL,
	[Trading_country] [nvarchar](100) NULL,
	[Export_country_code] [nvarchar](100) NULL,
	[Destination_country_code] [nvarchar](100) NULL,
	[TransportName] [nvarchar](100) NULL,
	[TransportNationality] [nvarchar](100) NULL,
	[Location_of_goods] [nvarchar](100) NULL,
	[Border_information_Mode] [nvarchar](100) NULL,
	[Delivery_terms_Code] [nvarchar](100) NULL,
	[Border_office_Code] [nvarchar](100) NULL,
	[Gs_Invoice_Currency_code] [nvarchar](100) NULL,
	[Warehouse_Identification] [nvarchar](100) NULL,
	[Warehouse_Delay] [nvarchar](100) NULL,
	[Number_of_packages] [nvarchar](100) NULL,
	[Total_number_of_packages] [nvarchar](100) NULL,
	[Deffered_payment_reference] [nvarchar](100) NULL,
	[AttachedDocumentCode] [nvarchar](100) NULL,
	[Manifest] [nvarchar](100) NULL,
	[BL] [nvarchar](100) NULL,
 CONSTRAINT [PK_ExportTemplate] PRIMARY KEY CLUSTERED 
(
	[ExportTemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ExportTemplate] ON 

INSERT [dbo].[ExportTemplate] ([ExportTemplateId], [ApplicationSettingsId], [Description], [Exporter_code], [Exporter_name], [Consignee_code], [Consignee_name], [Consignee_Address], [Financial_code], [Customs_clearance_office_code], [Customs_Procedure], [Declarant_code], [Country_first_destination], [Trading_country], [Export_country_code], [Destination_country_code], [TransportName], [TransportNationality], [Location_of_goods], [Border_information_Mode], [Delivery_terms_Code], [Border_office_Code], [Gs_Invoice_Currency_code], [Warehouse_Identification], [Warehouse_Delay], [Number_of_packages], [Total_number_of_packages], [Deffered_payment_reference], [AttachedDocumentCode], [Manifest], [BL]) VALUES (39, 3, N'IM4', N'225756', N'WEB SOURCE GRENADA LTD
GCNA COMPLEX
KIRANI JAMES BLVD', NULL, NULL, NULL, NULL, N'GDWBS', N'4000-000', N'225756', N'US', N'US', N'US', N'GD', N'TROPIC CARIB', N'PA', N'STGYARD', N'4', N'FOB', N'GDWBS', N'USD', N'225756', N'730', N'1', N'1', N'CR225756', N'IV05', NULL, NULL)
INSERT [dbo].[ExportTemplate] ([ExportTemplateId], [ApplicationSettingsId], [Description], [Exporter_code], [Exporter_name], [Consignee_code], [Consignee_name], [Consignee_Address], [Financial_code], [Customs_clearance_office_code], [Customs_Procedure], [Declarant_code], [Country_first_destination], [Trading_country], [Export_country_code], [Destination_country_code], [TransportName], [TransportNationality], [Location_of_goods], [Border_information_Mode], [Delivery_terms_Code], [Border_office_Code], [Gs_Invoice_Currency_code], [Warehouse_Identification], [Warehouse_Delay], [Number_of_packages], [Total_number_of_packages], [Deffered_payment_reference], [AttachedDocumentCode], [Manifest], [BL]) VALUES (41, 3, N'IM4', N'225756', N'WEB SOURCE GRENADA LTD
GCNA COMPLEX
KIRANI JAMES BLVD', NULL, NULL, NULL, NULL, N'GDWBS', N'4000-800', N'225756', N'US', N'US', N'US', N'GD', N'TROPIC CARIB', N'PA', N'STGYARD', N'4', N'FOB', N'GDWBS', N'USD', N'225756', N'730', N'1', N'1', N'CR225756', N'IV05', NULL, NULL)
INSERT [dbo].[ExportTemplate] ([ExportTemplateId], [ApplicationSettingsId], [Description], [Exporter_code], [Exporter_name], [Consignee_code], [Consignee_name], [Consignee_Address], [Financial_code], [Customs_clearance_office_code], [Customs_Procedure], [Declarant_code], [Country_first_destination], [Trading_country], [Export_country_code], [Destination_country_code], [TransportName], [TransportNationality], [Location_of_goods], [Border_information_Mode], [Delivery_terms_Code], [Border_office_Code], [Gs_Invoice_Currency_code], [Warehouse_Identification], [Warehouse_Delay], [Number_of_packages], [Total_number_of_packages], [Deffered_payment_reference], [AttachedDocumentCode], [Manifest], [BL]) VALUES (1045, 3, N'SE4', N'225756', N'WEB SOURCE GRENADA LTD
GCNA COMPLEX
KIRANI JAMES BLVD', NULL, NULL, N'St. George''s Grenada', NULL, N'GDWBS', N'4000-000', N'225756', N'US', N'US', N'US', N'GD', N'TROPIC CARIB', N'PA', N'STGYARD', N'4', N'FOB', N'GDWBS', N'USD', N'225756', N'730', N'1', N'1', N'CR225756', N'IV05', NULL, NULL)
INSERT [dbo].[ExportTemplate] ([ExportTemplateId], [ApplicationSettingsId], [Description], [Exporter_code], [Exporter_name], [Consignee_code], [Consignee_name], [Consignee_Address], [Financial_code], [Customs_clearance_office_code], [Customs_Procedure], [Declarant_code], [Country_first_destination], [Trading_country], [Export_country_code], [Destination_country_code], [TransportName], [TransportNationality], [Location_of_goods], [Border_information_Mode], [Delivery_terms_Code], [Border_office_Code], [Gs_Invoice_Currency_code], [Warehouse_Identification], [Warehouse_Delay], [Number_of_packages], [Total_number_of_packages], [Deffered_payment_reference], [AttachedDocumentCode], [Manifest], [BL]) VALUES (1048, 3, N'SD4', N'225756', N'WEB SOURCE GRENADA LTD', NULL, NULL, N'St. George''s Grenada', NULL, N'GDWBS', N'4200-000', N'225756', N'US', N'US', N'US', N'GD', N'TROPIC CARIB', N'PA', N'STGYARD', N'4', N'FOB', N'GDWBS', N'USD', N'225756', N'730', N'1', N'1', N'CR225756', N'IV05', NULL, NULL)
SET IDENTITY_INSERT [dbo].[ExportTemplate] OFF
GO
ALTER TABLE [dbo].[ExportTemplate] ADD  CONSTRAINT [DF_ExportTemplate_ApplicationSettingsId]  DEFAULT ((2)) FOR [ApplicationSettingsId]
GO
ALTER TABLE [dbo].[ExportTemplate]  WITH CHECK ADD  CONSTRAINT [FK_ExportTemplate_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExportTemplate] CHECK CONSTRAINT [FK_ExportTemplate_ApplicationSettings]
GO
