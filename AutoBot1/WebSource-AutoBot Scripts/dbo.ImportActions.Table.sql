USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ImportActions]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportActions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileTypeId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Action] [nvarchar](500) NOT NULL,
 CONSTRAINT [PK_ImportActions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ImportActions]  WITH NOCHECK ADD  CONSTRAINT [FK_ImportActions_FileTypes] FOREIGN KEY([FileTypeId])
REFERENCES [dbo].[FileTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ImportActions] CHECK CONSTRAINT [FK_ImportActions_FileTypes]
GO
