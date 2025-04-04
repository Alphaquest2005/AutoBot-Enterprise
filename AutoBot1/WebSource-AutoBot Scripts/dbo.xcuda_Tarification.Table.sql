USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Tarification]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Tarification](
	[Extended_customs_procedure] [nvarchar](20) NULL,
	[National_customs_procedure] [nvarchar](20) NULL,
	[Item_price] [float] NOT NULL,
	[Item_Id] [int] NOT NULL,
	[Value_item] [nvarchar](20) NULL,
	[Attached_doc_item] [nvarchar](20) NULL,
 CONSTRAINT [PK_xcuda_Tarification] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[xcuda_Tarification] ([Extended_customs_procedure], [National_customs_procedure], [Item_price], [Item_Id], [Value_item], [Attached_doc_item]) VALUES (N'4200', N'000', 119.97000122070313, 1004680, NULL, NULL)
INSERT [dbo].[xcuda_Tarification] ([Extended_customs_procedure], [National_customs_procedure], [Item_price], [Item_Id], [Value_item], [Attached_doc_item]) VALUES (N'4200', N'000', 119.97000122070313, 1004681, NULL, NULL)
INSERT [dbo].[xcuda_Tarification] ([Extended_customs_procedure], [National_customs_procedure], [Item_price], [Item_Id], [Value_item], [Attached_doc_item]) VALUES (N'4200', N'000', 119.97000122070313, 1004682, NULL, NULL)
GO
ALTER TABLE [dbo].[xcuda_Tarification]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Tarification] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Tarification] CHECK CONSTRAINT [FK_Item_Tarification]
GO
