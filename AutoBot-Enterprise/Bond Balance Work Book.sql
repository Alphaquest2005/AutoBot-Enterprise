USE [AutoBot-EnterpriseDB]
GO

/****** Object:  View [dbo].[BondBalance-PerDocument]    Script Date: 9/21/2020 9:44:01 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[BondBalance-PerDocument]
AS
SELECT ApplicationSettings.BondQuantum, SUM(ISNULL(AsycudaItemDutyLiablity.DutyLiability, PreviousItemsEx.DutyLiablity)) AS TotalDutyLiablity, AsycudaDocument.TotalCIF, 
                /* ApplicationSettings.BondQuantum - SUM(ISNULL(AsycudaItemDutyLiablity.DutyLiability, PreviousItemsEx.DutyLiablity)) AS RemainingBalance,
				need to do this as running total to make sense
				*/
				AsycudaDocument.ApplicationSettingsId, AsycudaDocument.CNumber, AsycudaDocument.AssessmentDate, AsycudaDocument.RegistrationDate, 
                 AsycudaDocument.ReferenceNumber, COUNT(DISTINCT AsycudaItemBasicInfo.Item_Id) AS Lines, AsycudaDocumentCustomsProcedures.CustomsOperation 
FROM    AsycudaDocument INNER JOIN
                 ApplicationSettings ON AsycudaDocument.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId INNER JOIN
                 AsycudaItemBasicInfo ON AsycudaDocument.ASYCUDA_Id = AsycudaItemBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 AsycudaItemDutyLiablity ON AsycudaItemBasicInfo.Item_Id = AsycudaItemDutyLiablity.Item_Id INNER JOIN
                 AsycudaDocumentCustomsProcedures ON AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id LEFT OUTER JOIN
                 PreviousItemsEx ON AsycudaItemBasicInfo.Item_Id = PreviousItemsEx.PreviousItem_Id
WHERE (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse' OR
                 AsycudaDocumentCustomsProcedures.CustomsOperation = 'Exwarehouse') AND (AsycudaDocument.ImportComplete = 1) and (isnull(AsycudaDocument.Cancelled,0) <> 1)
		--and AsycudaDocument.CNumber = '33816'
GROUP BY ApplicationSettings.BondQuantum, AsycudaDocument.ApplicationSettingsId, AsycudaDocument.CNumber, AsycudaDocument.AssessmentDate, AsycudaDocument.RegistrationDate, AsycudaDocument.ReferenceNumber, AsycudaDocument.TotalCIF,AsycudaDocumentCustomsProcedures.CustomsOperation 

GO

select * from 

(
select 'C#' + CNumber as Id, RegistrationDate as Date,CustomsOperation as Type, case when customsoperation = 'Exwarehouse' then TotalDutyLiablity  else TotalDutyLiablity  end as Total from [BondBalance-PerDocument] where ApplicationSettingsId = 2 --order by AssessmentDate, CNumber
--and CNumber = '38691'
) as t
Order by Date, Id

select * from AsycudaDocument where CNumber = '6768'


union
SELECT EntryDataId, EntryDataDate, 'Sales' as Type, Total AS SalesTotal
FROM    SalesData
WHERE (ApplicationSettingsId = 2) -- AND (EntryDataId = 'GB00043236')
GROUP BY EntryDataId, EntryDataDate, Total
--ORDER BY EntryDataDate

select * from AsycudaDocument where Cancelled = 1 and ApplicationSettingsId = 2 order by RegistrationDate