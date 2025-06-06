USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[OCR-End]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OCR-End](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PartId] [int] NOT NULL,
	[RegExId] [int] NOT NULL,
 CONSTRAINT [PK_End] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[OCR-End] ON 

INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (1, 4, 1)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (3, 5, 5)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (5, 7, 12)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (12, 1017, 34)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (15, 1020, 35)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (25, 1030, 74)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (32, 1033, 107)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (43, 1052, 163)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (46, 1077, 237)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (47, 1035, 109)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (48, 1109, 344)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (49, 1017, 60)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (51, 1118, 375)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (53, 1120, 381)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (61, 1175, 516)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (62, 1180, 537)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (64, 1112, 537)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (65, 2260, 537)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (67, 2408, 2325)
INSERT [dbo].[OCR-End] ([Id], [PartId], [RegExId]) VALUES (69, 2412, 2336)
SET IDENTITY_INSERT [dbo].[OCR-End] OFF
GO
ALTER TABLE [dbo].[OCR-End]  WITH CHECK ADD  CONSTRAINT [FK_OCR-End_OCR-Parts] FOREIGN KEY([PartId])
REFERENCES [dbo].[OCR-Parts] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCR-End] CHECK CONSTRAINT [FK_OCR-End_OCR-Parts]
GO
ALTER TABLE [dbo].[OCR-End]  WITH CHECK ADD  CONSTRAINT [FK_OCR-End_OCR-RegularExpressions] FOREIGN KEY([RegExId])
REFERENCES [dbo].[OCR-RegularExpressions] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OCR-End] CHECK CONSTRAINT [FK_OCR-End_OCR-RegularExpressions]
GO
