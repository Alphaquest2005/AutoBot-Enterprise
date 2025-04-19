USE [BudgetMarine-AutoBot]
GO
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSet] ON 
GO
INSERT [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId], [ApplicationSettingsId], [Declarant_Reference_Number], [Exchange_Rate], [Customs_ProcedureId], [Country_of_origin_code], [Currency_Code], [Document_TypeId], [Description], [Manifest_Number], [BLNumber], [EntryTimeStamp], [StartingFileCount], [ApportionMethod], [TotalWeight], [TotalFreight], [TotalPackages], [LastFileNumber], [TotalInvoices], [MaxLines], [LocationOfGoods], [FreightCurrencyCode], [Office], [UpgradeKey], [ExpectedEntries]) VALUES (5, 2, N'Destruction', 0, 1, N'QN', N'XCD', 1, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, 280, NULL, NULL, NULL, N'USD', NULL, NULL, NULL)
go
SET IDENTITY_INSERT [dbo].[AsycudaDocumentSet] OFF
GO
