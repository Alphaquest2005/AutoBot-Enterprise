USE [master]
GO
/****** Object:  Database [WaterNutDB]    Script Date: 6/11/2013 9:44:44 PM ******/

ALTER DATABASE [TurbulenceDB] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [WaterNutDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [WaterNutDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [WaterNutDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [WaterNutDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [WaterNutDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [WaterNutDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [WaterNutDB] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [WaterNutDB] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [WaterNutDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [WaterNutDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [WaterNutDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [WaterNutDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [WaterNutDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [WaterNutDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [WaterNutDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [WaterNutDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [WaterNutDB] SET  DISABLE_BROKER 
GO
ALTER DATABASE [WaterNutDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [WaterNutDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [WaterNutDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [WaterNutDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [WaterNutDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [WaterNutDB] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [WaterNutDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [WaterNutDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [WaterNutDB] SET  MULTI_USER 
GO
ALTER DATABASE [WaterNutDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [WaterNutDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [WaterNutDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [WaterNutDB] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [WaterNutDB]
GO
/****** Object:  StoredProcedure [dbo].[UpdateAsycudaEntry]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateAsycudaEntry]
	-- Add the parameters for the stored procedure here
	@Item_Id int,
	@QtyAllocated Float
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Update xcuda_Item
	Set QtyAllocated = @QtyAllocated
	where Item_Id = @Item_Id

END

GO
/****** Object:  Table [dbo].[ApplicationSettings]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ApplicationSettings](
	[ApplicationSettingsId] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](max) NULL,
	[MaxEntryLines] [int] NULL,
	[SoftwareName] [varchar](max) NULL,
 CONSTRAINT [PK_ApplicationSettings] PRIMARY KEY CLUSTERED 
(
	[ApplicationSettingsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AsycudaDocumentSet]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AsycudaDocumentSet](
	[AsycudaDocumentSetId] [int] IDENTITY(1,1) NOT NULL,
	[Declarant_Reference_Number] [varchar](50) NULL,
	[Exchange_Rate] [float] NULL,
	[Customs_ProcedureId] [int] NULL,
	[Country_of_origin_code] [varchar](50) NULL,
	[Currency_Code] [varchar](50) NULL,
	[Document_Type_Id] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[ExportTemplate_Id] [int] NULL,
	[Manifest_Number] [varchar](50) NULL,
	[BLNumber] [varchar](50) NULL,
	[EntryTimeStamp] [datetime] NULL,
 CONSTRAINT [PK_AsycudaDocumentSet] PRIMARY KEY CLUSTERED 
(
	[AsycudaDocumentSetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AsycudaSalesAllocations]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AsycudaSalesAllocations](
	[AllocationId] [int] IDENTITY(1,1) NOT NULL,
	[EntryDataDetailsId] [int] NULL,
	[PreviousItem_Id] [int] NULL,
	[Status] [varchar](max) NULL,
	[QtyAllocated] [float] NULL,
	[EntryTimeStamp] [datetime] NULL,
	[EANumber] [int] NULL,
	[SANumber] [int] NULL,
	[xEntryItem_Id] [int] NULL,
 CONSTRAINT [PK_AsycudaSalesAllocations] PRIMARY KEY CLUSTERED 
(
	[AllocationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Customs_Procedure]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Customs_Procedure](
	[Document_Type_Id] [int] NOT NULL,
	[Customs_ProcedureId] [int] IDENTITY(1,1) NOT NULL,
	[Extended_customs_procedure] [varchar](max) NULL,
	[National_customs_procedure] [varchar](max) NULL,
 CONSTRAINT [PK_Customs_Procedure] PRIMARY KEY CLUSTERED 
(
	[Customs_ProcedureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Document_Type]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Document_Type](
	[Document_Type_Id] [int] IDENTITY(1,1) NOT NULL,
	[Type_of_declaration] [varchar](max) NULL,
	[Declaration_gen_procedure_code] [varchar](max) NULL,
	[DefaultCustoms_ProcedureId] [int] NULL,
 CONSTRAINT [PK_Document_Type] PRIMARY KEY CLUSTERED 
(
	[Document_Type_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EntryData]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EntryData](
	[AsycudaDocumentSetId] [int] NOT NULL,
	[EntryDataId] [varchar](50) NOT NULL,
	[EntryDataDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY CLUSTERED 
(
	[EntryDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EntryData_OpeningStock]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EntryData_OpeningStock](
	[OPSNumber] [nvarchar](max) NOT NULL,
	[EntryDataId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_EntryData_OpeningStock] PRIMARY KEY CLUSTERED 
(
	[EntryDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EntryData_PurchaseOrders]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EntryData_PurchaseOrders](
	[PONumber] [nvarchar](max) NOT NULL,
	[EntryDataId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_EntryData_PurchaseOrders] PRIMARY KEY CLUSTERED 
(
	[EntryDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EntryData_Sales]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EntryData_Sales](
	[INVNumber] [nvarchar](max) NOT NULL,
	[EntryDataId] [varchar](50) NOT NULL,
	[TaxAmount] [float] NULL,
	[CustomerName] [varchar](max) NULL,
 CONSTRAINT [PK_EntryData_Sales] PRIMARY KEY CLUSTERED 
(
	[EntryDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EntryDataDetails]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EntryDataDetails](
	[EntryDataDetailsId] [int] IDENTITY(1,1) NOT NULL,
	[EntryDataId] [varchar](50) NOT NULL,
	[LineNumber] [int] NULL,
	[ItemNumber] [varchar](50) NOT NULL,
	[Quantity] [decimal](15, 4) NOT NULL,
	[Units] [varchar](15) NULL,
	[ItemDescription] [varchar](50) NOT NULL,
	[Cost] [decimal](15, 4) NOT NULL,
	[QtyAllocated] [float] NULL,
	[UnitWeight] [decimal](15, 4) NULL,
 CONSTRAINT [PK_PurchaseOrderDetails] PRIMARY KEY CLUSTERED 
(
	[EntryDataDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExportTemplate]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExportTemplate](
	[ExportTemplate_Id] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](50) NULL,
	[Exporter_code] [varchar](50) NULL,
	[Exporter_name] [varchar](max) NULL,
	[Consignee_code] [varchar](50) NULL,
	[Financial_code] [varchar](50) NULL,
	[Customs_clearance_office_code] [varchar](50) NULL,
	[Declarant_code] [varchar](50) NULL,
	[Country_first_destination] [varchar](50) NULL,
	[Trading_country] [varchar](50) NULL,
	[Export_country_code] [varchar](50) NULL,
	[Destination_country_code] [varchar](50) NULL,
	[TransportName] [varchar](50) NULL,
	[TransportNationality] [varchar](50) NULL,
	[Location_of_goods] [varchar](50) NULL,
	[Border_information_Mode] [varchar](50) NULL,
	[Delivery_terms_Code] [varchar](50) NULL,
	[Border_office_Code] [varchar](50) NULL,
	[Gs_Invoice_Currency_code] [varchar](50) NULL,
	[Warehouse_Identification] [varchar](50) NULL,
	[Warehouse_Delay] [varchar](50) NULL,
	[Number_of_packages] [varchar](50) NULL,
	[Total_number_of_packages] [varchar](50) NULL,
	[Deffered_payment_reference] [varchar](50) NULL,
 CONSTRAINT [PK_ExportTemplate] PRIMARY KEY CLUSTERED 
(
	[ExportTemplate_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InventoryItems]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InventoryItems](
	[ItemNumber] [varchar](50) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[Category] [nvarchar](60) NULL,
	[TariffCode] [varchar](8) NULL,
	[EntryTimeStamp] [datetime] NULL,
 CONSTRAINT [PK_InventoryItems] PRIMARY KEY CLUSTERED 
(
	[ItemNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Licences]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Licences](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Year] [nvarchar](max) NOT NULL,
	[LicenceNumber] [nvarchar](max) NOT NULL,
	[QtyAllocated] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[TariffCateoryCode] [varchar](8) NULL,
	[AsycudaDocumentSetId] [int] NULL,
 CONSTRAINT [PK_Licences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sysdiagrams]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sysdiagrams](
	[name] [nvarchar](128) NOT NULL,
	[principal_id] [int] NOT NULL,
	[diagram_id] [int] IDENTITY(1,1) NOT NULL,
	[version] [int] NULL,
	[definition] [varbinary](max) NULL,
 CONSTRAINT [PK_sysdiagrams] PRIMARY KEY CLUSTERED 
(
	[diagram_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TariffCategory]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TariffCategory](
	[TariffCategoryCode] [varchar](8) NOT NULL,
	[Description] [varchar](999) NULL,
	[ParentTariffCategoryCode] [varchar](5) NULL,
	[LicenseRequired] [bit] NULL,
 CONSTRAINT [PK_TariffCategory] PRIMARY KEY CLUSTERED 
(
	[TariffCategoryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TariffCodes]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TariffCodes](
	[TariffCode] [varchar](8) NOT NULL,
	[Description] [varchar](999) NULL,
	[RateofDuty] [varchar](50) NULL,
	[EnvironmentalLevy] [varchar](50) NULL,
	[CustomsServiceCharge] [varchar](50) NULL,
	[ExciseTax] [varchar](50) NULL,
	[VatRate] [varchar](50) NULL,
	[PetrolTax] [varchar](50) NULL,
	[Units] [nvarchar](999) NULL,
	[SiteRev3] [varchar](50) NULL,
	[TariffCategoryCode] [varchar](8) NULL,
 CONSTRAINT [PK_TariffCodes] PRIMARY KEY CLUSTERED 
(
	[TariffCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TariffSupUnitLkps]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TariffSupUnitLkps](
	[TariffCategoryCode] [varchar](8) NOT NULL,
	[SuppUnitCode2] [nvarchar](50) NULL,
	[SuppUnitName2] [nvarchar](50) NULL,
	[SuppQty] [float] NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_TariffSupUnitLkps] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Assessment]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Assessment](
	[Number] [varchar](max) NULL,
	[Date] [varchar](max) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Assessment] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Assessment_notice]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Assessment_notice](
	[Assessment_notice_Id] [int] NOT NULL,
	[ASYCUDA_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Assessment_notice] PRIMARY KEY CLUSTERED 
(
	[Assessment_notice_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_ASYCUDA]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_ASYCUDA](
	[id] [varchar](max) NULL,
	[ASYCUDA_Id] [int] IDENTITY(1,1) NOT NULL,
	[EntryTimeStamp] [datetime] NULL,
 CONSTRAINT [PK_xcuda_ASYCUDA] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_ASYCUDA_ExtendedProperties]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties](
	[ASYCUDA_Id] [int] NOT NULL,
	[AsycudaDocumentSetId] [int] NULL,
	[FileNumber] [int] NULL,
	[IsManuallyAssessed] [bit] NULL,
	[CNumber] [varchar](50) NULL,
	[RegistrationDate] [datetime] NULL,
	[ReferenceNumber] [varchar](50) NULL,
	[Customs_ProcedureId] [int] NULL,
	[Document_Type_Id] [int] NULL,
	[Description] [nvarchar](max) NULL,
	[ExportTemplate_Id] [int] NULL,
	[BLNumber] [varchar](50) NULL,
	[AutoUpdate] [bit] NULL,
	[EffectiveRegistrationDate] [datetime] NULL,
 CONSTRAINT [PK_xcuda_ASYCUDA_xcuda_ExtendedProperty] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Attached_documents]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Attached_documents](
	[Attached_document_date] [nvarchar](max) NULL,
	[Attached_documents_Id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NULL,
	[Attached_document_code] [varchar](50) NULL,
	[Attached_document_name] [varchar](50) NULL,
	[Attached_document_reference] [varchar](50) NULL,
	[Attached_document_from_rule] [int] NULL,
 CONSTRAINT [PK_xcuda_Attached_documents] PRIMARY KEY CLUSTERED 
(
	[Attached_documents_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Border_information]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Border_information](
	[Border_information_Id] [int] IDENTITY(1,1) NOT NULL,
	[Means_of_transport_Id] [int] NULL,
	[Identity] [varchar](50) NULL,
	[Nationality] [varchar](50) NULL,
	[Mode] [int] NULL,
 CONSTRAINT [PK_xcuda_Border_information] PRIMARY KEY CLUSTERED 
(
	[Border_information_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Border_office]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Border_office](
	[Border_office_Id] [int] IDENTITY(1,1) NOT NULL,
	[Transport_Id] [int] NULL,
	[Code] [varchar](50) NULL,
	[Name] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Border_office] PRIMARY KEY CLUSTERED 
(
	[Border_office_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Consignee]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Consignee](
	[Traders_Id] [int] NOT NULL,
	[Consignee_code] [varchar](50) NULL,
	[Consignee_name] [varchar](255) NULL,
 CONSTRAINT [PK_xcuda_Consignee_1] PRIMARY KEY CLUSTERED 
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Container]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Container](
	[Item_Number] [varchar](max) NULL,
	[Container_identity] [varchar](max) NULL,
	[Container_type] [varchar](max) NULL,
	[Empty_full_indicator] [varchar](max) NULL,
	[Gross_weight] [real] NOT NULL,
	[Goods_description] [varchar](max) NULL,
	[Packages_type] [varchar](max) NULL,
	[Packages_number] [varchar](max) NULL,
	[Packages_weight] [real] NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[Container_Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_xcuda_Container] PRIMARY KEY CLUSTERED 
(
	[Container_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Country]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Country](
	[Country_first_destination] [varchar](max) NULL,
	[Country_of_origin_name] [varchar](max) NULL,
	[Country_Id] [int] NOT NULL,
	[Place_of_loading_Id] [int] NULL,
	[Trading_country] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Country] PRIMARY KEY CLUSTERED 
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Declarant]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Declarant](
	[Declarant_code] [varchar](max) NULL,
	[Declarant_name] [varchar](max) NULL,
	[Declarant_representative] [varchar](max) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[Number] [nvarchar](16) NULL,
 CONSTRAINT [PK_xcuda_Declarant] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Delivery_terms]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Delivery_terms](
	[Delivery_terms_Id] [int] IDENTITY(1,1) NOT NULL,
	[Transport_Id] [int] NULL,
	[Code] [varchar](50) NULL,
	[Place] [varchar](50) NULL,
	[Situation] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Delivery_terms] PRIMARY KEY CLUSTERED 
(
	[Delivery_terms_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Departure_arrival_information]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Departure_arrival_information](
	[Departure_arrival_information_Id] [int] IDENTITY(1,1) NOT NULL,
	[Means_of_transport_Id] [int] NULL,
	[Identity] [varchar](50) NULL,
	[Nationality] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Departure_arrival_information] PRIMARY KEY CLUSTERED 
(
	[Departure_arrival_information_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Destination]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Destination](
	[Destination_country_code] [varchar](max) NULL,
	[Destination_country_name] [varchar](max) NULL,
	[Country_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Destination_1] PRIMARY KEY CLUSTERED 
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Export]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Export](
	[Export_country_code] [varchar](50) NULL,
	[Export_country_name] [varchar](50) NULL,
	[Country_Id] [int] NOT NULL,
	[Export_country_region] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Export_1] PRIMARY KEY CLUSTERED 
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Export_release]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Export_release](
	[Date_of_exit] [varchar](max) NULL,
	[Time_of_exit] [varchar](max) NULL,
	[Export_release_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Export_release] PRIMARY KEY CLUSTERED 
(
	[Export_release_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Exporter]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Exporter](
	[Exporter_name] [varchar](max) NULL,
	[Traders_Id] [int] NOT NULL,
	[Exporter_code] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Exporter_1] PRIMARY KEY CLUSTERED 
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Financial]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Financial](
	[Financial_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[Deffered_payment_reference] [varchar](50) NULL,
	[Mode_of_payment] [varchar](50) NULL,
	[Financial_Code] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Financial] PRIMARY KEY CLUSTERED 
(
	[Financial_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Financial_Amounts]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Financial_Amounts](
	[Amounts_Id] [int] IDENTITY(1,1) NOT NULL,
	[Financial_Id] [int] NOT NULL,
	[Total_manual_taxes] [decimal](15, 4) NULL,
	[Global_taxes] [decimal](15, 4) NULL,
	[Totals_taxes] [decimal](15, 4) NULL,
 CONSTRAINT [PK_xcuda_Financial_Amounts] PRIMARY KEY CLUSTERED 
(
	[Amounts_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_Financial_Guarantee]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Financial_Guarantee](
	[Guarantee_Id] [int] IDENTITY(1,1) NOT NULL,
	[Financial_Id] [int] NOT NULL,
	[Amount] [decimal](15, 4) NULL,
	[Date] [datetime] NULL,
 CONSTRAINT [PK_xcuda_Financial_Guarantee] PRIMARY KEY CLUSTERED 
(
	[Guarantee_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_Forms]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Forms](
	[ASYCUDA_Id] [int] NOT NULL,
	[Number_of_the_form] [int] NULL,
	[Total_number_of_forms] [int] NULL,
 CONSTRAINT [PK_xcuda_Forms] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_General_information]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_General_information](
	[Value_details] [varchar](max) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[CAP] [varchar](max) NULL,
	[Additional_information] [varchar](max) NULL,
	[Comments_free_text] [nvarchar](max) NULL,
 CONSTRAINT [PK_xcuda_General_information_1] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Global_taxes]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Global_taxes](
	[Global_taxes_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Global_taxes] PRIMARY KEY CLUSTERED 
(
	[Global_taxes_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_Goods_description]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Goods_description](
	[Country_of_origin_code] [varchar](max) NULL,
	[Description_of_goods] [varchar](max) NULL,
	[Commercial_Description] [varchar](max) NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Goods_description] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Gs_deduction]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Gs_deduction](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_rate] [real] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Gs_deduction_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Gs_external_freight]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Gs_external_freight](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_code] [varchar](50) NULL,
	[Currency_rate] [real] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Gs_external_freight_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Gs_insurance]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Gs_insurance](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_rate] [real] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Gs_insurance_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Gs_internal_freight]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Gs_internal_freight](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_rate] [real] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Gs_internal_freight_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Gs_Invoice]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Gs_Invoice](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_code] [varchar](50) NULL,
	[Currency_rate] [real] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
	[Currency_name] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Gs_Invoice_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Gs_other_cost]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Gs_other_cost](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_rate] [real] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Gs_other_cost_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_HScode]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_HScode](
	[Commodity_code] [varchar](8) NOT NULL,
	[Precision_1] [varchar](max) NULL,
	[Precision_4] [varchar](50) NOT NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_HScode] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Identification]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Identification](
	[ASYCUDA_Id] [int] NOT NULL,
	[Manifest_reference_number] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Identification] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Inventory_Item]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Inventory_Item](
	[ItemNumber] [varchar](50) NOT NULL,
	[id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Inventory_Item] PRIMARY KEY CLUSTERED 
(
	[ItemNumber] ASC,
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Item]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Item](
	[Amount_deducted_from_licence] [varchar](max) NULL,
	[Quantity_deducted_from_licence] [varchar](max) NULL,
	[Item_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[Licence_number] [nvarchar](max) NULL,
	[Free_text_1] [nvarchar](max) NULL,
	[Free_text_2] [nvarchar](max) NULL,
	[EntryDataDetailsId] [int] NULL,
	[LineNumber] [int] NOT NULL,
	[IsAssessed] [bit] NULL,
	[QtyAllocated] [float] NULL,
	[EntryTimeStamp] [datetime] NULL,
 CONSTRAINT [PK_xcuda_Item] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_item_deduction]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_item_deduction](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_rate] [varchar](max) NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_item_deduction_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_item_external_freight]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_item_external_freight](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_rate] [real] NOT NULL,
	[Currency_code] [varchar](max) NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_item_external_freight_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_item_insurance]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_item_insurance](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_rate] [varchar](max) NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_item_insurance_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_item_internal_freight]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_item_internal_freight](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_rate] [varchar](max) NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_item_internal_freight_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Item_Invoice]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Item_Invoice](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_code] [varchar](max) NULL,
	[Currency_rate] [real] NOT NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Item_Invoice_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_item_other_cost]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_item_other_cost](
	[Amount_national_currency] [real] NOT NULL,
	[Amount_foreign_currency] [real] NOT NULL,
	[Currency_name] [varchar](max) NULL,
	[Currency_rate] [varchar](max) NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_item_other_cost_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Market_valuer]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Market_valuer](
	[Rate] [varchar](max) NULL,
	[Currency_amount] [real] NOT NULL,
	[Basis_amount] [varchar](max) NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Market_valuer_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Means_of_transport]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Means_of_transport](
	[Means_of_transport_Id] [int] IDENTITY(1,1) NOT NULL,
	[Transport_Id] [int] NULL,
	[Inland_mode_of_transport] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Means_of_transport] PRIMARY KEY CLUSTERED 
(
	[Means_of_transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Nbers]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Nbers](
	[Number_of_loading_lists] [varchar](max) NULL,
	[Total_number_of_items] [varchar](max) NULL,
	[Total_number_of_packages] [real] NOT NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Nbers] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Office_segment]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Office_segment](
	[Customs_clearance_office_code] [varchar](max) NULL,
	[Customs_Clearance_office_name] [varchar](max) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Office_segment] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Packages]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Packages](
	[Number_of_packages] [real] NOT NULL,
	[Kind_of_packages_code] [varchar](max) NULL,
	[Kind_of_packages_name] [varchar](max) NULL,
	[Packages_Id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NULL,
	[Marks1_of_packages] [nvarchar](max) NULL,
	[Marks2_of_packages] [nvarchar](max) NULL,
 CONSTRAINT [PK_xcuda_Packages] PRIMARY KEY CLUSTERED 
(
	[Packages_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Place_of_loading]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Place_of_loading](
	[Place_of_loading_Id] [int] IDENTITY(1,1) NOT NULL,
	[Transport_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Place_of_loading] PRIMARY KEY CLUSTERED 
(
	[Place_of_loading_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_Previous_doc]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Previous_doc](
	[Summary_declaration] [varchar](max) NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Previous_doc_1] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_PreviousItem]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_PreviousItem](
	[Packages_number] [varchar](max) NULL,
	[Previous_Packages_number] [varchar](max) NULL,
	[Hs_code] [varchar](max) NULL,
	[Commodity_code] [varchar](max) NULL,
	[Previous_item_number] [varchar](max) NULL,
	[Goods_origin] [varchar](max) NULL,
	[Net_weight] [real] NOT NULL,
	[Prev_net_weight] [real] NOT NULL,
	[Prev_reg_ser] [varchar](max) NULL,
	[Prev_reg_nbr] [varchar](max) NULL,
	[Prev_reg_dat] [varchar](max) NULL,
	[Prev_reg_cuo] [varchar](max) NULL,
	[Suplementary_Quantity] [real] NOT NULL,
	[Preveious_suplementary_quantity] [real] NOT NULL,
	[Current_value] [real] NOT NULL,
	[Previous_value] [real] NOT NULL,
	[Current_item_number] [varchar](max) NULL,
	[PreviousItem_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_PreviousItem] PRIMARY KEY CLUSTERED 
(
	[PreviousItem_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Principal]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Principal](
	[Principal_Id] [int] NOT NULL,
	[Transit_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Principal] PRIMARY KEY CLUSTERED 
(
	[Principal_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_Property]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Property](
	[Sad_flow] [varchar](max) NULL,
	[Date_of_declaration] [varchar](max) NULL,
	[Selected_page] [varchar](max) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[Place_of_declaration] [varchar](max) NULL,
 CONSTRAINT [PK_xcuda_Property] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_receipt]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_receipt](
	[Number] [varchar](max) NULL,
	[Date] [varchar](max) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_receipt] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Registration]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Registration](
	[Number] [varchar](max) NULL,
	[Date] [varchar](max) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Registration] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Seals]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Seals](
	[Number] [varchar](max) NULL,
	[Seals_Id] [int] NOT NULL,
	[Transit_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Seals] PRIMARY KEY CLUSTERED 
(
	[Seals_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Signature]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Signature](
	[Date] [varchar](max) NULL,
	[Signature_Id] [int] NOT NULL,
	[Transit_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Signature] PRIMARY KEY CLUSTERED 
(
	[Signature_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Supplementary_unit]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Supplementary_unit](
	[Suppplementary_unit_quantity] [varchar](max) NULL,
	[Supplementary_unit_Id] [int] IDENTITY(1,1) NOT NULL,
	[Tarification_Id] [int] NULL,
	[Suppplementary_unit_code] [nvarchar](max) NULL,
	[Suppplementary_unit_name] [nvarchar](max) NULL,
 CONSTRAINT [PK_xcuda_Supplementary_unit] PRIMARY KEY CLUSTERED 
(
	[Supplementary_unit_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Suppliers_documents]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Suppliers_documents](
	[Suppliers_document_date] [varchar](max) NULL,
	[Suppliers_documents_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[Suppliers_document_itmlink] [nvarchar](max) NULL,
	[Suppliers_document_code] [nvarchar](max) NULL,
	[Suppliers_document_name] [nvarchar](max) NULL,
	[Suppliers_document_country] [nvarchar](max) NULL,
	[Suppliers_document_city] [nvarchar](max) NULL,
	[Suppliers_document_street] [nvarchar](max) NULL,
	[Suppliers_document_telephone] [nvarchar](max) NULL,
	[Suppliers_document_fax] [nvarchar](max) NULL,
	[Suppliers_document_zip_code] [nvarchar](max) NULL,
	[Suppliers_document_invoice_nbr] [nvarchar](max) NULL,
	[Suppliers_document_invoice_amt] [nvarchar](max) NULL,
	[Suppliers_document_type_code] [nvarchar](max) NULL,
 CONSTRAINT [PK_xcuda_Suppliers_documents] PRIMARY KEY CLUSTERED 
(
	[Suppliers_documents_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Suppliers_link]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Suppliers_link](
	[Suppliers_link_code] [varchar](50) NULL,
	[Item_Id] [int] NULL,
	[Suppliers_link_Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_xcuda_Suppliers_link] PRIMARY KEY CLUSTERED 
(
	[Suppliers_link_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Tarification]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Tarification](
	[Extended_customs_procedure] [varchar](max) NULL,
	[National_customs_procedure] [varchar](max) NULL,
	[Item_price] [real] NOT NULL,
	[Item_Id] [int] NOT NULL,
	[Value_item] [nvarchar](max) NULL,
	[Attached_doc_item] [nvarchar](max) NULL,
 CONSTRAINT [PK_xcuda_Tarification] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Taxation]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Taxation](
	[Item_taxes_amount] [real] NOT NULL,
	[Item_taxes_guaranted_amount] [varchar](max) NULL,
	[Counter_of_normal_mode_of_payment] [varchar](max) NULL,
	[Displayed_item_taxes_amount] [varchar](max) NULL,
	[Taxation_Id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NULL,
	[Item_taxes_mode_of_payment] [nvarchar](max) NULL,
 CONSTRAINT [PK_xcuda_Taxation] PRIMARY KEY CLUSTERED 
(
	[Taxation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Taxation_line]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Taxation_line](
	[Duty_tax_Base] [varchar](max) NULL,
	[Duty_tax_rate] [varchar](max) NULL,
	[Duty_tax_amount] [varchar](max) NULL,
	[Taxation_line_Id] [int] IDENTITY(1,1) NOT NULL,
	[Taxation_Id] [int] NULL,
	[Duty_tax_code] [nvarchar](max) NULL,
	[Duty_tax_MP] [nvarchar](max) NULL,
 CONSTRAINT [PK_xcuda_Taxation_line] PRIMARY KEY CLUSTERED 
(
	[Taxation_line_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Total]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Total](
	[Total_invoice] [real] NOT NULL,
	[Total_weight] [real] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Total_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_Traders]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Traders](
	[Traders_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Traders] PRIMARY KEY CLUSTERED 
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[xcuda_Traders_Financial]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Traders_Financial](
	[Traders_Id] [int] NOT NULL,
	[Financial_code] [varchar](50) NULL,
	[Financial_name] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_xcuda_Traders_Financial_1] PRIMARY KEY CLUSTERED 
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Transit]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Transit](
	[Time_limit] [varchar](max) NULL,
	[Transit_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Transit] PRIMARY KEY CLUSTERED 
(
	[Transit_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Transit_Destination]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Transit_Destination](
	[Destination_Id] [int] IDENTITY(1,1) NOT NULL,
	[Office] [varchar](max) NULL,
	[Country] [varchar](max) NULL,
	[Transit_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Transit_Destination] PRIMARY KEY CLUSTERED 
(
	[Destination_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Transport]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Transport](
	[Container_flag] [bit] NOT NULL,
	[Single_waybill_flag] [bit] NOT NULL,
	[Transport_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[Location_of_goods] [varchar](50) NULL,
 CONSTRAINT [PK_xcuda_Transport] PRIMARY KEY CLUSTERED 
(
	[Transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Type]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Type](
	[Type_of_declaration] [varchar](max) NULL,
	[Declaration_gen_procedure_code] [varchar](max) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Type] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Valuation]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Valuation](
	[Calculation_working_mode] [varchar](max) NULL,
	[Total_cost] [real] NOT NULL,
	[Total_CIF] [real] NOT NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Valuation_1] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Valuation_item]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Valuation_item](
	[Total_cost_itm] [real] NOT NULL,
	[Total_CIF_itm] [real] NOT NULL,
	[Rate_of_adjustement] [varchar](max) NULL,
	[Statistical_value] [real] NOT NULL,
	[Alpha_coeficient_of_apportionment] [varchar](max) NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Valuation_item] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Warehouse]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Warehouse](
	[Identification] [varchar](50) NULL,
	[Delay] [varchar](50) NULL,
	[ASYCUDA_Id] [int] NULL,
	[Warehouse_Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_xcuda_Warehouse] PRIMARY KEY CLUSTERED 
(
	[Warehouse_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Weight]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[xcuda_Weight](
	[Gross_weight] [varchar](max) NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Weight_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[xcuda_Weight_itm]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Weight_itm](
	[Gross_weight_itm] [real] NOT NULL,
	[Net_weight_itm] [real] NOT NULL,
	[Valuation_item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Weight_itm_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[CounterPointSalesDetails]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CounterPointSalesDetails]
AS
SELECT PS_TKT_HIST_1.TKT_NO AS INVNO, PS_TKT_HIST_LIN_1.LIN_SEQ_NO AS SEQ_NO, PS_TKT_HIST_LIN_1.ITEM_NO, 
                  PS_TKT_HIST_LIN_1.DESCR AS ITEM_DESCR, PS_TKT_HIST_LIN_1.QTY_SOLD AS QUANTITY, PS_TKT_HIST_LIN_1.UNIT_COST AS COST, 
                  PS_TKT_HIST_1.CUST_NO AS [ACCT NO], AR_CUST_1.NAM + '|' + COALESCE (PS_TKT_HIST_CONTACT_1.NAM, AR_CUST_1.ADRS_1) AS [CUSTOMER NAME], 
                  PS_TKT_HIST_1.TKT_DT AS DATE, PS_TKT_HIST_1.TAX_AMT, PS_TKT_HIST_LIN_1.UNIT_WEIGHT, PS_TKT_HIST_LIN_1.QTY_UNIT
FROM     IWWGNDSRV.IWW.dbo.PS_TKT_HIST AS PS_TKT_HIST_1 LEFT OUTER JOIN
                  IWWGNDSRV.IWW.dbo.PS_TKT_HIST_LIN AS PS_TKT_HIST_LIN_1 ON PS_TKT_HIST_1.DOC_ID = PS_TKT_HIST_LIN_1.DOC_ID AND 
                  PS_TKT_HIST_1.BUS_DAT = PS_TKT_HIST_LIN_1.BUS_DAT LEFT OUTER JOIN
                  IWWGNDSRV.IWW.dbo.PS_TKT_HIST_CONTACT AS PS_TKT_HIST_CONTACT_1 ON PS_TKT_HIST_1.BUS_DAT = PS_TKT_HIST_CONTACT_1.BUS_DAT AND 
                  PS_TKT_HIST_1.DOC_ID = PS_TKT_HIST_CONTACT_1.DOC_ID AND 
                  PS_TKT_HIST_1.SHIP_TO_CONTACT_ID = PS_TKT_HIST_CONTACT_1.CONTACT_ID LEFT OUTER JOIN
                  IWWGNDSRV.IWW.dbo.AR_CUST AS AR_CUST_1 ON PS_TKT_HIST_1.CUST_NO = AR_CUST_1.CUST_NO
WHERE  (PS_TKT_HIST_LIN_1.UNIT_COST IS NOT NULL)

GO
/****** Object:  View [dbo].[CounterPointSales]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CounterPointSales]
AS
SELECT INVNO, DATE, TAX_AMT, [CUSTOMER NAME], COUNT(INVNO) AS LIN_CNT
FROM     dbo.CounterPointSalesDetails AS Sales
GROUP BY INVNO, DATE, TAX_AMT, [CUSTOMER NAME]

GO
/****** Object:  View [dbo].[AsycudaDocumentSetPreviousDocuments]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentSetPreviousDocuments]
AS
SELECT DISTINCT dbo.AsycudaDocumentSet.AsycudaDocumentSetId, dbo.xcuda_ASYCUDA.ASYCUDA_Id
FROM     dbo.AsycudaSalesAllocations INNER JOIN
                  dbo.EntryDataDetails ON dbo.AsycudaSalesAllocations.EntryDataDetailsId = dbo.EntryDataDetails.EntryDataDetailsId INNER JOIN
                  dbo.xcuda_Item ON dbo.AsycudaSalesAllocations.PreviousItem_Id = dbo.xcuda_Item.Item_Id INNER JOIN
                  dbo.xcuda_ASYCUDA ON dbo.xcuda_Item.ASYCUDA_Id = dbo.xcuda_ASYCUDA.ASYCUDA_Id INNER JOIN
                  dbo.AsycudaDocumentSet INNER JOIN
                  dbo.EntryData ON dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.EntryData.AsycudaDocumentSetId INNER JOIN
                  dbo.EntryData_Sales ON dbo.EntryData.EntryDataId = dbo.EntryData_Sales.EntryDataId ON 
                  dbo.EntryDataDetails.EntryDataId = dbo.EntryData_Sales.EntryDataId

GO
/****** Object:  View [dbo].[AsycudaDocumentSetPreviousEntries]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentSetPreviousEntries]
AS
SELECT DISTINCT dbo.AsycudaDocumentSet.AsycudaDocumentSetId, dbo.xcuda_ASYCUDA.ASYCUDA_Id, dbo.xcuda_Item.Item_Id
FROM     dbo.AsycudaSalesAllocations INNER JOIN
                  dbo.EntryDataDetails ON dbo.AsycudaSalesAllocations.EntryDataDetailsId = dbo.EntryDataDetails.EntryDataDetailsId INNER JOIN
                  dbo.xcuda_Item ON dbo.AsycudaSalesAllocations.PreviousItem_Id = dbo.xcuda_Item.Item_Id INNER JOIN
                  dbo.xcuda_ASYCUDA ON dbo.xcuda_Item.ASYCUDA_Id = dbo.xcuda_ASYCUDA.ASYCUDA_Id INNER JOIN
                  dbo.AsycudaDocumentSet INNER JOIN
                  dbo.EntryData ON dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.EntryData.AsycudaDocumentSetId INNER JOIN
                  dbo.EntryData_Sales ON dbo.EntryData.EntryDataId = dbo.EntryData_Sales.EntryDataId ON 
                  dbo.EntryDataDetails.EntryDataId = dbo.EntryData_Sales.EntryDataId

GO
/****** Object:  View [dbo].[AsycudaEntries]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaEntries]
AS
SELECT Item.Item_Id, Item.ASYCUDA_Id, Item.LineNumber, Item.QtyAllocated, Supp.Suppplementary_unit_quantity AS ItemQuantity, dbo.InventoryItems.ItemNumber, 
                  dbo.InventoryItems.TariffCode, Declarant.Number AS DeclarantReferenceNumber, Tar.Item_price AS ItemPrice, 
                  CASE WHEN IsManuallyAssessed = 1 THEN Ext.Cnumber ELSE Reg.Number END AS CNumber, 
                  CASE WHEN IsManuallyAssessed = 1 THEN Ext.RegistrationDate ELSE Reg.Date END AS RegistrationDate, Ext.EffectiveRegistrationDate, HS.Item_Id AS Expr1, 
                  HS.Precision_4
FROM     dbo.xcuda_ASYCUDA_ExtendedProperties AS Ext INNER JOIN
                  dbo.xcuda_Identification AS Iden INNER JOIN
                  dbo.xcuda_Registration AS Reg ON Iden.ASYCUDA_Id = Reg.ASYCUDA_Id INNER JOIN
                  dbo.xcuda_ASYCUDA ON Iden.ASYCUDA_Id = dbo.xcuda_ASYCUDA.ASYCUDA_Id ON Ext.ASYCUDA_Id = dbo.xcuda_ASYCUDA.ASYCUDA_Id INNER JOIN
                  dbo.xcuda_Declarant AS Declarant ON dbo.xcuda_ASYCUDA.ASYCUDA_Id = Declarant.ASYCUDA_Id INNER JOIN
                  dbo.xcuda_Inventory_Item INNER JOIN
                  dbo.InventoryItems ON dbo.xcuda_Inventory_Item.ItemNumber = dbo.InventoryItems.ItemNumber INNER JOIN
                  dbo.xcuda_HScode AS HS ON dbo.xcuda_Inventory_Item.id = HS.Item_Id INNER JOIN
                  dbo.xcuda_Tarification AS Tar INNER JOIN
                  dbo.xcuda_Supplementary_unit AS Supp ON Tar.Item_Id = Supp.Tarification_Id ON HS.Item_Id = Tar.Item_Id INNER JOIN
                  dbo.xcuda_Item AS Item ON Tar.Item_Id = Item.Item_Id ON dbo.xcuda_ASYCUDA.ASYCUDA_Id = Item.ASYCUDA_Id
WHERE  (Supp.Suppplementary_unit_code = N'NMB')
GROUP BY Item.Item_Id, Item.ASYCUDA_Id, Item.LineNumber, Item.QtyAllocated, Supp.Suppplementary_unit_quantity, dbo.InventoryItems.ItemNumber, 
                  dbo.InventoryItems.TariffCode, Declarant.Number, Tar.Item_price, CASE WHEN IsManuallyAssessed = 1 THEN Ext.Cnumber ELSE Reg.Number END, 
                  CASE WHEN IsManuallyAssessed = 1 THEN Ext.RegistrationDate ELSE Reg.Date END, Ext.EffectiveRegistrationDate, HS.Item_Id, HS.Precision_4

GO
/****** Object:  View [dbo].[CounterPointPODetails]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CounterPointPODetails]
AS
SELECT PO_NO, SEQ_NO, ITEM_NO, ORD_QTY, ORD_UNIT, ITEM_DESCR, ORD_COST, UNIT_WEIGHT
FROM     IWWGNDSRV.IWW.dbo.PO_ORD_LIN AS PO_ORD_LIN_1

GO
/****** Object:  View [dbo].[CounterPointPOs]    Script Date: 6/11/2013 9:44:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CounterPointPOs]
AS
SELECT PO_NO, ORD_DAT AS DATE, LIN_CNT
FROM     IWWGNDSRV.IWW.dbo.PO_ORD_HDR AS PO_ORD_HDR_1
GROUP BY PO_NO, ORD_DAT, LIN_CNT

GO
/****** Object:  Index [IX_FK_Customs_ProcedureDocumentType]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Customs_ProcedureDocumentType] ON [dbo].[Document_Type]
(
	[DefaultCustoms_ProcedureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FK_InventoryItemPurchaseOrderDetail]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_InventoryItemPurchaseOrderDetail] ON [dbo].[EntryDataDetails]
(
	[ItemNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FK_PurchaseOrderPurchaseOrderDetail]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_PurchaseOrderPurchaseOrderDetail] ON [dbo].[EntryDataDetails]
(
	[EntryDataId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FK_TariffCodesInventoryItem]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_TariffCodesInventoryItem] ON [dbo].[InventoryItems]
(
	[TariffCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FK_TariffCategoryTariffCodes]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_TariffCategoryTariffCodes] ON [dbo].[TariffCodes]
(
	[TariffCategoryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_FK_TariffCategoryTariffSupUnitLkp]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_TariffCategoryTariffSupUnitLkp] ON [dbo].[TariffSupUnitLkps]
(
	[TariffCategoryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Assessment_notice]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Assessment_notice] ON [dbo].[xcuda_Assessment_notice]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Item_Attached_documents]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Item_Attached_documents] ON [dbo].[xcuda_Attached_documents]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Means_of_transport_Border_information]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Means_of_transport_Border_information] ON [dbo].[xcuda_Border_information]
(
	[Means_of_transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Transport_Border_office]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Transport_Border_office] ON [dbo].[xcuda_Border_office]
(
	[Transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Traders_Consignee]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Traders_Consignee] ON [dbo].[xcuda_Consignee]
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Container]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Container] ON [dbo].[xcuda_Container]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Place_of_loading_Country]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Place_of_loading_Country] ON [dbo].[xcuda_Country]
(
	[Place_of_loading_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Transport_Delivery_terms]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Transport_Delivery_terms] ON [dbo].[xcuda_Delivery_terms]
(
	[Transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Means_of_transport_Departure_arrival_information]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Means_of_transport_Departure_arrival_information] ON [dbo].[xcuda_Departure_arrival_information]
(
	[Means_of_transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Country_Destination]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Country_Destination] ON [dbo].[xcuda_Destination]
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Country_Export]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Country_Export] ON [dbo].[xcuda_Export]
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_xcuda_Export_xcuda_Country]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_xcuda_Export_xcuda_Country] ON [dbo].[xcuda_Export]
(
	[Country_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Export_release]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Export_release] ON [dbo].[xcuda_Export_release]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Traders_Exporter]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Traders_Exporter] ON [dbo].[xcuda_Exporter]
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Financial]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Financial] ON [dbo].[xcuda_Financial]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_xcuda_Financialxcuda_Financial_Amounts]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_xcuda_Financialxcuda_Financial_Amounts] ON [dbo].[xcuda_Financial_Amounts]
(
	[Financial_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_xcuda_Financialxcuda_Financial_Guarantee]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_xcuda_Financialxcuda_Financial_Guarantee] ON [dbo].[xcuda_Financial_Guarantee]
(
	[Financial_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_General_information]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_General_information] ON [dbo].[xcuda_General_information]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Global_taxes]    Script Date: 6/11/2013 9:44:44 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Global_taxes] ON [dbo].[xcuda_Global_taxes]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_Gs_deduction]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_Gs_deduction] ON [dbo].[xcuda_Gs_deduction]
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_Gs_external_freight]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_Gs_external_freight] ON [dbo].[xcuda_Gs_external_freight]
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_Gs_insurance]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_Gs_insurance] ON [dbo].[xcuda_Gs_insurance]
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_Gs_internal_freight]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_Gs_internal_freight] ON [dbo].[xcuda_Gs_internal_freight]
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_Gs_Invoice]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_Gs_Invoice] ON [dbo].[xcuda_Gs_Invoice]
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_Gs_other_cost]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_Gs_other_cost] ON [dbo].[xcuda_Gs_other_cost]
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Item]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Item] ON [dbo].[xcuda_Item]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_PurchaseOrderDetailxcuda_Item]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_PurchaseOrderDetailxcuda_Item] ON [dbo].[xcuda_Item]
(
	[EntryDataDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_item_item_deduction]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_item_item_deduction] ON [dbo].[xcuda_item_deduction]
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_item_item_external_freight]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_item_item_external_freight] ON [dbo].[xcuda_item_external_freight]
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_item_item_insurance]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_item_item_insurance] ON [dbo].[xcuda_item_insurance]
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_item_item_internal_freight]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_item_item_internal_freight] ON [dbo].[xcuda_item_internal_freight]
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_item_Item_Invoice]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_item_Item_Invoice] ON [dbo].[xcuda_Item_Invoice]
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_item_item_other_cost]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_item_item_other_cost] ON [dbo].[xcuda_item_other_cost]
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_item_Market_valuer]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_item_Market_valuer] ON [dbo].[xcuda_Market_valuer]
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Transport_Means_of_transport]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Transport_Means_of_transport] ON [dbo].[xcuda_Means_of_transport]
(
	[Transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Item_Packages]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Item_Packages] ON [dbo].[xcuda_Packages]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Transport_Place_of_loading]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Transport_Place_of_loading] ON [dbo].[xcuda_Place_of_loading]
(
	[Transport_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Item_Previous_doc]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Item_Previous_doc] ON [dbo].[xcuda_Previous_doc]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_PreviousItem]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_PreviousItem] ON [dbo].[xcuda_PreviousItem]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Transit_Principal]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Transit_Principal] ON [dbo].[xcuda_Principal]
(
	[Transit_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Transit_Seals]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Transit_Seals] ON [dbo].[xcuda_Seals]
(
	[Transit_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Transit_Signature]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Transit_Signature] ON [dbo].[xcuda_Signature]
(
	[Transit_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Tarification_Supplementary_unit]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Tarification_Supplementary_unit] ON [dbo].[xcuda_Supplementary_unit]
(
	[Tarification_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Suppliers_documents]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Suppliers_documents] ON [dbo].[xcuda_Suppliers_documents]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Item_Suppliers_link]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Item_Suppliers_link] ON [dbo].[xcuda_Suppliers_link]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Item_Taxation]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Item_Taxation] ON [dbo].[xcuda_Taxation]
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Taxation_Taxation_line]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Taxation_Taxation_line] ON [dbo].[xcuda_Taxation_line]
(
	[Taxation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_Total]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_Total] ON [dbo].[xcuda_Total]
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Traders_Financial]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Traders_Financial] ON [dbo].[xcuda_Traders_Financial]
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Transit]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Transit] ON [dbo].[xcuda_Transit]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Transport]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Transport] ON [dbo].[xcuda_Transport]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Valuation]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Valuation] ON [dbo].[xcuda_Valuation]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_ASYCUDA_Warehouse]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_ASYCUDA_Warehouse] ON [dbo].[xcuda_Warehouse]
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_FK_Valuation_item_Weight_itm]    Script Date: 6/11/2013 9:44:45 PM ******/
CREATE NONCLUSTERED INDEX [IX_FK_Valuation_item_Weight_itm] ON [dbo].[xcuda_Weight_itm]
(
	[Valuation_item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] ADD  CONSTRAINT [DF_AsycudaDocumentSet_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations] ADD  CONSTRAINT [DF_AsycudaSalesAllocations_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[InventoryItems] ADD  CONSTRAINT [DF_InventoryItems_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA] ADD  CONSTRAINT [DF_xcuda_ASYCUDA_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] ADD  CONSTRAINT [DF_xcuda_ASYCUDA_ExtendedProperties_AutoUpdate]  DEFAULT ((1)) FOR [AutoUpdate]
GO
ALTER TABLE [dbo].[xcuda_Item] ADD  CONSTRAINT [DF_xcuda_Item_EntryTimeStamp]  DEFAULT (sysutcdatetime()) FOR [EntryTimeStamp]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_Customs_Procedure] FOREIGN KEY([Customs_ProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] CHECK CONSTRAINT [FK_AsycudaDocumentSet_Customs_Procedure]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_Document_Type] FOREIGN KEY([Document_Type_Id])
REFERENCES [dbo].[Document_Type] ([Document_Type_Id])
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] CHECK CONSTRAINT [FK_AsycudaDocumentSet_Document_Type]
GO
ALTER TABLE [dbo].[AsycudaDocumentSet]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaDocumentSet_ExportTemplate] FOREIGN KEY([ExportTemplate_Id])
REFERENCES [dbo].[ExportTemplate] ([ExportTemplate_Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[AsycudaDocumentSet] CHECK CONSTRAINT [FK_AsycudaDocumentSet_ExportTemplate]
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaSalesAllocations_EntryDataDetails] FOREIGN KEY([EntryDataDetailsId])
REFERENCES [dbo].[EntryDataDetails] ([EntryDataDetailsId])
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations] CHECK CONSTRAINT [FK_AsycudaSalesAllocations_EntryDataDetails]
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaSalesAllocations_xcuda_Item] FOREIGN KEY([PreviousItem_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations] CHECK CONSTRAINT [FK_AsycudaSalesAllocations_xcuda_Item]
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations]  WITH CHECK ADD  CONSTRAINT [FK_AsycudaSalesAllocations_xcuda_Item1] FOREIGN KEY([xEntryItem_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
GO
ALTER TABLE [dbo].[AsycudaSalesAllocations] CHECK CONSTRAINT [FK_AsycudaSalesAllocations_xcuda_Item1]
GO
ALTER TABLE [dbo].[Customs_Procedure]  WITH NOCHECK ADD  CONSTRAINT [FK_Customs_Procedure_Document_Type] FOREIGN KEY([Document_Type_Id])
REFERENCES [dbo].[Document_Type] ([Document_Type_Id])
GO
ALTER TABLE [dbo].[Customs_Procedure] CHECK CONSTRAINT [FK_Customs_Procedure_Document_Type]
GO
ALTER TABLE [dbo].[Document_Type]  WITH CHECK ADD  CONSTRAINT [FK_Customs_ProcedureDocumentType] FOREIGN KEY([DefaultCustoms_ProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Document_Type] CHECK CONSTRAINT [FK_Customs_ProcedureDocumentType]
GO
ALTER TABLE [dbo].[EntryData]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrders_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryData] CHECK CONSTRAINT [FK_PurchaseOrders_AsycudaDocumentSet]
GO
ALTER TABLE [dbo].[EntryDataDetails]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderDetails_InventoryItems] FOREIGN KEY([ItemNumber])
REFERENCES [dbo].[InventoryItems] ([ItemNumber])
GO
ALTER TABLE [dbo].[EntryDataDetails] CHECK CONSTRAINT [FK_PurchaseOrderDetails_InventoryItems]
GO
ALTER TABLE [dbo].[EntryDataDetails]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderDetails_PurchaseOrders] FOREIGN KEY([EntryDataId])
REFERENCES [dbo].[EntryData] ([EntryDataId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryDataDetails] CHECK CONSTRAINT [FK_PurchaseOrderDetails_PurchaseOrders]
GO
ALTER TABLE [dbo].[InventoryItems]  WITH NOCHECK ADD  CONSTRAINT [FK_InventoryItems_TariffCodes] FOREIGN KEY([TariffCode])
REFERENCES [dbo].[TariffCodes] ([TariffCode])
GO
ALTER TABLE [dbo].[InventoryItems] CHECK CONSTRAINT [FK_InventoryItems_TariffCodes]
GO
ALTER TABLE [dbo].[Licences]  WITH NOCHECK ADD  CONSTRAINT [FK_Licences_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Licences] CHECK CONSTRAINT [FK_Licences_AsycudaDocumentSet]
GO
ALTER TABLE [dbo].[TariffCodes]  WITH NOCHECK ADD  CONSTRAINT [FK_TariffCodes_TariffCategory] FOREIGN KEY([TariffCategoryCode])
REFERENCES [dbo].[TariffCategory] ([TariffCategoryCode])
GO
ALTER TABLE [dbo].[TariffCodes] CHECK CONSTRAINT [FK_TariffCodes_TariffCategory]
GO
ALTER TABLE [dbo].[TariffSupUnitLkps]  WITH CHECK ADD  CONSTRAINT [FK_TariffCategoryTariffSupUnitLkp] FOREIGN KEY([TariffCategoryCode])
REFERENCES [dbo].[TariffCategory] ([TariffCategoryCode])
GO
ALTER TABLE [dbo].[TariffSupUnitLkps] CHECK CONSTRAINT [FK_TariffCategoryTariffSupUnitLkp]
GO
ALTER TABLE [dbo].[xcuda_Assessment]  WITH CHECK ADD  CONSTRAINT [FK_Identification_Assessment] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Assessment] CHECK CONSTRAINT [FK_Identification_Assessment]
GO
ALTER TABLE [dbo].[xcuda_Assessment_notice]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Assessment_notice] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Assessment_notice] CHECK CONSTRAINT [FK_ASYCUDA_Assessment_notice]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_AsycudaDocumentSet]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_Customs_Procedure] FOREIGN KEY([Customs_ProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_Customs_Procedure]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_Document_Type] FOREIGN KEY([Document_Type_Id])
REFERENCES [dbo].[Document_Type] ([Document_Type_Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_Document_Type]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_ExportTemplate] FOREIGN KEY([ExportTemplate_Id])
REFERENCES [dbo].[ExportTemplate] ([ExportTemplate_Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ASYCUDA_ExtendedProperties_ExportTemplate]
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_ExtendedProperty_inherits_xcuda_ASYCUDA] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_ASYCUDA_ExtendedProperties] CHECK CONSTRAINT [FK_xcuda_ExtendedProperty_inherits_xcuda_ASYCUDA]
GO
ALTER TABLE [dbo].[xcuda_Attached_documents]  WITH CHECK ADD  CONSTRAINT [FK_Item_Attached_documents] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Attached_documents] CHECK CONSTRAINT [FK_Item_Attached_documents]
GO
ALTER TABLE [dbo].[xcuda_Border_information]  WITH CHECK ADD  CONSTRAINT [FK_Means_of_transport_Border_information] FOREIGN KEY([Means_of_transport_Id])
REFERENCES [dbo].[xcuda_Means_of_transport] ([Means_of_transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Border_information] CHECK CONSTRAINT [FK_Means_of_transport_Border_information]
GO
ALTER TABLE [dbo].[xcuda_Border_office]  WITH CHECK ADD  CONSTRAINT [FK_Transport_Border_office] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Border_office] CHECK CONSTRAINT [FK_Transport_Border_office]
GO
ALTER TABLE [dbo].[xcuda_Consignee]  WITH CHECK ADD  CONSTRAINT [FK_Traders_Consignee] FOREIGN KEY([Traders_Id])
REFERENCES [dbo].[xcuda_Traders] ([Traders_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Consignee] CHECK CONSTRAINT [FK_Traders_Consignee]
GO
ALTER TABLE [dbo].[xcuda_Container]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Container] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Container] CHECK CONSTRAINT [FK_ASYCUDA_Container]
GO
ALTER TABLE [dbo].[xcuda_Country]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Country_xcuda_General_information] FOREIGN KEY([Country_Id])
REFERENCES [dbo].[xcuda_General_information] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Country] CHECK CONSTRAINT [FK_xcuda_Country_xcuda_General_information]
GO
ALTER TABLE [dbo].[xcuda_Declarant]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Declarant] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Declarant] CHECK CONSTRAINT [FK_ASYCUDA_Declarant]
GO
ALTER TABLE [dbo].[xcuda_Delivery_terms]  WITH CHECK ADD  CONSTRAINT [FK_Transport_Delivery_terms] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Delivery_terms] CHECK CONSTRAINT [FK_Transport_Delivery_terms]
GO
ALTER TABLE [dbo].[xcuda_Departure_arrival_information]  WITH CHECK ADD  CONSTRAINT [FK_Means_of_transport_Departure_arrival_information] FOREIGN KEY([Means_of_transport_Id])
REFERENCES [dbo].[xcuda_Means_of_transport] ([Means_of_transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Departure_arrival_information] CHECK CONSTRAINT [FK_Means_of_transport_Departure_arrival_information]
GO
ALTER TABLE [dbo].[xcuda_Destination]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Destination_xcuda_Country] FOREIGN KEY([Country_Id])
REFERENCES [dbo].[xcuda_Country] ([Country_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Destination] CHECK CONSTRAINT [FK_xcuda_Destination_xcuda_Country]
GO
ALTER TABLE [dbo].[xcuda_Export]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Export_xcuda_Country] FOREIGN KEY([Country_Id])
REFERENCES [dbo].[xcuda_Country] ([Country_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Export] CHECK CONSTRAINT [FK_xcuda_Export_xcuda_Country]
GO
ALTER TABLE [dbo].[xcuda_Export_release]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Export_release] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Export_release] CHECK CONSTRAINT [FK_ASYCUDA_Export_release]
GO
ALTER TABLE [dbo].[xcuda_Exporter]  WITH CHECK ADD  CONSTRAINT [FK_Traders_Exporter] FOREIGN KEY([Traders_Id])
REFERENCES [dbo].[xcuda_Traders] ([Traders_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Exporter] CHECK CONSTRAINT [FK_Traders_Exporter]
GO
ALTER TABLE [dbo].[xcuda_Financial]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Financial] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Financial] CHECK CONSTRAINT [FK_ASYCUDA_Financial]
GO
ALTER TABLE [dbo].[xcuda_Financial_Amounts]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Financialxcuda_Financial_Amounts] FOREIGN KEY([Financial_Id])
REFERENCES [dbo].[xcuda_Financial] ([Financial_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Financial_Amounts] CHECK CONSTRAINT [FK_xcuda_Financialxcuda_Financial_Amounts]
GO
ALTER TABLE [dbo].[xcuda_Financial_Guarantee]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Financialxcuda_Financial_Guarantee] FOREIGN KEY([Financial_Id])
REFERENCES [dbo].[xcuda_Financial] ([Financial_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Financial_Guarantee] CHECK CONSTRAINT [FK_xcuda_Financialxcuda_Financial_Guarantee]
GO
ALTER TABLE [dbo].[xcuda_Forms]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Forms_xcuda_Property] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Property] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Forms] CHECK CONSTRAINT [FK_xcuda_Forms_xcuda_Property]
GO
ALTER TABLE [dbo].[xcuda_General_information]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_General_information] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_General_information] CHECK CONSTRAINT [FK_ASYCUDA_General_information]
GO
ALTER TABLE [dbo].[xcuda_Global_taxes]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Global_taxes] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Global_taxes] CHECK CONSTRAINT [FK_ASYCUDA_Global_taxes]
GO
ALTER TABLE [dbo].[xcuda_Goods_description]  WITH CHECK ADD  CONSTRAINT [FK_Item_Goods_description] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Goods_description] CHECK CONSTRAINT [FK_Item_Goods_description]
GO
ALTER TABLE [dbo].[xcuda_Gs_deduction]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Gs_deduction_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Gs_deduction] CHECK CONSTRAINT [FK_xcuda_Gs_deduction_xcuda_Valuation]
GO
ALTER TABLE [dbo].[xcuda_Gs_insurance]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Gs_insurance_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Gs_insurance] CHECK CONSTRAINT [FK_xcuda_Gs_insurance_xcuda_Valuation]
GO
ALTER TABLE [dbo].[xcuda_Gs_internal_freight]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Gs_internal_freight_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Gs_internal_freight] CHECK CONSTRAINT [FK_xcuda_Gs_internal_freight_xcuda_Valuation]
GO
ALTER TABLE [dbo].[xcuda_Gs_other_cost]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Gs_other_cost_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Gs_other_cost] CHECK CONSTRAINT [FK_xcuda_Gs_other_cost_xcuda_Valuation]
GO
ALTER TABLE [dbo].[xcuda_HScode]  WITH CHECK ADD  CONSTRAINT [FK_Tarification_HScode] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Tarification] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_HScode] CHECK CONSTRAINT [FK_Tarification_HScode]
GO
ALTER TABLE [dbo].[xcuda_Identification]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Identification] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Identification] CHECK CONSTRAINT [FK_ASYCUDA_Identification]
GO
ALTER TABLE [dbo].[xcuda_Inventory_Item]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Inventory_Item_InventoryItems] FOREIGN KEY([ItemNumber])
REFERENCES [dbo].[InventoryItems] ([ItemNumber])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Inventory_Item] CHECK CONSTRAINT [FK_xcuda_Inventory_Item_InventoryItems]
GO
ALTER TABLE [dbo].[xcuda_Inventory_Item]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Inventory_Item_xcuda_HScode] FOREIGN KEY([id])
REFERENCES [dbo].[xcuda_HScode] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Inventory_Item] CHECK CONSTRAINT [FK_xcuda_Inventory_Item_xcuda_HScode]
GO
ALTER TABLE [dbo].[xcuda_Item]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Item] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
GO
ALTER TABLE [dbo].[xcuda_Item] CHECK CONSTRAINT [FK_ASYCUDA_Item]
GO
ALTER TABLE [dbo].[xcuda_Item]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Item_EntryDataDetails] FOREIGN KEY([EntryDataDetailsId])
REFERENCES [dbo].[EntryDataDetails] ([EntryDataDetailsId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[xcuda_Item] CHECK CONSTRAINT [FK_xcuda_Item_EntryDataDetails]
GO
ALTER TABLE [dbo].[xcuda_item_deduction]  WITH CHECK ADD  CONSTRAINT [FK_Valuation_item_item_deduction] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_item_deduction] CHECK CONSTRAINT [FK_Valuation_item_item_deduction]
GO
ALTER TABLE [dbo].[xcuda_item_external_freight]  WITH CHECK ADD  CONSTRAINT [FK_Valuation_item_item_external_freight] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_item_external_freight] CHECK CONSTRAINT [FK_Valuation_item_item_external_freight]
GO
ALTER TABLE [dbo].[xcuda_item_insurance]  WITH CHECK ADD  CONSTRAINT [FK_Valuation_item_item_insurance] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_item_insurance] CHECK CONSTRAINT [FK_Valuation_item_item_insurance]
GO
ALTER TABLE [dbo].[xcuda_item_internal_freight]  WITH CHECK ADD  CONSTRAINT [FK_Valuation_item_item_internal_freight] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_item_internal_freight] CHECK CONSTRAINT [FK_Valuation_item_item_internal_freight]
GO
ALTER TABLE [dbo].[xcuda_Item_Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Valuation_item_Item_Invoice] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Item_Invoice] CHECK CONSTRAINT [FK_Valuation_item_Item_Invoice]
GO
ALTER TABLE [dbo].[xcuda_item_other_cost]  WITH CHECK ADD  CONSTRAINT [FK_Valuation_item_item_other_cost] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_item_other_cost] CHECK CONSTRAINT [FK_Valuation_item_item_other_cost]
GO
ALTER TABLE [dbo].[xcuda_Market_valuer]  WITH CHECK ADD  CONSTRAINT [FK_Valuation_item_Market_valuer] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Market_valuer] CHECK CONSTRAINT [FK_Valuation_item_Market_valuer]
GO
ALTER TABLE [dbo].[xcuda_Means_of_transport]  WITH CHECK ADD  CONSTRAINT [FK_Transport_Means_of_transport] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Means_of_transport] CHECK CONSTRAINT [FK_Transport_Means_of_transport]
GO
ALTER TABLE [dbo].[xcuda_Nbers]  WITH CHECK ADD  CONSTRAINT [FK_Property_Nbers] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Property] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Nbers] CHECK CONSTRAINT [FK_Property_Nbers]
GO
ALTER TABLE [dbo].[xcuda_Office_segment]  WITH CHECK ADD  CONSTRAINT [FK_Identification_Office_segment] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Office_segment] CHECK CONSTRAINT [FK_Identification_Office_segment]
GO
ALTER TABLE [dbo].[xcuda_Packages]  WITH CHECK ADD  CONSTRAINT [FK_Item_Packages] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Packages] CHECK CONSTRAINT [FK_Item_Packages]
GO
ALTER TABLE [dbo].[xcuda_Place_of_loading]  WITH CHECK ADD  CONSTRAINT [FK_Transport_Place_of_loading] FOREIGN KEY([Transport_Id])
REFERENCES [dbo].[xcuda_Transport] ([Transport_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Place_of_loading] CHECK CONSTRAINT [FK_Transport_Place_of_loading]
GO
ALTER TABLE [dbo].[xcuda_Previous_doc]  WITH CHECK ADD  CONSTRAINT [FK_Item_Previous_doc] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Previous_doc] CHECK CONSTRAINT [FK_Item_Previous_doc]
GO
ALTER TABLE [dbo].[xcuda_PreviousItem]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_PreviousItem] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_PreviousItem] CHECK CONSTRAINT [FK_ASYCUDA_PreviousItem]
GO
ALTER TABLE [dbo].[xcuda_Principal]  WITH CHECK ADD  CONSTRAINT [FK_Transit_Principal] FOREIGN KEY([Transit_Id])
REFERENCES [dbo].[xcuda_Transit] ([Transit_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Principal] CHECK CONSTRAINT [FK_Transit_Principal]
GO
ALTER TABLE [dbo].[xcuda_Property]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Property] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Property] CHECK CONSTRAINT [FK_ASYCUDA_Property]
GO
ALTER TABLE [dbo].[xcuda_receipt]  WITH CHECK ADD  CONSTRAINT [FK_Identification_receipt] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_receipt] CHECK CONSTRAINT [FK_Identification_receipt]
GO
ALTER TABLE [dbo].[xcuda_Registration]  WITH CHECK ADD  CONSTRAINT [FK_Identification_Registration] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Registration] CHECK CONSTRAINT [FK_Identification_Registration]
GO
ALTER TABLE [dbo].[xcuda_Seals]  WITH CHECK ADD  CONSTRAINT [FK_Transit_Seals] FOREIGN KEY([Transit_Id])
REFERENCES [dbo].[xcuda_Transit] ([Transit_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Seals] CHECK CONSTRAINT [FK_Transit_Seals]
GO
ALTER TABLE [dbo].[xcuda_Signature]  WITH CHECK ADD  CONSTRAINT [FK_Transit_Signature] FOREIGN KEY([Transit_Id])
REFERENCES [dbo].[xcuda_Transit] ([Transit_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Signature] CHECK CONSTRAINT [FK_Transit_Signature]
GO
ALTER TABLE [dbo].[xcuda_Supplementary_unit]  WITH CHECK ADD  CONSTRAINT [FK_Tarification_Supplementary_unit] FOREIGN KEY([Tarification_Id])
REFERENCES [dbo].[xcuda_Tarification] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Supplementary_unit] CHECK CONSTRAINT [FK_Tarification_Supplementary_unit]
GO
ALTER TABLE [dbo].[xcuda_Suppliers_documents]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Suppliers_documents] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Suppliers_documents] CHECK CONSTRAINT [FK_ASYCUDA_Suppliers_documents]
GO
ALTER TABLE [dbo].[xcuda_Suppliers_link]  WITH CHECK ADD  CONSTRAINT [FK_Item_Suppliers_link] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Suppliers_link] CHECK CONSTRAINT [FK_Item_Suppliers_link]
GO
ALTER TABLE [dbo].[xcuda_Tarification]  WITH CHECK ADD  CONSTRAINT [FK_Item_Tarification] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Tarification] CHECK CONSTRAINT [FK_Item_Tarification]
GO
ALTER TABLE [dbo].[xcuda_Taxation]  WITH CHECK ADD  CONSTRAINT [FK_Item_Taxation] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Taxation] CHECK CONSTRAINT [FK_Item_Taxation]
GO
ALTER TABLE [dbo].[xcuda_Taxation_line]  WITH CHECK ADD  CONSTRAINT [FK_Taxation_Taxation_line] FOREIGN KEY([Taxation_Id])
REFERENCES [dbo].[xcuda_Taxation] ([Taxation_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Taxation_line] CHECK CONSTRAINT [FK_Taxation_Taxation_line]
GO
ALTER TABLE [dbo].[xcuda_Total]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Total_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Total] CHECK CONSTRAINT [FK_xcuda_Total_xcuda_Valuation]
GO
ALTER TABLE [dbo].[xcuda_Traders]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Traders_xcuda_ASYCUDA] FOREIGN KEY([Traders_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Traders] CHECK CONSTRAINT [FK_xcuda_Traders_xcuda_ASYCUDA]
GO
ALTER TABLE [dbo].[xcuda_Traders_Financial]  WITH CHECK ADD  CONSTRAINT [FK_Traders_Financial] FOREIGN KEY([Traders_Id])
REFERENCES [dbo].[xcuda_Traders] ([Traders_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Traders_Financial] CHECK CONSTRAINT [FK_Traders_Financial]
GO
ALTER TABLE [dbo].[xcuda_Transit]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Transit] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Transit] CHECK CONSTRAINT [FK_ASYCUDA_Transit]
GO
ALTER TABLE [dbo].[xcuda_Transit_Destination]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Transit_Destination_xcuda_Transit] FOREIGN KEY([Transit_Id])
REFERENCES [dbo].[xcuda_Transit] ([Transit_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Transit_Destination] CHECK CONSTRAINT [FK_xcuda_Transit_Destination_xcuda_Transit]
GO
ALTER TABLE [dbo].[xcuda_Transport]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Transport] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Transport] CHECK CONSTRAINT [FK_ASYCUDA_Transport]
GO
ALTER TABLE [dbo].[xcuda_Type]  WITH CHECK ADD  CONSTRAINT [FK_Identification_Type] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Type] CHECK CONSTRAINT [FK_Identification_Type]
GO
ALTER TABLE [dbo].[xcuda_Valuation]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Valuation] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Valuation] CHECK CONSTRAINT [FK_ASYCUDA_Valuation]
GO
ALTER TABLE [dbo].[xcuda_Valuation_item]  WITH CHECK ADD  CONSTRAINT [FK_Item_Valuation_item] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Valuation_item] CHECK CONSTRAINT [FK_Item_Valuation_item]
GO
ALTER TABLE [dbo].[xcuda_Warehouse]  WITH CHECK ADD  CONSTRAINT [FK_ASYCUDA_Warehouse] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Warehouse] CHECK CONSTRAINT [FK_ASYCUDA_Warehouse]
GO
ALTER TABLE [dbo].[xcuda_Weight]  WITH CHECK ADD  CONSTRAINT [FK_xcuda_Weight_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Weight] CHECK CONSTRAINT [FK_xcuda_Weight_xcuda_Valuation]
GO
ALTER TABLE [dbo].[xcuda_Weight_itm]  WITH CHECK ADD  CONSTRAINT [FK_Valuation_item_Weight_itm] FOREIGN KEY([Valuation_item_Id])
REFERENCES [dbo].[xcuda_Valuation_item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Weight_itm] CHECK CONSTRAINT [FK_Valuation_item_Weight_itm]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 111
               Right = 363
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 35
               Left = 620
               Bottom = 198
               Right = 839
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 343
               Left = 48
               Bottom = 506
               Right = 358
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA"
            Begin Extent = 
               Top = 511
               Left = 48
               Bottom = 652
               Right = 252
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 658
               Left = 48
               Bottom = 821
               Right = 339
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 826
               Left = 48
               Bottom = 967
               Right = 301
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 973
               Left = 48
               Bottom = 1136
               Right =' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousDocuments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' 246
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousDocuments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousDocuments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 82
               Left = 587
               Bottom = 245
               Right = 806
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 973
               Left = 48
               Bottom = 1136
               Right = 246
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 658
               Left = 48
               Bottom = 821
               Right = 339
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 826
               Left = 48
               Bottom = 967
               Right = 301
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 343
               Left = 48
               Bottom = 506
               Right = 358
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 267
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA"
            Begin Extent = 
               Top = 511
               Left = 48
               Bottom = 652
               Right =' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' 252
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[48] 4[22] 2[12] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[70] 2[17] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[74] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 2
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = -360
      End
      Begin Tables = 
         Begin Table = "Ext"
            Begin Extent = 
               Top = 62
               Left = 440
               Bottom = 137
               Right = 709
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Iden"
            Begin Extent = 
               Top = 168
               Left = 284
               Bottom = 243
               Right = 577
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Reg"
            Begin Extent = 
               Top = 101
               Left = 749
               Bottom = 176
               Right = 959
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA"
            Begin Extent = 
               Top = 0
               Left = 6
               Bottom = 141
               Right = 226
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Declarant"
            Begin Extent = 
               Top = 0
               Left = 355
               Bottom = 75
               Right = 630
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Inventory_Item"
            Begin Extent = 
               Top = 279
               Left = 1233
               Bottom = 436
               Right = 1514
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 54
               Left = 1592
               Bottom = 250
               Right = 1812
            End
          ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'  DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "HS"
            Begin Extent = 
               Top = 197
               Left = 924
               Bottom = 353
               Right = 1152
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "Tar"
            Begin Extent = 
               Top = 279
               Left = 500
               Bottom = 475
               Right = 807
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Supp"
            Begin Extent = 
               Top = 408
               Left = 1086
               Bottom = 483
               Right = 1397
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Item"
            Begin Extent = 
               Top = 285
               Left = 102
               Bottom = 360
               Right = 428
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 15
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1740
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      PaneHidden = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "PO_ORD_LIN_1"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 272
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointPODetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointPODetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "PO_ORD_HDR_1"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 323
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointPOs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointPOs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "Sales"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 239
               Right = 283
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1680
         Width = 2868
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 3240
         Alias = 900
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 1416
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointSales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointSales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[33] 4[28] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "PS_TKT_HIST_1"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 346
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PS_TKT_HIST_LIN_1"
            Begin Extent = 
               Top = 7
               Left = 394
               Bottom = 170
               Right = 684
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PS_TKT_HIST_CONTACT_1"
            Begin Extent = 
               Top = 175
               Left = 48
               Bottom = 338
               Right = 243
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AR_CUST_1"
            Begin Extent = 
               Top = 175
               Left = 291
               Bottom = 338
               Right = 632
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 13
         Width = 284
         Width = 1200
         Width = 1200
         Width = 2676
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1920
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 5364
         Alias = 2724
         Table = 1176
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1356
         SortOrder = 14' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointSalesDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'16
         GroupBy = 1350
         Filter = 1356
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointSalesDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CounterPointSalesDetails'
GO
USE [master]
GO
ALTER DATABASE [WaterNutDB] SET  READ_WRITE 
GO
