USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_General_information]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_General_information](
	[Value_details] [nvarchar](20) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[CAP] [nvarchar](20) NULL,
	[Additional_information] [nvarchar](20) NULL,
	[Comments_free_text] [nvarchar](255) NULL,
 CONSTRAINT [PK_xcuda_General_information_1] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_General_information]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_General_information] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_General_information] CHECK CONSTRAINT [FK_ASYCUDA_General_information]
GO
