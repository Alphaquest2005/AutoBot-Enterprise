USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PreDiscrepancyErrors]    Script Date: 3/27/2025 1:48:24 AM ******/
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
