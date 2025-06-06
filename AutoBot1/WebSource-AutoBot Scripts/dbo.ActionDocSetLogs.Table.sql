USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ActionDocSetLogs]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActionDocSetLogs](
	[ActonId] [int] NOT NULL,
	[AsycudaDocumentSetId] [int] NOT NULL,
	[Id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_ActionDocSetLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[ActionDocSetLogs]  WITH CHECK ADD  CONSTRAINT [FK_ActionDocSetLogs_Actions] FOREIGN KEY([ActonId])
REFERENCES [dbo].[Actions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ActionDocSetLogs] CHECK CONSTRAINT [FK_ActionDocSetLogs_Actions]
GO
ALTER TABLE [dbo].[ActionDocSetLogs]  WITH CHECK ADD  CONSTRAINT [FK_ActionDocSetLogs_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ActionDocSetLogs] CHECK CONSTRAINT [FK_ActionDocSetLogs_AsycudaDocumentSet]
GO
