USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ShipmentInvoiceExtraInfo]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShipmentInvoiceExtraInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Info] [nvarchar](50) NOT NULL,
	[Value] [nvarchar](255) NOT NULL,
	[InvoiceId] [int] NOT NULL,
 CONSTRAINT [PK_ShipmentInvoiceExtraInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ShipmentInvoiceExtraInfo]  WITH CHECK ADD  CONSTRAINT [FK_ShipmentInvoiceExtraInfo_ShipmentInvoice] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[ShipmentInvoice] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ShipmentInvoiceExtraInfo] CHECK CONSTRAINT [FK_ShipmentInvoiceExtraInfo_ShipmentInvoice]
GO
