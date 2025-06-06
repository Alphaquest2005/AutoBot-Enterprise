USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_HScode]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_HScode](
	[Commodity_code] [nvarchar](20) NOT NULL,
	[Precision_1] [nvarchar](255) NULL,
	[Precision_4] [nvarchar](50) NULL,
	[Item_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_HScode] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_xcuda_HScode]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [IX_xcuda_HScode] ON [dbo].[xcuda_HScode]
(
	[Commodity_code] ASC,
	[Precision_4] ASC,
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_HScode_55_54]    Script Date: 3/27/2025 1:48:25 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_HScode_55_54] ON [dbo].[xcuda_HScode]
(
	[Precision_4] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_HScode]  WITH NOCHECK ADD  CONSTRAINT [FK_Tarification_HScode] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Tarification] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_HScode] CHECK CONSTRAINT [FK_Tarification_HScode]
GO
