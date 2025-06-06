USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[CustomsOperations]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomsOperations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CustomsOperations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[CustomsOperations] ON 

INSERT [dbo].[CustomsOperations] ([Id], [Name]) VALUES (1, N'Import')
INSERT [dbo].[CustomsOperations] ([Id], [Name]) VALUES (2, N'Warehouse')
INSERT [dbo].[CustomsOperations] ([Id], [Name]) VALUES (3, N'Exwarehouse')
INSERT [dbo].[CustomsOperations] ([Id], [Name]) VALUES (4, N'Export')
SET IDENTITY_INSERT [dbo].[CustomsOperations] OFF
GO
