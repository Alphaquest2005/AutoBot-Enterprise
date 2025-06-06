USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[DocumentsCancelledList]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentsCancelledList](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[Year] [nvarchar](4) NOT NULL,
	[Office] [nvarchar](50) NOT NULL,
	[Declarant] [nvarchar](500) NOT NULL,
	[DeclarantCode] [nvarchar](50) NOT NULL,
	[ReferenceNumber] [nvarchar](50) NOT NULL,
	[RegistrationSerial] [nvarchar](50) NOT NULL,
	[RegistrationNumber] [nvarchar](50) NOT NULL,
	[RegistrationDate] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](50) NULL,
	[GeneralProcedure] [nvarchar](50) NULL,
	[Items] [nvarchar](50) NULL,
	[Exporter] [nvarchar](500) NULL,
	[RecieptSerial] [nvarchar](50) NULL,
	[RecieptNumber] [nvarchar](50) NOT NULL,
	[RecieptDate] [nvarchar](50) NULL,
	[Consignee] [nvarchar](500) NULL,
	[ConsigneeCode] [nvarchar](50) NULL,
	[TotalTaxes] [nvarchar](50) NULL,
	[WarehouseCode] [nvarchar](50) NULL,
	[AssessmentSerial] [nvarchar](1) NOT NULL,
	[AssessmentNumber] [nvarchar](8) NOT NULL,
	[AssessmentDate] [nvarchar](50) NOT NULL,
	[Color] [nvarchar](50) NULL,
 CONSTRAINT [PK_DocumentsCancelledList] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[DocumentsCancelledList] ON 

INSERT [dbo].[DocumentsCancelledList] ([Id], [ApplicationSettingsId], [Year], [Office], [Declarant], [DeclarantCode], [ReferenceNumber], [RegistrationSerial], [RegistrationNumber], [RegistrationDate], [Type], [GeneralProcedure], [Items], [Exporter], [RecieptSerial], [RecieptNumber], [RecieptDate], [Consignee], [ConsigneeCode], [TotalTaxes], [WarehouseCode], [AssessmentSerial], [AssessmentNumber], [AssessmentDate], [Color]) VALUES (1, 2, N'2021', N'GDFDX', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S', N'7.29E+12', N'244834005', N'C', N'3285', N'5/27/2021', N'IM', N'4', N'1', N'FLIR MARITIME US, INC. 9 TOWNSEND WEST, NASHUA, NH 03063,UNITED STATES', N'', N'', N'', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S GRENADA 0729094', N'7.29E+12', N'354.17', N'', N'L', N'3285', N'5/27/2021', N'')
INSERT [dbo].[DocumentsCancelledList] ([Id], [ApplicationSettingsId], [Year], [Office], [Declarant], [DeclarantCode], [ReferenceNumber], [RegistrationSerial], [RegistrationNumber], [RegistrationDate], [Type], [GeneralProcedure], [Items], [Exporter], [RecieptSerial], [RecieptNumber], [RecieptDate], [Consignee], [ConsigneeCode], [TotalTaxes], [WarehouseCode], [AssessmentSerial], [AssessmentNumber], [AssessmentDate], [Color]) VALUES (2, 2, N'2021', N'GDFDX', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S', N'7.29E+12', N'20217329', N'C', N'6333', N'9/29/2021', N'IM', N'4', N'1', N'TELEDYNE FLIR MARATIME US, INC 9 TOWNSEND WEST NASHAU, NH USA', N'', N'', N'', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S GRENADA 0729094', N'7.29E+12', N'1314.4', N'', N'L', N'6333', N'9/29/2021', N'')
INSERT [dbo].[DocumentsCancelledList] ([Id], [ApplicationSettingsId], [Year], [Office], [Declarant], [DeclarantCode], [ReferenceNumber], [RegistrationSerial], [RegistrationNumber], [RegistrationDate], [Type], [GeneralProcedure], [Items], [Exporter], [RecieptSerial], [RecieptNumber], [RecieptDate], [Consignee], [ConsigneeCode], [TotalTaxes], [WarehouseCode], [AssessmentSerial], [AssessmentNumber], [AssessmentDate], [Color]) VALUES (3, 2, N'2021', N'GDSGO', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S', N'7.29E+12', N'PEVGRE11334-M3CS', N'C', N'11394', N'3/19/2021', N'IM', N'4', N'4', N'3M GLOBAL CHANNEL SERVICES, INC. 3M CENTER, I-94 AND MCKNIGHT RD ST PAUL, MN 55144-1000 USA', N'', N'', N'', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S GRENADA 0729094', N'7.29E+12', N'0', N'', N'L', N'11394', N'3/19/2021', N'Red')
INSERT [dbo].[DocumentsCancelledList] ([Id], [ApplicationSettingsId], [Year], [Office], [Declarant], [DeclarantCode], [ReferenceNumber], [RegistrationSerial], [RegistrationNumber], [RegistrationDate], [Type], [GeneralProcedure], [Items], [Exporter], [RecieptSerial], [RecieptNumber], [RecieptDate], [Consignee], [ConsigneeCode], [TotalTaxes], [WarehouseCode], [AssessmentSerial], [AssessmentNumber], [AssessmentDate], [Color]) VALUES (4, 2, N'2021', N'GDSGO', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S', N'7.29E+12', N'SUCR444759-BM-F9', N'C', N'1608', N'1/9/2021', N'IM', N'4', N'2', N'GARMIN USA 1200 E 151ST STREET, OLATHE KS 66062, UNITED STATES', N'', N'', N'', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S GRENADA 0729094', N'7.29E+12', N'0', N'', N'L', N'1608', N'1/9/2021', N'Yellow')
INSERT [dbo].[DocumentsCancelledList] ([Id], [ApplicationSettingsId], [Year], [Office], [Declarant], [DeclarantCode], [ReferenceNumber], [RegistrationSerial], [RegistrationNumber], [RegistrationDate], [Type], [GeneralProcedure], [Items], [Exporter], [RecieptSerial], [RecieptNumber], [RecieptDate], [Consignee], [ConsigneeCode], [TotalTaxes], [WarehouseCode], [AssessmentSerial], [AssessmentNumber], [AssessmentDate], [Color]) VALUES (5, 2, N'2021', N'GDSGO', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S', N'7.29E+12', N'12BM', N'C', N'49656', N'11/22/2021', N'IM', N'4', N'2', N'LTL MANUFACTURING LOT 23K YORKE AVENUE O''MEARA INDUSTRIAL ESTATE ARIMA TRINIDAD', N'', N'', N'', N'BUDGET MARINE (GRENADA) LIMITED LANCE-AUX-EPINES ST. GEORGE''S GRENADA 0729094', N'7.29E+12', N'3686.64', N'', N'L', N'49658', N'11/22/2021', N'Green')
SET IDENTITY_INSERT [dbo].[DocumentsCancelledList] OFF
GO
