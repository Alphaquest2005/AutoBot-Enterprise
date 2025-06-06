USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetAttachementExpected]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-PODocSetAttachementExpected]
AS
SELECT AsycudaDocumentSetId, COUNT(Code) AS ExpectedAttachments
FROM    (SELECT AsycudaDocumentSetId, ASYCUDA_Id, Code, ReferenceNumber, ImportComplete, Id
FROM    [AsycudaDocumentAttachments-Required]
WHERE (ImportComplete = 0)
GROUP BY AsycudaDocumentSetId, ASYCUDA_Id, Code, ReferenceNumber, Id, ImportComplete
                 UNION all
                 SELECT AsycudaDocumentSetId, ASYCUDA_Id, Code, Reference, ImportComplete,cast(Item_Id as nvarchar(50)) as Id
FROM    [AsycudaDocumentItemAttachments-Required]
WHERE (ImportComplete = 0)
GROUP BY AsycudaDocumentSetId, ASYCUDA_Id, Code, Reference, Item_Id , ImportComplete) AS required
GROUP BY AsycudaDocumentSetId
GO
