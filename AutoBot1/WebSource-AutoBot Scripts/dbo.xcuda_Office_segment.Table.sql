USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Office_segment]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Office_segment](
	[Customs_clearance_office_code] [nvarchar](20) NULL,
	[Customs_Clearance_office_name] [nvarchar](255) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
 CONSTRAINT [PK_xcuda_Office_segment] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Office_segment]  WITH NOCHECK ADD  CONSTRAINT [FK_Identification_Office_segment] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_Identification] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Office_segment] CHECK CONSTRAINT [FK_Identification_Office_segment]
GO
