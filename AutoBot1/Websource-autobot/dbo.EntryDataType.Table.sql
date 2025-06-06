USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryDataType]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryDataType](
	[EntryDataType] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[RequirePreviousCNumber] [bit] NOT NULL,
	[RequirePreviousInvoiceNumber] [bit] NOT NULL,
 CONSTRAINT [PK_EntryDataType] PRIMARY KEY CLUSTERED 
(
	[EntryDataType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[EntryDataType] ([EntryDataType], [Description], [RequirePreviousCNumber], [RequirePreviousInvoiceNumber]) VALUES (N'ADJ', N'Adjustments', 0, 0)
INSERT [dbo].[EntryDataType] ([EntryDataType], [Description], [RequirePreviousCNumber], [RequirePreviousInvoiceNumber]) VALUES (N'DIS', N'Discrepancy', 1, 1)
INSERT [dbo].[EntryDataType] ([EntryDataType], [Description], [RequirePreviousCNumber], [RequirePreviousInvoiceNumber]) VALUES (N'INV', N'Shipment Invoice', 0, 0)
INSERT [dbo].[EntryDataType] ([EntryDataType], [Description], [RequirePreviousCNumber], [RequirePreviousInvoiceNumber]) VALUES (N'OPS', N'Opening Stock', 0, 0)
INSERT [dbo].[EntryDataType] ([EntryDataType], [Description], [RequirePreviousCNumber], [RequirePreviousInvoiceNumber]) VALUES (N'PO', N'Purchase Order', 0, 0)
INSERT [dbo].[EntryDataType] ([EntryDataType], [Description], [RequirePreviousCNumber], [RequirePreviousInvoiceNumber]) VALUES (N'Sales', N'Sales', 0, 0)
GO
