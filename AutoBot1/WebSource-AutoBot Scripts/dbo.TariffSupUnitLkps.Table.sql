USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[TariffSupUnitLkps]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TariffSupUnitLkps](
	[SuppUnitCode2] [nvarchar](50) NOT NULL,
	[SuppUnitName2] [nvarchar](50) NULL,
	[SuppQty] [float] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_TariffSupUnitLkps] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[TariffSupUnitLkps] ON 

INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'LTR', N'Litre (1 dm3)', 1.5, 2)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'MTQ', NULL, 32, 3)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'MTQ', NULL, 32, 4)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'LTR', NULL, 1.5, 5)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'MTQ', NULL, 32, 7)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'MTQ', NULL, 32, 8)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'MTQ', NULL, 32, 9)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'LTR', NULL, 1.5, 10)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'LTR', NULL, 1.5, 11)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'MTQ', NULL, 32, 12)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'LTR', NULL, 1.5, 14)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'MTK', NULL, 1.5, 15)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'MTK', NULL, 1.5, 17)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'YEF', NULL, 1.5, 19)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'NMB', N'Number', 1, 20)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'NPR', N'', 1, 21)
INSERT [dbo].[TariffSupUnitLkps] ([SuppUnitCode2], [SuppUnitName2], [SuppQty], [Id]) VALUES (N'ASV', N'Alcoholic strength by volume', 1, 22)
SET IDENTITY_INSERT [dbo].[TariffSupUnitLkps] OFF
GO
