USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Weight]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Weight](
	[Gross_weight] [float] NOT NULL,
	[Valuation_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Weight_1] PRIMARY KEY CLUSTERED 
(
	[Valuation_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Weight] ADD  CONSTRAINT [DF_xcuda_Weight_Gross_weight]  DEFAULT ((0)) FOR [Gross_weight]
GO
ALTER TABLE [dbo].[xcuda_Weight]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Weight_xcuda_Valuation] FOREIGN KEY([Valuation_Id])
REFERENCES [dbo].[xcuda_Valuation] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Weight] CHECK CONSTRAINT [FK_xcuda_Weight_xcuda_Valuation]
GO
