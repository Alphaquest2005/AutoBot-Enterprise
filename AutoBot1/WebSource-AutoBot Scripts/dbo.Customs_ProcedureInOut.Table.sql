USE [WebSource-AutoBot]
GO
/****** Object:  Table [dbo].[Customs_ProcedureInOut]    Script Date: 4/8/2025 8:33:18 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customs_ProcedureInOut](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WarehouseCustomsProcedureId] [int] NOT NULL,
	[ExwarehouseCustomsProcedureId] [int] NOT NULL,
 CONSTRAINT [PK_Customs_ProcedureInOut] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Customs_ProcedureInOut]  WITH CHECK ADD  CONSTRAINT [FK_Customs_ProcedureInOut_Customs_Procedure] FOREIGN KEY([WarehouseCustomsProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
GO
ALTER TABLE [dbo].[Customs_ProcedureInOut] CHECK CONSTRAINT [FK_Customs_ProcedureInOut_Customs_Procedure]
GO
ALTER TABLE [dbo].[Customs_ProcedureInOut]  WITH CHECK ADD  CONSTRAINT [FK_Customs_ProcedureInOut_Customs_Procedure1] FOREIGN KEY([ExwarehouseCustomsProcedureId])
REFERENCES [dbo].[Customs_Procedure] ([Customs_ProcedureId])
GO
ALTER TABLE [dbo].[Customs_ProcedureInOut] CHECK CONSTRAINT [FK_Customs_ProcedureInOut_Customs_Procedure1]
GO
