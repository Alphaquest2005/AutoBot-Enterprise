USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[CustomsProcedureEntryDataTypeEX]    Script Date: 4/8/2025 8:33:17 AM ******/
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
