USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[CustomsProcedureEntryDataTypeEX]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

create VIEW [dbo].[CustomsProcedureEntryDataTypeEX]
AS
SELECT        dbo.Customs_Procedure.CustomsProcedure, dbo.CustomsProcedureEntryDataType.EntryDataType
FROM            dbo.CustomsProcedureEntryDataType INNER JOIN
                         dbo.Customs_Procedure ON dbo.CustomsProcedureEntryDataType.Customs_ProcedureId = dbo.Customs_Procedure.Customs_ProcedureId
GO
