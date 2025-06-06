USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Traders]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Traders](
	[Traders_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Traders] PRIMARY KEY CLUSTERED 
(
	[Traders_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Traders]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Traders_xcuda_ASYCUDA] FOREIGN KEY([Traders_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Traders] CHECK CONSTRAINT [FK_xcuda_Traders_xcuda_ASYCUDA]
GO
