USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[CustomsProcedureEntryDataTypeEX]    Script Date: 3/27/2025 1:48:23 AM ******/
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
