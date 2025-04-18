USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[BondBalance-PerDocumentUnRegistered]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[BondBalance-PerDocumentUnRegistered]
AS
SELECT ApplicationSettings.BondQuantum, SUM(ISNULL(AsycudaItemDutyLiablity.DutyLiability, PreviousItemsEx.DutyLiablity)) AS TotalDutyLiablity, AsycudaDocumentTotalCIF.TotalCIF, 
                /* ApplicationSettings.BondQuantum - SUM(ISNULL(AsycudaItemDutyLiablity.DutyLiability, PreviousItemsEx.DutyLiablity)) AS RemainingBalance,
				need to do this as running total to make sense
				*/
			   AsycudaDocument.ApplicationSettingsId, AsycudaDocument.CNumber, AsycudaDocument.AssessmentDate, AsycudaDocument.RegistrationDate, 
                 AsycudaDocument.Reference as ReferenceNumber, COUNT(DISTINCT AsycudaItemBasicInfo.Item_Id) AS Lines, AsycudaDocumentCustomsProcedures.CustomsOperation 
FROM    AsycudaDocumentBasicInfo as AsycudaDocument INNER JOIN
                 ApplicationSettings ON AsycudaDocument.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId INNER JOIN
                 AsycudaItemBasicInfo ON AsycudaDocument.ASYCUDA_Id = AsycudaItemBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                 AsycudaItemDutyLiablity ON AsycudaItemBasicInfo.Item_Id = AsycudaItemDutyLiablity.Item_Id INNER JOIN
                 AsycudaDocumentCustomsProcedures ON AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id LEFT OUTER JOIN
                 PreviousItemsEx ON AsycudaItemBasicInfo.Item_Id = PreviousItemsEx.PreviousItem_Id
				 inner join AsycudaDocumentTotalCIF on AsycudaDocument.ASYCUDA_Id = AsycudaDocumentTotalCIF.ASYCUDA_Id
WHERE (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse' OR
                 AsycudaDocumentCustomsProcedures.CustomsOperation = 'Exwarehouse') AND (AsycudaDocument.ImportComplete = 0) and (isnull(AsycudaDocument.Cancelled,0) <> 1)
		--and AsycudaDocument.CNumber = '33816'
GROUP BY ApplicationSettings.BondQuantum, AsycudaDocument.ApplicationSettingsId, AsycudaDocument.CNumber, AsycudaDocument.AssessmentDate, AsycudaDocument.RegistrationDate, AsycudaDocument.Reference, AsycudaDocumentTotalCIF.TotalCIF,AsycudaDocumentCustomsProcedures.CustomsOperation
GO
