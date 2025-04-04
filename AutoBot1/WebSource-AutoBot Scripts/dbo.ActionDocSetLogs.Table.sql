USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[ActionDocSetLogs]    Script Date: 4/3/2025 10:23:54 PM ******/
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
SET IDENTITY_INSERT [dbo].[ActionDocSetLogs] ON 

INSERT [dbo].[ActionDocSetLogs] ([ActonId], [AsycudaDocumentSetId], [Id]) VALUES (106, 7847, 2200)
INSERT [dbo].[ActionDocSetLogs] ([ActonId], [AsycudaDocumentSetId], [Id]) VALUES (106, 7849, 2201)
SET IDENTITY_INSERT [dbo].[ActionDocSetLogs] OFF
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
