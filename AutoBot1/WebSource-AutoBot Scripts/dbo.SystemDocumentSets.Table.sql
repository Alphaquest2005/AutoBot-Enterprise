USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[SystemDocumentSets]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemDocumentSets](
	[Id] [int] NOT NULL,
 CONSTRAINT [PK_SystemDocumentSets] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[SystemDocumentSets] ([Id]) VALUES (5381)
INSERT [dbo].[SystemDocumentSets] ([Id]) VALUES (5428)
GO
ALTER TABLE [dbo].[SystemDocumentSets]  WITH CHECK ADD  CONSTRAINT [FK_SystemDocumentSets_AsycudaDocumentSet] FOREIGN KEY([Id])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
GO
ALTER TABLE [dbo].[SystemDocumentSets] CHECK CONSTRAINT [FK_SystemDocumentSets_AsycudaDocumentSet]
GO
