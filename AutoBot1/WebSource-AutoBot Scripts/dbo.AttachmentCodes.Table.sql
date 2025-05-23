USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[AttachmentCodes]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AttachmentCodes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](4) NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AttachmentCodes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[AttachmentCodes] ON 

INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (1, N'BL10', N'BILL OF LADING COPY')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (2, N'BL07', N'AIR WAY BILL COPY')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (3, N'IV05', N'COMMERCIAL INVOICE')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (4, N'IV04', N'FREIGHT INVOICE')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (5, N'DO02', N'PREVIOUS CUSTOMS ENTRY(SAD)')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (6, N'DC05', N'VALUE DECLARATION (C71)')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (7, N'DC02', N'SHIP''S STORES DECLARATION')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (8, N'W02', N'WAREHOUSING INFO.')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (9, N'LC02', N'IMPORT LICENCE')
INSERT [dbo].[AttachmentCodes] ([Id], [Code], [Description]) VALUES (11, N'DFS1', N'DUTY FREE SALES: DOCUMENT')
SET IDENTITY_INSERT [dbo].[AttachmentCodes] OFF
GO
