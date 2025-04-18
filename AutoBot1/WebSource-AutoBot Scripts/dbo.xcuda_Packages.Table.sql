USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Packages]    Script Date: 4/8/2025 8:33:17 AM ******/
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
/****** Object:  Index [SQLOPS_xcuda_Packages_22_21]    Script Date: 4/8/2025 8:33:18 AM ******/
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
