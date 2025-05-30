USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[CustomsProcedureEntryDataType]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomsProcedureEntryDataType](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Customs_ProcedureId] [int] NOT NULL,
	[EntryDataType] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CustomsProcedureEntryDataType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CustomsProcedureEntryDataType]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomsProcedureEntryDataType_Customs_Procedure] FOREIGN KEY([Customs_ProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
GO
ALTER TABLE [dbo].[CustomsProcedureEntryDataType] CHECK CONSTRAINT [FK_CustomsProcedureEntryDataType_Customs_Procedure]
GO
ALTER TABLE [dbo].[CustomsProcedureEntryDataType]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomsProcedureEntryDataType_EntryDataType] FOREIGN KEY([EntryDataType])
REFERENCES [dbo].[EntryDataType] ([EntryDataType])
GO
ALTER TABLE [dbo].[CustomsProcedureEntryDataType] CHECK CONSTRAINT [FK_CustomsProcedureEntryDataType_EntryDataType]
GO
