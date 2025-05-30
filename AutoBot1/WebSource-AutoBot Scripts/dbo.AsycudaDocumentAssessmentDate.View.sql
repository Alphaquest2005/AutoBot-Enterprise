USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentAssessmentDate]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[AsycudaDocumentAssessmentDate]
 
AS
SELECT   xcuda_ASYCUDA.ASYCUDA_Id, xcuda_Registration.Number AS CNumber, CASE WHEN ismanuallyassessed = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE dbo.xcuda_Registration.Date END, 
                dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(registrationdate, effectiveregistrationDate) END AS RegistrationDate, 
                ISNULL(CASE WHEN xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date or xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = '01/01/0001' THEN xcuda_Registration.Date ELSE xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate END, 
                CASE WHEN ismanuallyassessed = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE dbo.xcuda_Registration.Date END, xcuda_Registration.Date) ELSE isnull(registrationdate, 
                CASE WHEN xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date or xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = '01/01/0001' THEN xcuda_Registration.Date ELSE xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate END) END) AS AssessmentDate, 
                xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, xcuda_ASYCUDA_ExtendedProperties.Cancelled, xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate, xcuda_ASYCUDA_ExtendedProperties.ImportComplete, 
                xcuda_ASYCUDA_ExtendedProperties.SourceFileName, xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId, AsycudaDocumentSet.ApplicationSettingsId, xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId
FROM      xcuda_ASYCUDA WITH (NOLOCK) INNER JOIN
                xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId LEFT OUTER JOIN
                xcuda_Registration WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
GO
