USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[History-Allocations]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[History-Allocations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchNo] [int] NOT NULL,
	[AllocationId] [int] NULL,
	[Status] [nvarchar](255) NULL,
	[QtyAllocated] [float] NULL,
	[xLineNumber] [int] NULL,
	[InvoiceDate] [datetime] NULL,
	[CustomerName] [nvarchar](255) NULL,
	[SalesQuantity] [float] NULL,
	[SalesQtyAllocated] [float] NULL,
	[InvoiceNo] [nvarchar](100) NULL,
	[InvoiceLineNumber] [int] NULL,
	[ItemNumber] [nvarchar](510) NULL,
	[ItemDescription] [nvarchar](255) NULL,
	[DutyFreePaid] [nvarchar](18) NOT NULL,
	[pCNumber] [nvarchar](255) NULL,
	[pRegistrationDate] [datetime] NULL,
	[pQuantity] [float] NULL,
	[pQtyAllocated] [float] NULL,
	[PiQuantity] [float] NULL,
	[SalesFactor] [float] NULL,
	[xCNumber] [nvarchar](255) NULL,
	[xRegistrationDate] [datetime] NULL,
	[xQuantity] [float] NULL,
	[pReferenceNumber] [nvarchar](255) NULL,
	[pLineNumber] [int] NULL,
	[Cost] [float] NULL,
	[Total_CIF_itm] [float] NULL,
	[DutyLiability] [float] NULL,
	[TaxAmount] [float] NULL,
	[pIsAssessed] [bit] NULL,
	[DoNotAllocateSales] [bit] NULL,
	[DoNotAllocatePreviousEntry] [bit] NULL,
	[WarehouseError] [nvarchar](255) NULL,
	[SANumber] [int] NULL,
	[xReferenceNumber] [nvarchar](255) NULL,
	[TariffCode] [nvarchar](25) NULL,
	[Invalid] [bit] NULL,
	[pExpiryDate] [datetime] NULL,
	[pTariffCode] [nvarchar](25) NULL,
	[pItemNumber] [nvarchar](100) NULL,
	[EntryDateTime] [datetime] NOT NULL,
	[xStatus] [nvarchar](255) NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_History-Allocations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[History-Allocations]  WITH CHECK ADD  CONSTRAINT [FK_History-Allocations_History-Batches] FOREIGN KEY([BatchNo])
REFERENCES [dbo].[History-Batches] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[History-Allocations] CHECK CONSTRAINT [FK_History-Allocations_History-Batches]
GO
