USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Assessment]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Assessment](
	[Number] [nvarchar](6) NULL,
	[Date] [nvarchar](10) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Assessment] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Assessment]  WITH NOCHECK ADD  CONSTRAINT [FK_Identification_Assessment] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Assessment] CHECK CONSTRAINT [FK_Identification_Assessment]
GO
