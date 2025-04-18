USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR-PartTypes]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCR-PartTypes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_OCR-PartTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[OCR-PartTypes] ON 

INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (1, N'Invoice')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (2, N'InvoiceSummary')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (3, N'Header')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (4, N'InvoiceLines')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (10, N'InvoiceLine')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (11, N'WayBill')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (12, N'Details')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (13, N'ExtraInfo')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (14, N'DetailsSection')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (15, N'Details Section')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (16, N'SparseDetailsSection')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (17, N'SparseDetails')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (18, N'Details Line')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (19, N'Marks')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (20, N'InvoiceLine2')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (21, N'Column1')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (22, N'Column2')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (23, N'Details2')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (24, N'SAD')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (25, N'InvoiceLine3')
INSERT [dbo].[OCR-PartTypes] ([Id], [Name]) VALUES (26, N'Invoice Details')
SET IDENTITY_INSERT [dbo].[OCR-PartTypes] OFF
GO
