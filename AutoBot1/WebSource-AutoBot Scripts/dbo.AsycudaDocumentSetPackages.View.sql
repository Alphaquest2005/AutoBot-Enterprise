USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetPackages]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AsycudaDocumentSetPackages]
AS
SELECT  xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, sum(xcuda_Packages.Number_of_packages) AS Packages
FROM    xcuda_item inner join   xcuda_ASYCUDA_ExtendedProperties ON xcuda_item.Asycuda_id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_Packages ON xcuda_item.Item_Id = xcuda_Packages.Item_Id
where xcuda_Packages.Number_of_packages is not null
GROUP BY xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId
GO
