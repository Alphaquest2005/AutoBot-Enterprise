USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[xcuda_Tarification]    Script Date: 4/8/2025 8:33:17 AM ******/
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
ALTER TABLE [dbo].[xcuda_Tarification]  WITH NOCHECK ADD  CONSTRAINT [FK_Item_Tarification] FOREIGN KEY([Item_Id])
REFERENCES [dbo].[xcuda_Item] ([Item_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[xcuda_Tarification] CHECK CONSTRAINT [FK_Item_Tarification]
GO
