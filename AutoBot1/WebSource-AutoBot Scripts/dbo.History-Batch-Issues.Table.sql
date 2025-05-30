USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[History-Batch-Issues]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[History-Batch-Issues](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BatchId] [int] NOT NULL,
	[ItemNumber] [nvarchar](50) NOT NULL,
	[Issue] [nvarchar](255) NOT NULL,
	[ApplicationSettingsId] [int] NOT NULL,
 CONSTRAINT [PK_History-Batch-Issues] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[History-Batch-Issues]  WITH NOCHECK ADD  CONSTRAINT [FK_History-Batch-Issues_History-Batches] FOREIGN KEY([BatchId])
REFERENCES [dbo].[History-Batches] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[History-Batch-Issues] CHECK CONSTRAINT [FK_History-Batch-Issues_History-Batches]
GO
