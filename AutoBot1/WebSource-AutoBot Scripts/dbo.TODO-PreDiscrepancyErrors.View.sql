USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PreDiscrepancyErrors]    Script Date: 4/3/2025 10:23:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE view [dbo].[TODO-PreDiscrepancyErrors]
as
Select * from [dbo].[TODO-PreDiscrepancyErrors-A]

union 
Select * from [dbo].[TODO-PreDiscrepancyErrors-B]
GO
