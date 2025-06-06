USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Nbers]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Nbers](
	[Number_of_loading_lists] [nvarchar](20) NULL,
	[Total_number_of_items] [nvarchar](20) NULL,
	[Total_number_of_packages] [float] NOT NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Nbers] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Nbers]  WITH NOCHECK ADD  CONSTRAINT [FK_Property_Nbers] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Property] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Nbers] CHECK CONSTRAINT [FK_Property_Nbers]
GO
