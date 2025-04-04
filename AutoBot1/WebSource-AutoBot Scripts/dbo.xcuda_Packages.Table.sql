USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Packages]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Packages](
	[Number_of_packages] [float] NOT NULL,
	[Kind_of_packages_code] [nvarchar](20) NULL,
	[Kind_of_packages_name] [nvarchar](30) NULL,
	[Packages_Id] [int] IDENTITY(1,1) NOT NULL,
	[Item_Id] [int] NULL,
	[Marks1_of_packages] [nvarchar](40) NULL,
	[Marks2_of_packages] [nvarchar](40) NULL,
 CONSTRAINT [PK_xcuda_Packages] PRIMARY KEY CLUSTERED 
(
	[Packages_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[xcuda_Packages] ON 

INSERT [dbo].[xcuda_Packages] ([Number_of_packages], [Kind_of_packages_code], [Kind_of_packages_name], [Packages_Id], [Item_Id], [Marks1_of_packages], [Marks2_of_packages]) VALUES (1, N'PK', NULL, 511241, 1004680, N'Marks', N'Marks')
INSERT [dbo].[xcuda_Packages] ([Number_of_packages], [Kind_of_packages_code], [Kind_of_packages_name], [Packages_Id], [Item_Id], [Marks1_of_packages], [Marks2_of_packages]) VALUES (1, N'PK', NULL, 511242, 1004681, N'Marks', N'Marks')
INSERT [dbo].[xcuda_Packages] ([Number_of_packages], [Kind_of_packages_code], [Kind_of_packages_name], [Packages_Id], [Item_Id], [Marks1_of_packages], [Marks2_of_packages]) VALUES (1, N'PK', NULL, 511243, 1004682, N'Marks', N'Marks')
SET IDENTITY_INSERT [dbo].[xcuda_Packages] OFF
GO
/****** Object:  Index [SQLOPS_xcuda_Packages_22_21]    Script Date: 4/3/2025 10:23:55 PM ******/
CREATE NONCLUSTERED INDEX [SQLOPS_xcuda_Packages_22_21] ON [dbo].[xcuda_Packages]
(
	[Item_Id] ASC
)
INCLUDE([Number_of_packages]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Packages]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Packages] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Packages] CHECK CONSTRAINT [FK_Item_Packages]
GO
