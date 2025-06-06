USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Property]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Property](
	[Sad_flow] [nvarchar](20) NULL,
	[Date_of_declaration] [nvarchar](20) NULL,
	[Selected_page] [nvarchar](20) NULL,
	[ASYCUDA_Id] [int] NOT NULL,
	[Place_of_declaration] [nvarchar](20) NULL,
 CONSTRAINT [PK_xcuda_Property] PRIMARY KEY CLUSTERED 
(
	[ASYCUDA_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Property]  WITH NOCHECK ADD  CONSTRAINT [FK_ASYCUDA_Property] FOREIGN KEY([ASYCUDA_Id])
REFERENCES [dbo].[xcuda_ASYCUDA] ([ASYCUDA_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Property] CHECK CONSTRAINT [FK_ASYCUDA_Property]
GO
