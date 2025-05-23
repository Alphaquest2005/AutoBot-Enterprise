USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xSalesDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xSalesDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[xSalesFileId] [int] NOT NULL,
	[FileLine] [int] NOT NULL,
	[Line] [int] NOT NULL,
	[Date] [datetime] NOT NULL,
	[InvoiceNo] [nvarchar](50) NOT NULL,
	[CustomerName] [nvarchar](50) NULL,
	[ItemNumber] [nvarchar](50) NOT NULL,
	[ItemDescription] [nvarchar](50) NULL,
	[TariffCode] [nvarchar](50) NULL,
	[SalesQuantity] [float] NULL,
	[SalesFactor] [float] NULL,
	[xQuantity] [float] NOT NULL,
	[Price] [float] NULL,
	[DutyFreePaid] [nvarchar](50) NOT NULL,
	[pCNumber] [nvarchar](50) NOT NULL,
	[pLineNumber] [int] NOT NULL,
	[pRegDate] [datetime] NOT NULL,
	[CIFValue] [float] NULL,
	[DutyLiablity] [float] NULL,
	[Comment] [nvarchar](50) NULL,
	[InvoiceLineNumber] [int] NULL,
 CONSTRAINT [PK_xSalesDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xSalesDetails]  WITH CHECK ADD  CONSTRAINT [FK_xSalesDetails_xSalesFiles] FOREIGN KEY([xSalesFileId])
REFERENCES [dbo].[xSalesFiles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xSalesDetails] CHECK CONSTRAINT [FK_xSalesDetails_xSalesFiles]
GO
