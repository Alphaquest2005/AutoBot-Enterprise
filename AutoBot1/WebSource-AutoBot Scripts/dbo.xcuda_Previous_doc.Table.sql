USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Previous_doc]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[xcuda_Previous_doc](
	[Summary_declaration] [nvarchar](255) NULL,
	[Item_Id] [int] NOT NULL,
	[Previous_document_reference] [nvarchar](50) NULL,
	[Previous_warehouse_code] [nvarchar](50) NULL,
 CONSTRAINT [PK_xcuda_Previous_doc_1] PRIMARY KEY CLUSTERED 
(
	[Item_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[xcuda_Previous_doc]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Previous_doc] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Previous_doc] CHECK CONSTRAINT [FK_Item_Previous_doc]
GO
