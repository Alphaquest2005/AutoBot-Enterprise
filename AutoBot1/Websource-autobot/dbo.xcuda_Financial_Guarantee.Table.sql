USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Financial_Guarantee]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Financial_Guarantee](
	[Guarantee_Id] [int] IDENTITY(1,1) NOT NULL,
	[Financial_Id] [int] NOT NULL,
	[Amount] [decimal](15, 4) NULL,
	[Date] [datetime2](7) NULL,
 CONSTRAINT [PK_xcuda_Financial_Guarantee] PRIMARY KEY CLUSTERED 
(
	[Guarantee_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Financial_Guarantee]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Financialxcuda_Financial_Guarantee] FOREIGN KEY([Financial_Id])
REFERENCES [dbo].[xcuda_Financial] ([Financial_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Financial_Guarantee] CHECK CONSTRAINT [FK_xcuda_Financialxcuda_Financial_Guarantee]
GO
