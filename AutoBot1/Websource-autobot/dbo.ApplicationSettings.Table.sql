USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ApplicationSettings]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ApplicationSettings](
	[ApplicationSettingsId] [int] IDENTITY(1,1) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[MaxEntryLines] [int] NULL,
	[SoftwareName] [nvarchar](100) NULL,
	[AllowCounterPoint] [nvarchar](10) NULL,
	[GroupEX9] [bit] NULL,
	[InvoicePerEntry] [bit] NULL,
	[AllowTariffCodes] [nvarchar](10) NULL,
	[AllowWareHouse] [nvarchar](10) NULL,
	[AllowXBond] [nvarchar](10) NULL,
	[AllowAsycudaManager] [nvarchar](10) NULL,
	[AllowQuickBooks] [nvarchar](10) NULL,
	[ItemDescriptionContainsAsycudaAttribute] [bit] NULL,
	[AllowExportToExcel] [nvarchar](10) NULL,
	[AllowAutoWeightCalculation] [nvarchar](50) NULL,
	[AllowEntryPerIM7] [nvarchar](50) NULL,
	[AllowSalesToPI] [nvarchar](50) NULL,
	[AllowEffectiveAssessmentDate] [nvarchar](50) NULL,
	[AllowAutoFreightCalculation] [nvarchar](50) NULL,
	[AllowSubItems] [nvarchar](50) NULL,
	[AllowEntryDoNotAllocate] [nvarchar](50) NULL,
	[AllowPreviousItems] [nvarchar](50) NULL,
	[AllowOversShort] [nvarchar](50) NULL,
	[AllowContainers] [nvarchar](50) NULL,
	[AllowNonXEntries] [nvarchar](50) NULL,
	[AllowValidateTariffCodes] [nvarchar](50) NULL,
	[AllowCleanBond] [nvarchar](50) NULL,
	[OrderEntriesBy] [nvarchar](50) NULL,
	[OpeningStockDate] [datetime2](7) NOT NULL,
	[WeightCalculationMethod] [nvarchar](50) NULL,
	[BondQuantum] [float] NULL,
	[DataFolder] [nvarchar](999) NULL,
	[CompanyName] [nvarchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[Email] [nvarchar](255) NULL,
	[EmailPassword] [nvarchar](50) NULL,
	[AsycudaLogin] [nvarchar](50) NULL,
	[AsycudaPassword] [nvarchar](50) NULL,
	[AssessIM7] [bit] NULL,
	[AssessEX] [bit] NULL,
	[TestMode] [bit] NULL,
	[BondTypeId] [int] NOT NULL,
	[RequirePOs] [bit] NULL,
	[ExportNullTariffCodes] [bit] NULL,
	[PreAllocateEx9s] [bit] NULL,
	[AllowImportXSales] [nvarchar](10) NULL,
	[NotifyUnknownMessages] [bit] NULL,
	[ExportExpiredEntries] [bit] NULL,
	[AllowAdvanceWareHouse] [nvarchar](10) NULL,
	[AllowStressTest] [bit] NULL,
	[AllocationsOpeningStockDate] [datetime2](7) NULL,
	[GroupShipmentInvoices] [bit] NULL,
 CONSTRAINT [PK_ApplicationSettings] PRIMARY KEY CLUSTERED 
(
	[ApplicationSettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[ApplicationSettings] ON 

INSERT [dbo].[ApplicationSettings] ([ApplicationSettingsId], [Description], [MaxEntryLines], [SoftwareName], [AllowCounterPoint], [GroupEX9], [InvoicePerEntry], [AllowTariffCodes], [AllowWareHouse], [AllowXBond], [AllowAsycudaManager], [AllowQuickBooks], [ItemDescriptionContainsAsycudaAttribute], [AllowExportToExcel], [AllowAutoWeightCalculation], [AllowEntryPerIM7], [AllowSalesToPI], [AllowEffectiveAssessmentDate], [AllowAutoFreightCalculation], [AllowSubItems], [AllowEntryDoNotAllocate], [AllowPreviousItems], [AllowOversShort], [AllowContainers], [AllowNonXEntries], [AllowValidateTariffCodes], [AllowCleanBond], [OrderEntriesBy], [OpeningStockDate], [WeightCalculationMethod], [BondQuantum], [DataFolder], [CompanyName], [IsActive], [Email], [EmailPassword], [AsycudaLogin], [AsycudaPassword], [AssessIM7], [AssessEX], [TestMode], [BondTypeId], [RequirePOs], [ExportNullTariffCodes], [PreAllocateEx9s], [AllowImportXSales], [NotifyUnknownMessages], [ExportExpiredEntries], [AllowAdvanceWareHouse], [AllowStressTest], [AllocationsOpeningStockDate], [GroupShipmentInvoices]) VALUES (3, N'WaterNut', 50, N'Web Source Asycuda Toolkit', N'Hidden', 0, NULL, N'Visible', N'Visible', N'Collapsed', N'Visible', N'Collapsed', 0, N'Visible', N'Visible', N'Visible', N'Visible', N'Visible', N'Visible', N'Collapsed', N'Visible', N'Visible', N'Collapsed', N'Collapsed', N'Collapsed', N'Visible', N'Visible', N'Invoice', CAST(N'2022-10-15T00:00:00.0000000' AS DateTime2), N'Value', 0, N'D:\OneDrive\Clients\WebSource\Emails\', N'Web Source', 1, N'websource@auto-brokerage.com', N'WebSource', N'phillip.samara', N'PSAMARA1', 0, 0, 0, 1, NULL, NULL, NULL, NULL, 1, NULL, N'Visible', NULL, NULL, 1)
SET IDENTITY_INSERT [dbo].[ApplicationSettings] OFF
GO
ALTER TABLE [dbo].[ApplicationSettings]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationSettings_BondTypes] FOREIGN KEY([BondTypeId])
REFERENCES [dbo].[BondTypes] ([Id])
GO
ALTER TABLE [dbo].[ApplicationSettings] CHECK CONSTRAINT [FK_ApplicationSettings_BondTypes]
GO
ALTER TABLE [dbo].[ApplicationSettings]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationSettings_WeightCalculationMethods] FOREIGN KEY([WeightCalculationMethod])
REFERENCES [dbo].[WeightCalculationMethods] ([Name])
GO
ALTER TABLE [dbo].[ApplicationSettings] CHECK CONSTRAINT [FK_ApplicationSettings_WeightCalculationMethods]
GO
