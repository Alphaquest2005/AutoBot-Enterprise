USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ItemHistory]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
	[FileTypeId] [int] NULL,
	[SourceFile] [nvarchar](500) NULL,
	[EmailId] [nvarchar](255) NULL,
	[TransactionId] [nvarchar](255) NULL,
	[TransactionType] [nvarchar](50) NULL,
	[ItemNumber] [nvarchar](50) NULL,
	[ItemDescription] [nvarchar](255) NULL,
	[Date] [datetime] NULL,
	[Quantity] [float] NULL,
	[Cost] [float] NULL,
	[Comments] [nvarchar](1000) NULL,
 CONSTRAINT [PK_ItemHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_ItemHistory_2_1]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_ItemHistory_2_1] ON [dbo].[ItemHistory]
(
	[TransactionId] ASC
)
INCLUDE([ApplicationSettingsId],[FileTypeId],[SourceFile],[EmailId],[TransactionType],[ItemNumber],[ItemDescription],[Date],[Quantity],[Cost],[Comments]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_ItemHistory_40_39]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_ItemHistory_40_39] ON [dbo].[ItemHistory]
(
	[TransactionType] ASC,
	[ItemNumber] ASC,
	[Date] ASC
)
INCLUDE([ItemDescription],[Quantity],[Comments]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_ItemHistory_42_41]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_ItemHistory_42_41] ON [dbo].[ItemHistory]
(
	[TransactionType] ASC,
	[ItemNumber] ASC
)
INCLUDE([ItemDescription],[Date],[Quantity],[Comments]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_ItemHistory_50_49]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_ItemHistory_50_49] ON [dbo].[ItemHistory]
(
	[TransactionType] ASC,
	[ItemNumber] ASC,
	[Date] ASC
)
INCLUDE([TransactionId],[ItemDescription],[Quantity]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_ItemHistory_52_51]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_ItemHistory_52_51] ON [dbo].[ItemHistory]
(
	[TransactionType] ASC,
	[ItemNumber] ASC
)
INCLUDE([TransactionId],[ItemDescription],[Date],[Quantity]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
