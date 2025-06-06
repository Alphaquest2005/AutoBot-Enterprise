USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Document_Type]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Document_Type](
	[Document_TypeId] [int] IDENTITY(1,1) NOT NULL,
	[Type_of_declaration] [nvarchar](10) NULL,
	[Declaration_gen_procedure_code] [nvarchar](10) NULL,
	[NeedsC71] [bit] NULL,
 CONSTRAINT [PK_Document_Type] PRIMARY KEY CLUSTERED 
(
	[Document_TypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Document_Type] ON 

INSERT [dbo].[Document_Type] ([Document_TypeId], [Type_of_declaration], [Declaration_gen_procedure_code], [NeedsC71]) VALUES (1, N'IM', N'7', NULL)
INSERT [dbo].[Document_Type] ([Document_TypeId], [Type_of_declaration], [Declaration_gen_procedure_code], [NeedsC71]) VALUES (4, N'IM', N'4', NULL)
INSERT [dbo].[Document_Type] ([Document_TypeId], [Type_of_declaration], [Declaration_gen_procedure_code], [NeedsC71]) VALUES (35, N'IM', N'5', NULL)
INSERT [dbo].[Document_Type] ([Document_TypeId], [Type_of_declaration], [Declaration_gen_procedure_code], [NeedsC71]) VALUES (40, N'EX', N'1', NULL)
INSERT [dbo].[Document_Type] ([Document_TypeId], [Type_of_declaration], [Declaration_gen_procedure_code], [NeedsC71]) VALUES (41, N'IM', N'8', NULL)
INSERT [dbo].[Document_Type] ([Document_TypeId], [Type_of_declaration], [Declaration_gen_procedure_code], [NeedsC71]) VALUES (42, N'EX', N'8', NULL)
INSERT [dbo].[Document_Type] ([Document_TypeId], [Type_of_declaration], [Declaration_gen_procedure_code], [NeedsC71]) VALUES (43, N'SD', N'4', NULL)
INSERT [dbo].[Document_Type] ([Document_TypeId], [Type_of_declaration], [Declaration_gen_procedure_code], [NeedsC71]) VALUES (44, N'SE', N'4', NULL)
SET IDENTITY_INSERT [dbo].[Document_Type] OFF
GO
