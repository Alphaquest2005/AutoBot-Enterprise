USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AsycudaWarehouseData]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AsycudaWarehouseData](
	[WHS_DAYS] [int] NOT NULL,
	[WHS_COD] [float] NOT NULL,
	[WHS_NAM] [nvarchar](50) NOT NULL,
	[CMP_COD] [float] NOT NULL,
	[CMP_NAM] [nvarchar](50) NOT NULL,
	[IDE_REG_DAT] [datetime2](7) NOT NULL,
	[IDE_TYP_SAD] [nvarchar](50) NOT NULL,
	[IDE_TYP_PRC] [int] NOT NULL,
	[PROCEDURE_CODE] [int] NOT NULL,
	[IDE_REG_SER] [nvarchar](50) NOT NULL,
	[REG_NBR] [int] NOT NULL,
	[KEY_DEC] [int] NOT NULL,
	[YEAR] [int] NOT NULL,
	[REF_NBER] [nvarchar](50) NOT NULL,
	[OFFICE] [nvarchar](50) NOT NULL,
	[END_DAT] [datetime2](7) NOT NULL,
	[ITEM_NBR] [nvarchar](50) NOT NULL,
	[HSC_COD] [int] NOT NULL,
	[PRODUCT_ID] [nvarchar](50) NULL,
	[TAR_DSC] [nvarchar](255) NOT NULL,
	[CTY_COD] [nvarchar](255) NOT NULL,
	[INIT_WGT] [nvarchar](50) NOT NULL,
	[INIT_QTY] [float] NOT NULL,
	[REM_QTY] [float] NOT NULL,
	[REM_WGT] [nvarchar](50) NOT NULL,
	[TAR_SUP_COD] [nvarchar](50) NOT NULL,
	[CTY_NAM] [nvarchar](50) NOT NULL,
	[WHS_TIM] [datetime2](7) NOT NULL,
	[ApplicationSettingsId] [int] NULL
) ON [PRIMARY]
GO
