USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[EntryData_DocumentType]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryData_DocumentType](
	[DocumentType] [nvarchar](50) NULL,
	[EntryData_Id] [int] NOT NULL,
 CONSTRAINT [PK_EntryData_DocumentType] PRIMARY KEY CLUSTERED 
(
	[EntryData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[EntryData_DocumentType]  WITH NOCHECK ADD  CONSTRAINT [FK_EntryData_DocumentType_EntryData1] FOREIGN KEY([EntryData_Id])
REFERENCES [dbo].[EntryData] ([EntryData_Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[EntryData_DocumentType] CHECK CONSTRAINT [FK_EntryData_DocumentType_EntryData1]
GO
