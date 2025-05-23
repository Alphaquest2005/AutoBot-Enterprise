USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Container]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Container](
	[Container_identity] [nvarchar](255) NULL,
	[Container_type] [nvarchar](50) NULL,
	[Empty_full_indicator] [nvarchar](255) NULL,
	[Gross_weight] [float] NULL,
	[Goods_description] [nvarchar](255) NULL,
	[AsycudaDocumentSetId] [int] NOT NULL,
	[TotalValue] [float] NULL,
	[ShipDate] [date] NULL,
	[DeliveryDate] [date] NULL,
	[Seal] [nvarchar](100) NULL,
 CONSTRAINT [PK_Container] PRIMARY KEY CLUSTERED 
(
	[AsycudaDocumentSetId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Container]  WITH CHECK ADD  CONSTRAINT [FK_Container_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Container] CHECK CONSTRAINT [FK_Container_AsycudaDocumentSet]
GO
