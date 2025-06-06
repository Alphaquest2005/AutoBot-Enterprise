USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EmailInfoMappings]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EmailInfoMappings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailMappingId] [int] NOT NULL,
	[InfoMappingId] [int] NOT NULL,
	[UpdateDatabase] [bit] NULL,
 CONSTRAINT [PK_EmailInfoMappings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[EmailInfoMappings] ON 

INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (38, 22, 1024, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (39, 22, 1026, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (40, 22, 1027, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (41, 22, 1028, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (42, 22, 1029, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (43, 22, 1030, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (44, 22, 1036, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (45, 22, 1037, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (46, 22, 1038, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (47, 22, 1039, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (48, 22, 1040, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (49, 22, 1042, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (50, 22, 1044, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (51, 22, 1046, 1)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (52, 22, 1047, 0)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (53, 22, 1048, 0)
INSERT [dbo].[EmailInfoMappings] ([Id], [EmailMappingId], [InfoMappingId], [UpdateDatabase]) VALUES (54, 22, 1045, 1)
SET IDENTITY_INSERT [dbo].[EmailInfoMappings] OFF
GO
ALTER TABLE [dbo].[EmailInfoMappings]  WITH CHECK ADD  CONSTRAINT [FK_EmailInfoMappings_EmailMapping] FOREIGN KEY([EmailMappingId])
REFERENCES [dbo].[EmailMapping] ([Id])
GO
ALTER TABLE [dbo].[EmailInfoMappings] CHECK CONSTRAINT [FK_EmailInfoMappings_EmailMapping]
GO
ALTER TABLE [dbo].[EmailInfoMappings]  WITH CHECK ADD  CONSTRAINT [FK_EmailInfoMappings_InfoMapping] FOREIGN KEY([InfoMappingId])
REFERENCES [dbo].[InfoMapping] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EmailInfoMappings] CHECK CONSTRAINT [FK_EmailInfoMappings_InfoMapping]
GO
