USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaDocumentSet]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaDocumentSet](
	[AsycudaDocumentSetId] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[Declarant_Reference_Number] [nvarchar](50) NULL,
	[Exchange_Rate] [float] NOT NULL,
	[Customs_ProcedureId] [int] NULL,
	[Country_of_origin_code] [nvarchar](3) NULL,
	[Currency_Code] [nvarchar](3) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[Manifest_Number] [nvarchar](50) NULL,
	[BLNumber] [nvarchar](50) NULL,
	[EntryTimeStamp] [datetime2](7) NULL,
	[StartingFileCount] [int] NULL,
	[ApportionMethod] [nvarchar](50) NULL,
	[TotalWeight] [float] NULL,
	[TotalFreight] [float] NULL,
	[TotalPackages] [int] NULL,
	[LastFileNumber] [int] NULL,
	[TotalInvoices] [int] NULL,
	[MaxLines] [int] NULL,
	[LocationOfGoods] [nvarchar](50) NULL,
	[FreightCurrencyCode] [nvarchar](3) NOT NULL,
	[Office] [nvarchar](50) NULL,
	[UpgradeKey] [int] NULL,
	[ExpectedEntries] [int] NULL,
	[PackageType] [nvarchar](50) NULL,
	[ConsigneeName] [nvarchar](100) NULL,
 CONSTRAINT [PK_AsycudaDocumentSet] PRIMARY KEY CLUSTERED 
(
	[AsycudaDocumentSetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSet] ON 

INSERT [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId], [ApplicationSettingsId], [Declarant_Reference_Number], [Exchange_Rate], [Customs_ProcedureId], [Country_of_origin_code], [Currency_Code], [Description], [Manifest_Number], [BLNumber], [EntryTimeStamp], [StartingFileCount], [ApportionMethod], [TotalWeight], [TotalFreight], [TotalPackages], [LastFileNumber], [TotalInvoices], [MaxLines], [LocationOfGoods], [FreightCurrencyCode], [Office], [UpgradeKey], [ExpectedEntries], [PackageType], [ConsigneeName]) VALUES (5381, 3, N'Imports', 0, 66, N'US', N'USD', NULL, NULL, NULL, CAST(N'2020-10-25T23:29:21.7136509' AS DateTime2), NULL, NULL, 0, 0, NULL, 58, NULL, NULL, NULL, N'USD', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId], [ApplicationSettingsId], [Declarant_Reference_Number], [Exchange_Rate], [Customs_ProcedureId], [Country_of_origin_code], [Currency_Code], [Description], [Manifest_Number], [BLNumber], [EntryTimeStamp], [StartingFileCount], [ApportionMethod], [TotalWeight], [TotalFreight], [TotalPackages], [LastFileNumber], [TotalInvoices], [MaxLines], [LocationOfGoods], [FreightCurrencyCode], [Office], [UpgradeKey], [ExpectedEntries], [PackageType], [ConsigneeName]) VALUES (5428, 3, N'Shipments', 0, 66, N'US', N'USD', NULL, NULL, NULL, NULL, NULL, N'Equal', 0, 0, NULL, NULL, NULL, NULL, NULL, N'USD', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId], [ApplicationSettingsId], [Declarant_Reference_Number], [Exchange_Rate], [Customs_ProcedureId], [Country_of_origin_code], [Currency_Code], [Description], [Manifest_Number], [BLNumber], [EntryTimeStamp], [StartingFileCount], [ApportionMethod], [TotalWeight], [TotalFreight], [TotalPackages], [LastFileNumber], [TotalInvoices], [MaxLines], [LocationOfGoods], [FreightCurrencyCode], [Office], [UpgradeKey], [ExpectedEntries], [PackageType], [ConsigneeName]) VALUES (7847, 3, N'HAWB9595443', 0, 205, N'US', N'USD', NULL, N'2024 28', N'HAWB9595443', CAST(N'2025-04-03T11:14:32.6476786' AS DateTime2), NULL, NULL, 8, 17, 1, 2, 1, NULL, N'WEB SOURCE', N'US', N'GDWBS', NULL, 1, NULL, N'AARON WILSON')
INSERT [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId], [ApplicationSettingsId], [Declarant_Reference_Number], [Exchange_Rate], [Customs_ProcedureId], [Country_of_origin_code], [Currency_Code], [Description], [Manifest_Number], [BLNumber], [EntryTimeStamp], [StartingFileCount], [ApportionMethod], [TotalWeight], [TotalFreight], [TotalPackages], [LastFileNumber], [TotalInvoices], [MaxLines], [LocationOfGoods], [FreightCurrencyCode], [Office], [UpgradeKey], [ExpectedEntries], [PackageType], [ConsigneeName]) VALUES (7848, 3, N'March 2025', 0, 205, NULL, N'USD', NULL, NULL, NULL, CAST(N'2025-04-03T11:15:13.3497285' AS DateTime2), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'USD', NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId], [ApplicationSettingsId], [Declarant_Reference_Number], [Exchange_Rate], [Customs_ProcedureId], [Country_of_origin_code], [Currency_Code], [Description], [Manifest_Number], [BLNumber], [EntryTimeStamp], [StartingFileCount], [ApportionMethod], [TotalWeight], [TotalFreight], [TotalPackages], [LastFileNumber], [TotalInvoices], [MaxLines], [LocationOfGoods], [FreightCurrencyCode], [Office], [UpgradeKey], [ExpectedEntries], [PackageType], [ConsigneeName]) VALUES (7849, 3, N'HAWB9595459', 0, 205, N'US', N'USD', NULL, N'2024 28', N'HAWB9595459', CAST(N'2025-04-04T01:44:27.6566555' AS DateTime2), NULL, NULL, 8, 17, 1, 2, 1, NULL, N'WEB SOURCE', N'US', N'GDWBS', NULL, 1, NULL, N'AARON WILSON')
INSERT [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId], [ApplicationSettingsId], [Declarant_Reference_Number], [Exchange_Rate], [Customs_ProcedureId], [Country_of_origin_code], [Currency_Code], [Description], [Manifest_Number], [BLNumber], [EntryTimeStamp], [StartingFileCount], [ApportionMethod], [TotalWeight], [TotalFreight], [TotalPackages], [LastFileNumber], [TotalInvoices], [MaxLines], [LocationOfGoods], [FreightCurrencyCode], [Office], [UpgradeKey], [ExpectedEntries], [PackageType], [ConsigneeName]) VALUES (7850, 3, N'HAWB9596948', 0, 205, N'US', N'USD', NULL, N'2024 28', N'HAWB9596948', CAST(N'2025-04-04T01:44:29.0893535' AS DateTime2), NULL, NULL, 8, 17, 1, 2, 1, NULL, N'WEB SOURCE', N'US', N'GDWBS', NULL, 1, NULL, N'AARON S WILSON')
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSet] OFF
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] ADD  CONSTRAINT [DF_AsycudaDocumentSet_Currency_Code]  DEFAULT (N'USD') FOR [Currency_Code]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] ADD  CONSTRAINT [DF_AsycudaDocumentSet_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] ADD  CONSTRAINT [DF_AsycudaDocumentSet_FreightCurrencyCode]  DEFAULT (N'USD') FOR [FreightCurrencyCode]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_ApplicationSettings] FOREIGN KEY([ApplicationSettingsId])
REFERENCES [dbo].[ApplicationSettings] ([ApplicationSettingsId])
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] CHECK CONSTRAINT [FK_AsycudaDocumentSet_ApplicationSettings]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_Consignees] FOREIGN KEY([ConsigneeName])
REFERENCES [dbo].[Consignees] ([ConsigneeName])
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] CHECK CONSTRAINT [FK_AsycudaDocumentSet_Consignees]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_Customs_Procedure] FOREIGN KEY([Customs_ProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] CHECK CONSTRAINT [FK_AsycudaDocumentSet_Customs_Procedure]
GO
