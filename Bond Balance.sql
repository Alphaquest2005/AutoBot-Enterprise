/****** Script for SelectTopNRows command from SSMS  ******/
SELECT        AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.AssessmentDate, AsycudaDocumentBasicInfo.Reference, 
                         AsycudaDocumentBasicInfo.ExpiryDate, SUM(AsycudaItemDutyLiablity.DutyLiability) AS TotalDutyLiablity
FROM            AsycudaItemBasicInfo INNER JOIN
                         AsycudaDocumentBasicInfo ON AsycudaItemBasicInfo.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                         AsycudaItemDutyLiablity ON AsycudaItemBasicInfo.Item_Id = AsycudaItemDutyLiablity.Item_Id
GROUP BY AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.AssessmentDate, AsycudaDocumentBasicInfo.Reference, 
                         AsycudaDocumentBasicInfo.ExpiryDate
HAVING        (AsycudaDocumentBasicInfo.Reference = N'OPS-2018-08-31-F15')



SELECT        ApplicationSettings.BondQuantum, SUM(AsycudaItemDutyLiablity.DutyLiability) AS TotalDutyLiablity, ApplicationSettings.BondQuantum - SUM(AsycudaItemDutyLiablity.DutyLiability) AS RemainingBalance
FROM            AsycudaItemBasicInfo INNER JOIN
                         AsycudaDocumentBasicInfo ON AsycudaItemBasicInfo.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id LEFT OUTER JOIN
                         AsycudaItemDutyLiablity ON AsycudaItemBasicInfo.Item_Id = AsycudaItemDutyLiablity.Item_Id CROSS JOIN
                         ApplicationSettings
WHERE        (AsycudaDocumentBasicInfo.DocumentType = 'OS7') AND (AsycudaDocumentBasicInfo.IsManuallyAssessed IS NULL) OR
                         (AsycudaDocumentBasicInfo.DocumentType = 'IM7') AND (AsycudaDocumentBasicInfo.IsManuallyAssessed = 0)
GROUP BY ApplicationSettings.BondQuantum