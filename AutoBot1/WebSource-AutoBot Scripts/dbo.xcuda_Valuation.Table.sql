USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Valuation]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Valuation](
	[Calculation_working_mode] [nvarchar](20) NULL,
	[Total_cost] [float] NOT NULL,
	[Total_CIF] [float] NOT NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Valuation_1] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Valuation] ([Calculation_working_mode], [Total_cost], [Total_CIF], [ASYCUDA_Id]) VALUES (N'0', 0, 0, 48427)
INSERT [dbo].[xcuda_Valuation] ([Calculation_working_mode], [Total_cost], [Total_CIF], [ASYCUDA_Id]) VALUES (N'0', 0, 0, 48428)
INSERT [dbo].[xcuda_Valuation] ([Calculation_working_mode], [Total_cost], [Total_CIF], [ASYCUDA_Id]) VALUES (N'0', 0, 0, 48429)
GO
ALTER TABLE [dbo].[xcuda_Valuation]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Valuation] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Valuation] CHECK CONSTRAINT [FK_ASYCUDA_Valuation]
GO
