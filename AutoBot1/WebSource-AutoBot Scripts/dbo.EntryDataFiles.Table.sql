USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryDataFiles]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryDataFiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[EmailId] [nvarchar](255) NULL,
	[FileTypeId] [int] NULL,
	[FileType] [nvarchar](50) NOT NULL,
	[SourceFile] [nvarchar](max) NOT NULL,
	[SourceRow] [nvarchar](max) NOT NULL,
	[EntryData_Id] [int] NOT NULL,
	[EntryDataId] [nvarchar](50) NULL,
	[EntryDataDate] [datetime2](7) NULL,
	[LineNumber] [int] NULL,
	[ItemNumber] [nvarchar](20) NULL,
	[Quantity] [float] NULL,
	[Units] [nvarchar](15) NULL,
	[ItemDescription] [nvarchar](255) NULL,
	[Cost] [float] NULL,
	[TotalCost] [float] NULL,
	[InvoiceQty] [float] NULL,
	[ReceivedQty] [float] NULL,
	[TaxAmount] [float] NULL,
 CONSTRAINT [PK_EntryDataFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[EntryDataFiles] ON 

INSERT [dbo].[EntryDataFiles] ([Id], [ApplicationSettingsId], [EmailId], [FileTypeId], [FileType], [SourceFile], [SourceRow], [EntryData_Id], [EntryDataId], [EntryDataDate], [LineNumber], [ItemNumber], [Quantity], [Units], [ItemDescription], [Cost], [TotalCost], [InvoiceQty], [ReceivedQty], [TaxAmount]) VALUES (1410476, 3, N'Shipment: HAWB9595443--2025-04-03-07:13:53', 1152, N'PO', N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\478\111-8019845-2302666-Fixed.csv', N'"4200-000","111-8019845-2302666",,"2024-07-15",,"94054000","MESAILUP16InchLEDLig","MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)",,,"3",,,,"39.99","119.97","119.97","171.37","43","0","8.4","0","1","Marks","Amazon.com","Amazon.com",,,,,,,,,', 0, N'111-8019845-2302666', CAST(N'2024-07-15T00:00:00.0000000' AS DateTime2), 1, N'MESAILUP16InchLEDLig', 3, NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', 39.9900016784668, 119.97000122070313, NULL, NULL, NULL)
INSERT [dbo].[EntryDataFiles] ([Id], [ApplicationSettingsId], [EmailId], [FileTypeId], [FileType], [SourceFile], [SourceRow], [EntryData_Id], [EntryDataId], [EntryDataDate], [LineNumber], [ItemNumber], [Quantity], [Units], [ItemDescription], [Cost], [TotalCost], [InvoiceQty], [ReceivedQty], [TaxAmount]) VALUES (1410477, 3, N'Shipment: HAWB9595443--2025-04-03-21:34:29', 1152, N'PO', N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595443\492\111-8019845-2302666-Fixed.csv', N'"4200-000","111-8019845-2302666","111-8019845-2302666","2024-07-15",,,"MESAILUP16InchLEDLig","MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)","MESAILUP16INCHLEDLIG","MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)","3",,,,"39.99","119.97",,"171.37","43","0","8.4","0","1","Marks","Amazon.com","Amazon.com",,,,,,,,,', 0, N'111-8019845-2302666', CAST(N'2024-07-15T00:00:00.0000000' AS DateTime2), 1, N'MESAILUP16INCHLEDLIG', 3, NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', 39.9900016784668, 119.97000122070313, NULL, NULL, NULL)
INSERT [dbo].[EntryDataFiles] ([Id], [ApplicationSettingsId], [EmailId], [FileTypeId], [FileType], [SourceFile], [SourceRow], [EntryData_Id], [EntryDataId], [EntryDataDate], [LineNumber], [ItemNumber], [Quantity], [Units], [ItemDescription], [Cost], [TotalCost], [InvoiceQty], [ReceivedQty], [TaxAmount]) VALUES (1410478, 3, N'Shipment: HAWB9595459--2025-04-03-21:34:32', 1152, N'PO', N'D:\OneDrive\Clients\WebSource\Emails\HAWB9595459\493\111-8019845-2302666-Fixed.csv', N'"4200-000","111-8019845-2302666","111-8019845-2302666","2024-07-15",,,"MESAILUP16InchLEDLig","MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)","MESAILUP16INCHLEDLIG","MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)","3",,,,"39.99","119.97",,"171.37","43","0","8.4","0","1","Marks","Amazon.com","Amazon.com",,,,,,,,,', 0, N'111-8019845-2302666', CAST(N'2024-07-15T00:00:00.0000000' AS DateTime2), 1, N'MESAILUP16INCHLEDLIG', 3, NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', 39.9900016784668, 119.97000122070313, NULL, NULL, NULL)
INSERT [dbo].[EntryDataFiles] ([Id], [ApplicationSettingsId], [EmailId], [FileTypeId], [FileType], [SourceFile], [SourceRow], [EntryData_Id], [EntryDataId], [EntryDataDate], [LineNumber], [ItemNumber], [Quantity], [Units], [ItemDescription], [Cost], [TotalCost], [InvoiceQty], [ReceivedQty], [TaxAmount]) VALUES (1410479, 3, N'Shipment: HAWB9596948--2025-04-03-21:34:36', 1152, N'PO', N'D:\OneDrive\Clients\WebSource\Emails\HAWB9596948\494\111-8019845-2302666-Fixed.csv', N'"4200-000","111-8019845-2302666","111-8019845-2302666","2024-07-15",,,"MESAILUP16InchLEDLig","MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)","MESAILUP16INCHLEDLIG","MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)","3",,,,"39.99","119.97",,"171.37","43","0","8.4","0","1","Marks","Amazon.com","Amazon.com",,,,,,,,,', 0, N'111-8019845-2302666', CAST(N'2024-07-15T00:00:00.0000000' AS DateTime2), 1, N'MESAILUP16INCHLEDLIG', 3, NULL, N'MESAILUP 16 Inch LED Lighted Liquor Bottle Display 2 Step Illuminated Bottle Shelf 2 Tier Home Bar Drinks Commercial Lighting Shelves with Remote Control (2 Tier, 16 inch)', 39.9900016784668, 119.97000122070313, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[EntryDataFiles] OFF
GO
