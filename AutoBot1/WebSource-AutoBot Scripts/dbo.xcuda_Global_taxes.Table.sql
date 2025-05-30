USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Global_taxes]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Global_taxes](
	[Global_taxes_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Global_taxes] PRIMARY KEY CLUSTERED 
(
	[Global_taxes_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Global_taxes]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Global_taxes] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Global_taxes] CHECK CONSTRAINT [FK_ASYCUDA_Global_taxes]
GO
