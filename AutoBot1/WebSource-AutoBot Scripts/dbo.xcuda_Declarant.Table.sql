USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Declarant]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Declarant](
	[Declarant_code] [nvarchar](20) NULL,
	[Declarant_name] [nvarchar](255) NULL,
	[Declarant_representative] [nvarchar](255) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[Number] [nvarchar](30) NULL,
 CONSTRAINT [PK_xcuda_Declarant] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Declarant] ([Declarant_code], [Declarant_name], [Declarant_representative], [ASYCUDA_Id], [Number]) VALUES (N'225756', NULL, NULL, 48427, N'HAWB9595443-1')
INSERT [dbo].[xcuda_Declarant] ([Declarant_code], [Declarant_name], [Declarant_representative], [ASYCUDA_Id], [Number]) VALUES (N'225756', NULL, NULL, 48428, N'HAWB9595459-1')
INSERT [dbo].[xcuda_Declarant] ([Declarant_code], [Declarant_name], [Declarant_representative], [ASYCUDA_Id], [Number]) VALUES (N'225756', NULL, NULL, 48429, N'HAWB9596948-1')
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_xcuda_Declarant]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [IX_xcuda_Declarant] ON [dbo].[xcuda_Declarant]
(
	[ASYCUDA_Id] ASC,
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Declarant_134_133]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Declarant_134_133] ON [dbo].[xcuda_Declarant]
(
	[Number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Declarant_22_21]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Declarant_22_21] ON [dbo].[xcuda_Declarant]
(
	[Declarant_code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Declarant]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Declarant] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Declarant] CHECK CONSTRAINT [FK_ASYCUDA_Declarant]
GO
