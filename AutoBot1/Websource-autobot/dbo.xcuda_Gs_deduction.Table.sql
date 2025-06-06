USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Gs_deduction]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Gs_deduction](
	[Amount_national_currency] [float] NOT NULL,
	[Amount_foreign_currency] [float] NOT NULL,
	[Currency_name] [nvarchar](20) NULL,
	[Currency_rate] [float] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
	[Currency_code] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Gs_deduction_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Gs_deduction]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Gs_deduction_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Gs_deduction] CHECK CONSTRAINT [FK_xcuda_Gs_deduction_xcuda_Valuation]
GO
