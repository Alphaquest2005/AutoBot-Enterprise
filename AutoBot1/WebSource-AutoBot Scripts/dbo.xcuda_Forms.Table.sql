USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Forms]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Forms](
	[ASYCUDA_Id] [int] NOT NULL,
	[Number_of_the_form] [int] NULL,
	[Total_number_of_forms] [int] NULL,
 CONSTRAINT [PK_xcuda_Forms] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Forms]  WITH NOCHECK ADD  CONSTRAINT [FK_xcuda_Forms_xcuda_Property] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Property] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Forms] CHECK CONSTRAINT [FK_xcuda_Forms_xcuda_Property]
GO
