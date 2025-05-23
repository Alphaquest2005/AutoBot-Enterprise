USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[SupplierItemDescriptionRegEx]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupplierItemDescriptionRegEx](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SupplierItemDescriptionId] [int] NOT NULL,
	[Attribute] [nvarchar](50) NOT NULL,
	[RegEx] [nvarchar](255) NOT NULL,
	[POSRegEx] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_SupplierItemDescriptionRegEx] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[SupplierItemDescriptionRegEx] ON 

INSERT [dbo].[SupplierItemDescriptionRegEx] ([Id], [SupplierItemDescriptionId], [Attribute], [RegEx], [POSRegEx]) VALUES (1, 1, N'Description', N'COTTER PIN', N'COTTER PIN')
INSERT [dbo].[SupplierItemDescriptionRegEx] ([Id], [SupplierItemDescriptionId], [Attribute], [RegEx], [POSRegEx]) VALUES (2, 1, N'Size', N'[\d/-]+[\sX]+[\d/-]+', N'[\d/-]+[\sX]+[\d/-]+')
INSERT [dbo].[SupplierItemDescriptionRegEx] ([Id], [SupplierItemDescriptionId], [Attribute], [RegEx], [POSRegEx]) VALUES (3, 2, N'Description', N'Self Tapping Screw', N' T\-A \*')
INSERT [dbo].[SupplierItemDescriptionRegEx] ([Id], [SupplierItemDescriptionId], [Attribute], [RegEx], [POSRegEx]) VALUES (4, 2, N'Size', N'[\d/-]+[\sX]+[\d/-]+', N'[\d/-]+[\sX]+[\d/-]+')
INSERT [dbo].[SupplierItemDescriptionRegEx] ([Id], [SupplierItemDescriptionId], [Attribute], [RegEx], [POSRegEx]) VALUES (6, 2, N'Material', N'Stainless Steel', N' SS ')
INSERT [dbo].[SupplierItemDescriptionRegEx] ([Id], [SupplierItemDescriptionId], [Attribute], [RegEx], [POSRegEx]) VALUES (7, 3, N'Description', N'Hex Head Bolt', N'HEX HEAD CAP SCREW')
INSERT [dbo].[SupplierItemDescriptionRegEx] ([Id], [SupplierItemDescriptionId], [Attribute], [RegEx], [POSRegEx]) VALUES (8, 3, N'Size', N'[\d/-]+[\sX]+[\d/-]+', N'[\d/-]+[\sX]+[\d/-]+')
INSERT [dbo].[SupplierItemDescriptionRegEx] ([Id], [SupplierItemDescriptionId], [Attribute], [RegEx], [POSRegEx]) VALUES (9, 3, N'Material', N'Stainless Steel', N' SS ')
SET IDENTITY_INSERT [dbo].[SupplierItemDescriptionRegEx] OFF
GO
ALTER TABLE [dbo].[SupplierItemDescriptionRegEx]  WITH CHECK ADD  CONSTRAINT [FK_SupplierItemDescriptionRegEx_SupplierItemDescription] FOREIGN KEY([SupplierItemDescriptionId])
REFERENCES [dbo].[SupplierItemDescription] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SupplierItemDescriptionRegEx] CHECK CONSTRAINT [FK_SupplierItemDescriptionRegEx_SupplierItemDescription]
GO
