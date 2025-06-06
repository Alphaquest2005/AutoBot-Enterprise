USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Seals]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Seals](
	[Number] [nvarchar](max) NULL,
	[Seals_Id] [int] NOT NULL,
	[Transit_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Seals] PRIMARY KEY CLUSTERED 
(
	[Seals_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Seals]  WITH NOCHECK ADD  CONSTRAINT [FK_Transit_Seals] FOREIGN KEY([Transit_Id])
REFERENCES [dbo].[xcuda_Transit] ([Transit_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Seals] CHECK CONSTRAINT [FK_Transit_Seals]
GO
