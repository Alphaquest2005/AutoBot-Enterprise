USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Licences]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Licences](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Year] [nvarchar](4) NOT NULL,
	[LicenceNumber] [nvarchar](50) NOT NULL,
	[QtyAllocated] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[TariffCateoryCode] [nvarchar](16) NULL,
	[AsycudaDocumentSetId] [int] NULL,
 CONSTRAINT [PK_Licences] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Licences]  WITH NOCHECK ADD  CONSTRAINT [FK_Licences_AsycudaDocumentSet] FOREIGN KEY([AsycudaDocumentSetId])
REFERENCES [dbo].[AsycudaDocumentSet] ([AsycudaDocumentSetId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Licences] CHECK CONSTRAINT [FK_Licences_AsycudaDocumentSet]
GO
