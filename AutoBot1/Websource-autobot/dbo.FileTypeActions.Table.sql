USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[FileTypeActions]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileTypeActions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileTypeId] [int] NOT NULL,
	[ActionId] [int] NOT NULL,
	[AssessIM7] [bit] NULL,
	[AssessEX] [bit] NULL,
 CONSTRAINT [PK_FileTypeActions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[FileTypeActions] ON 

INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (232, 93, 43, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (233, 94, 48, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (235, 102, 95, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (236, 103, 97, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (239, 107, 1, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (265, 114, 69, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (280, 109, 15, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (281, 109, 40, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (282, 109, 42, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (283, 109, 47, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (284, 109, 41, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (285, 109, 44, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (286, 109, 36, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (287, 109, 49, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (288, 109, 38, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (289, 109, 34, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (290, 109, 39, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (291, 109, 35, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (292, 109, 37, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (293, 109, 50, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (294, 109, 7, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (295, 109, 51, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (296, 109, 106, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (297, 109, 8, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1324, 1126, 15, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1325, 1126, 53, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1326, 1127, 15, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1327, 1127, 104, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1329, 109, 10, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1333, 1141, 13, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1337, 1143, 6, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1338, 1145, 6, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1339, 1144, 52, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1340, 1148, 111, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1341, 1151, 13, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1342, 1152, 69, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1345, 1158, 112, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1359, 1146, 13, NULL, NULL)
INSERT [dbo].[FileTypeActions] ([Id], [FileTypeId], [ActionId], [AssessIM7], [AssessEX]) VALUES (1371, 1173, 117, NULL, NULL)
SET IDENTITY_INSERT [dbo].[FileTypeActions] OFF
GO
ALTER TABLE [dbo].[FileTypeActions]  WITH CHECK ADD  CONSTRAINT [FK_FileTypeActions_Actions] FOREIGN KEY([ActionId])
REFERENCES [dbo].[Actions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileTypeActions] CHECK CONSTRAINT [FK_FileTypeActions_Actions]
GO
ALTER TABLE [dbo].[FileTypeActions]  WITH NOCHECK ADD  CONSTRAINT [FK_FileTypeActions_FileTypes] FOREIGN KEY([FileTypeId])
REFERENCES [dbo].[FileTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FileTypeActions] CHECK CONSTRAINT [FK_FileTypeActions_FileTypes]
GO
