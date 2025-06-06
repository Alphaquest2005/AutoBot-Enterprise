USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Identification]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Identification](
	[ASYCUDA_Id] [int] NOT NULL,
	[Manifest_reference_number] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Identification] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Identification]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Identification] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Identification] CHECK CONSTRAINT [FK_ASYCUDA_Identification]
GO
