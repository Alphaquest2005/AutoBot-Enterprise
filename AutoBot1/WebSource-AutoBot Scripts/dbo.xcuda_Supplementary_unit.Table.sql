USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Supplementary_unit]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Supplementary_unit](
	[Suppplementary_unit_quantity] [float] NULL,
	[Supplementary_unit_Id] [int] IDENTITY(1,1) NOT NULL,
	[Tarification_Id] [int] NOT NULL,
	[Suppplementary_unit_code] [nvarchar](4) NULL,
	[Suppplementary_unit_name] [nvarchar](255) NULL,
	[IsFirstRow] [bit] NULL,
 CONSTRAINT [PK_xcuda_Supplementary_unit] PRIMARY KEY CLUSTERED 
(
	[Supplementary_unit_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[xcuda_Supplementary_unit] ON 

INSERT [dbo].[xcuda_Supplementary_unit] ([Suppplementary_unit_quantity], [Supplementary_unit_Id], [Tarification_Id], [Suppplementary_unit_code], [Suppplementary_unit_name], [IsFirstRow]) VALUES (3, 2057945, 1004680, N'NMB', NULL, 1)
INSERT [dbo].[xcuda_Supplementary_unit] ([Suppplementary_unit_quantity], [Supplementary_unit_Id], [Tarification_Id], [Suppplementary_unit_code], [Suppplementary_unit_name], [IsFirstRow]) VALUES (3, 2057946, 1004681, N'NMB', NULL, 1)
INSERT [dbo].[xcuda_Supplementary_unit] ([Suppplementary_unit_quantity], [Supplementary_unit_Id], [Tarification_Id], [Suppplementary_unit_code], [Suppplementary_unit_name], [IsFirstRow]) VALUES (3, 2057947, 1004682, N'NMB', NULL, 1)
SET IDENTITY_INSERT [dbo].[xcuda_Supplementary_unit] OFF
GO
/****** Object:  Index [SQLOPS_xcuda_Supplementary_unit_115_114]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Supplementary_unit_115_114] ON [dbo].[xcuda_Supplementary_unit]
(
	[IsFirstRow] ASC
)
INCLUDE([Suppplementary_unit_quantity],[Tarification_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Supplementary_unit_232_231]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Supplementary_unit_232_231] ON [dbo].[xcuda_Supplementary_unit]
(
	[Suppplementary_unit_quantity] ASC,
	[Tarification_Id] ASC,
	[IsFirstRow] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Supplementary_unit_45_44]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Supplementary_unit_45_44] ON [dbo].[xcuda_Supplementary_unit]
(
	[Tarification_Id] ASC,
	[IsFirstRow] ASC
)
INCLUDE([Suppplementary_unit_quantity]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Supplementary_unit]  WITH NOCHECK ADD  CONSTRAINT [FK_Tarification_Supplementary_unit] FOREIGN KEY([Tarification_Id])
REFERENCES [dbo].[xcuda_Tarification] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Supplementary_unit] CHECK CONSTRAINT [FK_Tarification_Supplementary_unit]
GO
