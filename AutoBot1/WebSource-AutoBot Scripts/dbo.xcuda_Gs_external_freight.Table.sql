USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Gs_external_freight]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Gs_external_freight](
	[Amount_national_currency] [float] NOT NULL,
	[Amount_foreign_currency] [float] NOT NULL,
	[Currency_name] [nvarchar](20) NULL,
	[Currency_code] [nvarchar](4) NULL,
	[Currency_rate] [float] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Gs_external_freight_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Gs_external_freight] ([Amount_national_currency], [Amount_foreign_currency], [Currency_name], [Currency_code], [Currency_rate], [Valuation_Id]) VALUES (0, 17, NULL, N'US', 0, 48427)
INSERT [dbo].[xcuda_Gs_external_freight] ([Amount_national_currency], [Amount_foreign_currency], [Currency_name], [Currency_code], [Currency_rate], [Valuation_Id]) VALUES (0, 17, NULL, N'US', 0, 48428)
INSERT [dbo].[xcuda_Gs_external_freight] ([Amount_national_currency], [Amount_foreign_currency], [Currency_name], [Currency_code], [Currency_rate], [Valuation_Id]) VALUES (0, 17, NULL, N'US', 0, 48429)
GO
ALTER TABLE [dbo].[xcuda_Gs_external_freight]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Gs_external_freight_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Gs_external_freight] CHECK CONSTRAINT [FK_xcuda_Gs_external_freight_xcuda_Valuation]
GO
