USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Export_release]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Export_release](
	[Date_of_exit] [nvarchar](20) NULL,
	[Time_of_exit] [nvarchar](20) NULL,
	[Export_release_Id] [int] IDENTITY(1,1) NOT NULL,
	[ASYCUDA_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Export_release] PRIMARY KEY CLUSTERED 
(
	[Export_release_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Export_release]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Export_release] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Export_release] CHECK CONSTRAINT [FK_ASYCUDA_Export_release]
GO
