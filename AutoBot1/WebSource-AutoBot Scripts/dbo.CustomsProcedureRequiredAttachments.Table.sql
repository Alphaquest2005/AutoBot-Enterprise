USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[CustomsProcedureRequiredAttachments]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomsProcedureRequiredAttachments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Customs_ProcedureId] [int] NOT NULL,
	[AttachmentCodeId] [int] NOT NULL,
 CONSTRAINT [PK_CustomsProcedureRequiredAttachments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[CustomsProcedureRequiredAttachments] ON 

INSERT [dbo].[CustomsProcedureRequiredAttachments] ([Id], [Customs_ProcedureId], [AttachmentCodeId]) VALUES (17, 143, 3)
INSERT [dbo].[CustomsProcedureRequiredAttachments] ([Id], [Customs_ProcedureId], [AttachmentCodeId]) VALUES (18, 143, 1)
INSERT [dbo].[CustomsProcedureRequiredAttachments] ([Id], [Customs_ProcedureId], [AttachmentCodeId]) VALUES (19, 143, 2)
INSERT [dbo].[CustomsProcedureRequiredAttachments] ([Id], [Customs_ProcedureId], [AttachmentCodeId]) VALUES (20, 143, 6)
INSERT [dbo].[CustomsProcedureRequiredAttachments] ([Id], [Customs_ProcedureId], [AttachmentCodeId]) VALUES (21, 143, 4)
INSERT [dbo].[CustomsProcedureRequiredAttachments] ([Id], [Customs_ProcedureId], [AttachmentCodeId]) VALUES (22, 143, 5)
SET IDENTITY_INSERT [dbo].[CustomsProcedureRequiredAttachments] OFF
GO
ALTER TABLE [dbo].[CustomsProcedureRequiredAttachments]  WITH CHECK ADD  CONSTRAINT [FK_CustomsProcedureRequiredAttachments_AttachmentCodes] FOREIGN KEY([AttachmentCodeId])
REFERENCES [dbo].[AttachmentCodes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CustomsProcedureRequiredAttachments] CHECK CONSTRAINT [FK_CustomsProcedureRequiredAttachments_AttachmentCodes]
GO
ALTER TABLE [dbo].[CustomsProcedureRequiredAttachments]  WITH CHECK ADD  CONSTRAINT [FK_CustomsProcedureRequiredAttachments_Customs_Procedure] FOREIGN KEY([Customs_ProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CustomsProcedureRequiredAttachments] CHECK CONSTRAINT [FK_CustomsProcedureRequiredAttachments_Customs_Procedure]
GO
