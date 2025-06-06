USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Principal]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Principal](
	[Principal_Id] [int] NOT NULL,
	[Transit_Id] [int] NULL,
 CONSTRAINT [PK_xcuda_Principal] PRIMARY KEY CLUSTERED 
(
	[Principal_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Principal]  WITH NOCHECK ADD  CONSTRAINT [FK_Transit_Principal] FOREIGN KEY([Transit_Id])
REFERENCES [dbo].[xcuda_Transit] ([Transit_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Principal] CHECK CONSTRAINT [FK_Transit_Principal]
GO
