USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Assessment_notice]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Assessment_notice](
	[Assessment_notice_Id] [int] NOT NULL,
	[ASYCUDA_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Assessment_notice] PRIMARY KEY CLUSTERED 
(
	[Assessment_notice_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Assessment_notice]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Assessment_notice] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Assessment_notice] CHECK CONSTRAINT [FK_ASYCUDA_Assessment_notice]
GO
