USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Suppliers_documents]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Suppliers_documents](
	[Suppliers_document_date] [nvarchar](max) NULL,
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Suppliers_documents]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Suppliers_documents] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Suppliers_documents] CHECK CONSTRAINT [FK_ASYCUDA_Suppliers_documents]
GO
