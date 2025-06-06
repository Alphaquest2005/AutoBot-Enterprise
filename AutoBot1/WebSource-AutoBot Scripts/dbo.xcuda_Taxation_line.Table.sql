USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Taxation_line]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Taxation_line](
	[Duty_tax_Base] [nvarchar](20) NULL,
	[Duty_tax_rate] [float] NOT NULL,
	[Duty_tax_amount] [float] NOT NULL,
	[Taxation_line_Id] [int] IDENTITY(1,1) NOT NULL,
	[Taxation_Id] [int] NULL,
	[Duty_tax_code] [nvarchar](20) NULL,
	[Duty_tax_MP] [nvarchar](20) NULL,
 CONSTRAINT [PK_xcuda_Taxation_line] PRIMARY KEY CLUSTERED 
(
	[Taxation_line_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [SQLOPS_xcuda_Taxation_line_565_564]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Taxation_line_565_564] ON [dbo].[xcuda_Taxation_line]
(
	[Taxation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [SQLOPS_xcuda_Taxation_line_580_579]    Script Date: 4/8/2025 8:33:18 AM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Taxation_line_580_579] ON [dbo].[xcuda_Taxation_line]
(
	[Duty_tax_code] ASC
)
INCLUDE([Duty_tax_amount],[Taxation_Id]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Taxation_line]  WITH NOCHECK ADD  CONSTRAINT [FK_Taxation_Taxation_line] FOREIGN KEY([Taxation_Id])
REFERENCES [dbo].[xcuda_Taxation] ([Taxation_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Taxation_line] CHECK CONSTRAINT [FK_Taxation_Taxation_line]
GO
