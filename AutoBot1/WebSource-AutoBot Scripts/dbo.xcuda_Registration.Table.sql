USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Registration]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Registration](
	[Number] [nvarchar](20) NULL,
	[Date] [datetime2](7) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Registration] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Registration] ([Number], [Date], [ASYCUDA_Id]) VALUES (NULL, NULL, 48427)
INSERT [dbo].[xcuda_Registration] ([Number], [Date], [ASYCUDA_Id]) VALUES (NULL, NULL, 48428)
INSERT [dbo].[xcuda_Registration] ([Number], [Date], [ASYCUDA_Id]) VALUES (NULL, NULL, 48429)
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Registration_6_5]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Registration_6_5] ON [dbo].[xcuda_Registration]
(
	[Number] ASC,
	[Date] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Registration]  WITH NOCHECK ADD  CONSTRAINT [FK_Identification_Registration] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Registration] CHECK CONSTRAINT [FK_Identification_Registration]
GO
