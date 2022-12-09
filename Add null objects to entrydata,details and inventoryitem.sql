USE [ColumbianEmeralds-DiscoveryDB]
GO
SET IDENTITY_INSERT [dbo].[EntryData] ON 
GO
INSERT [dbo].[EntryData] ([EntryDataId], [ApplicationSettingsId], [EntryDataDate], [InvoiceTotal], [ImportedLines], [SupplierCode], [TotalFreight], [TotalInternalFreight], [TotalWeight], [Currency], [FileTypeId], [TotalOtherCost], [TotalInsurance], [TotalDeduction], [SourceFile], [EntryData_Id], [Packages], [UpgradeKey], [EntryType], [EmailId]) VALUES (N'Unknown', 6, CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), 0, NULL, NULL, 0, 0, 0, NULL, 127, 0, 0, 0, N'made up', 0, 0, NULL, N'Sales', NULL)
GO

SET IDENTITY_INSERT [dbo].[EntryData] OFF
GO
INSERT [dbo].[EntryData_Sales] ([INVNumber], [CustomerName], [EntryData_Id], [Tax]) VALUES (N'Unknown', NULL, 0, 0)
GO

SET IDENTITY_INSERT [dbo].InventoryItems ON 
GO

INSERT INTO InventoryItems
                         (ItemNumber, ApplicationSettingsId, Description, Id)VALUES        ('Unknown',6,'Unknown',0)
						 GO

SET IDENTITY_INSERT [dbo].InventoryItems OFF
GO


SET IDENTITY_INSERT [dbo].[EntryDataDetails] ON 
GO
INSERT [dbo].[EntryDataDetails] ([EntryDataDetailsId], [EntryData_Id], [EntryDataId], [LineNumber], [ItemNumber], [Quantity], [Units], [ItemDescription], [Cost], [TotalCost], [QtyAllocated], [UnitWeight], [DoNotAllocate], [Freight], [Weight], [InternalFreight], [Status], [InvoiceQty], [ReceivedQty], [PreviousInvoiceNumber], [CNumber], [Comment], [EffectiveDate], [IsReconciled], [TaxAmount], [LastCost], [InventoryItemId], [FileLineNumber], [UpgradeKey], [VolumeLiters], [CLineNumber]) VALUES (0, 0, N'Unknown', 0, 'Unknown', 0, NULL, 'Unknown', 0, 0, 0, 0, NULL, 0, 0, 0, NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, 0, NULL, 0, 0, NULL, 0, NULL)
GO

SET IDENTITY_INSERT [dbo].[EntryDataDetails] OFF
GO
