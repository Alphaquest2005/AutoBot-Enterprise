USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-EntriesExpiringNextMonth-AsycudaDocumentItem]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[TODO-EntriesExpiringNextMonth-AsycudaDocumentItem]
AS
SELECT xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id AS AsycudaDocumentId, xcuda_Item.IsAssessed, CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) AS ItemQuantity, 
                 Primary_Supplementary_Unit.Suppplementary_unit_code, xcuda_Item.ImportComplete, PiQuantities.PiWeight, PiQuantities.PiQuantity
FROM     dbo.[AscyudaItemPiQuantity-Basic]  AS PiQuantities  WITH (NOLOCK) RIGHT OUTER JOIN
                     (SELECT Item_Id, ASYCUDA_Id, LineNumber,ImportComplete, IsAssessed
                      FROM     xcuda_Item AS xcuda_Item_1 WITH (NOLOCK)                      ) AS xcuda_Item INNER JOIN
                 Primary_Supplementary_Unit WITH (NOLOCK) ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id ON PiQuantities.Item_Id = xcuda_Item.Item_Id
--GROUP BY xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id, xcuda_Item.LineNumber, Primary_Supplementary_Unit.Suppplementary_unit_quantity, Primary_Supplementary_Unit.Suppplementary_unit_code, 
--                 xcuda_Item.IsAssessed, xcuda_Item.ImportComplete, PiQuantities.PiWeight, PiQuantities.PiQuantity, xcuda_Item.LineNumber

GO
