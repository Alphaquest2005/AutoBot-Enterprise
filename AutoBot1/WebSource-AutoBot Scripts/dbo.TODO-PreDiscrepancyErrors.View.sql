USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PreDiscrepancyErrors]    Script Date: 4/8/2025 8:33:17 AM ******/
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
