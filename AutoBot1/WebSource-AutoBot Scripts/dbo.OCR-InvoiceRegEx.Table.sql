USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR-InvoiceRegEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCR-InvoiceRegEx](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InvoiceId] [int] NOT NULL,
	[RegExId] [int] NOT NULL,
	[ReplacementRegExId] [int] NOT NULL,
 CONSTRAINT [PK_OCR-InvoiceRegEx] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[OCR-InvoiceRegEx] ON 

INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (1, 10, 140, 60)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (2, 12, 153, 60)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (3, 12, 352, 60)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (4, 39, 587, 588)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (6, 10, 2183, 60)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (7, 10, 2184, 60)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (8, 154, 2276, 60)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (9, 154, 2277, 60)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (10, 154, 2278, 60)
INSERT [dbo].[OCR-InvoiceRegEx] ([Id], [InvoiceId], [RegExId], [ReplacementRegExId]) VALUES (11, 154, 2279, 2280)
SET IDENTITY_INSERT [dbo].[OCR-InvoiceRegEx] OFF
GO
ALTER TABLE [dbo].[OCR-InvoiceRegEx]  WITH CHECK ADD  CONSTRAINT [FK_OCR-InvoiceRegEx_OCR-Invoices] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[OCR-Invoices] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCR-InvoiceRegEx] CHECK CONSTRAINT [FK_OCR-InvoiceRegEx_OCR-Invoices]
GO
ALTER TABLE [dbo].[OCR-InvoiceRegEx]  WITH CHECK ADD  CONSTRAINT [FK_OCR-InvoiceRegEx_OCR-RegularExpressions] FOREIGN KEY([RegExId])
REFERENCES [dbo].[OCR-RegularExpressions] ([Id])
GO
ALTER TABLE [dbo].[OCR-InvoiceRegEx] CHECK CONSTRAINT [FK_OCR-InvoiceRegEx_OCR-RegularExpressions]
GO
ALTER TABLE [dbo].[OCR-InvoiceRegEx]  WITH CHECK ADD  CONSTRAINT [FK_OCR-InvoiceRegEx_OCR-RegularExpressions1] FOREIGN KEY([ReplacementRegExId])
REFERENCES [dbo].[OCR-RegularExpressions] ([Id])
GO
ALTER TABLE [dbo].[OCR-InvoiceRegEx] CHECK CONSTRAINT [FK_OCR-InvoiceRegEx_OCR-RegularExpressions1]
GO
