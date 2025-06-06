USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmailMappingActions]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailMappingActions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailMappingId] [int] NOT NULL,
	[ActionId] [int] NOT NULL,
 CONSTRAINT [PK_EmailMappingActions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[EmailMappingActions] ON 

INSERT [dbo].[EmailMappingActions] ([Id], [EmailMappingId], [ActionId]) VALUES (2, 39, 120)
INSERT [dbo].[EmailMappingActions] ([Id], [EmailMappingId], [ActionId]) VALUES (3, 43, 120)
SET IDENTITY_INSERT [dbo].[EmailMappingActions] OFF
GO
ALTER TABLE [dbo].[EmailMappingActions]  WITH CHECK ADD  CONSTRAINT [FK_EmailMappingActions_Actions] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Actions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailMappingActions] CHECK CONSTRAINT [FK_EmailMappingActions_Actions]
GO
ALTER TABLE [dbo].[EmailMappingActions]  WITH CHECK ADD  CONSTRAINT [FK_EmailMappingActions_EmailMapping] FOREIGN KEY([EmailMappingId])
REFERENCES [dbo].[EmailMapping] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailMappingActions] CHECK CONSTRAINT [FK_EmailMappingActions_EmailMapping]
GO
