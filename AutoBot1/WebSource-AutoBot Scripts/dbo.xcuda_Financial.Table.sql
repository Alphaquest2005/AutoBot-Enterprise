USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Financial]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Financial](
	[Financial_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
	[Deffered_payment_reference] [nvarchar](100) NULL,
	[Mode_of_payment] [nvarchar](100) NULL,
	[Financial_Code] [nvarchar](100) NULL,
 CONSTRAINT [PK_xcuda_Financial] PRIMARY KEY CLUSTERED 
(
	[Financial_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Financial]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Financial] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Financial] CHECK CONSTRAINT [FK_ASYCUDA_Financial]
GO
