USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Financial_Amounts]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Financial_Amounts](
	[Amounts_Id] [int] IDENTITY(1,1) NOT NULL,
	[Financial_Id] [int] NOT NULL,
	[Total_manual_taxes] [decimal](15, 4) NULL,
	[Global_taxes] [decimal](15, 4) NULL,
	[Totals_taxes] [decimal](15, 4) NULL,
 CONSTRAINT [PK_xcuda_Financial_Amounts] PRIMARY KEY CLUSTERED 
(
	[Amounts_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Financial_Amounts]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Financialxcuda_Financial_Amounts] FOREIGN KEY([Financial_Id])
REFERENCES [dbo].[xcuda_Financial] ([Financial_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Financial_Amounts] CHECK CONSTRAINT [FK_xcuda_Financialxcuda_Financial_Amounts]
GO
