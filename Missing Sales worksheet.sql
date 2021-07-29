USE [AutoBot-EnterpriseDB]
GO

/****** Object:  View [dbo].[AsycudaDocumentItemEntryDataDetailsA]    Script Date: 7/20/2021 9:09:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER VIEW [dbo].[AsycudaDocumentItemEntryDataDetailsA]
 
AS
SELECT AsycudaDocumentBasicInfo.AsycudaDocumentSetId,AsycudaDocumentBasicInfo.ApplicationSettingsId, dbo.EntryDataDetails.EntryDataDetailsId, dbo.EntryDataDetails.EntryData_Id, dbo.xcuda_Item.Item_Id, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(dbo.EntryDataDetails.LineNumber AS varchar(50)))) 
                 AS [key], dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.AsycudaDocumentBasicInfo.Extended_customs_procedure + '-' + dbo.AsycudaDocumentBasicInfo.National_customs_procedure as CustomsProcedure  , dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, dbo.AsycudaDocumentBasicInfo.ImportComplete, xcuda_item.ASYCUDA_Id,
				 EntryDataTypes.Type as EntryDataType, Commodity_code as Tariffcode
FROM    dbo.EntryDataDetails WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Item WITH (NOLOCK) ON /* dbo.EntryDataDetails.ItemNumber = dbo.xcuda_Item.Free_text_2 AND*/ 
                 --replace(dbo.xcuda_Item.Free_text_1,' ', '') =  replace(dbo.EntryDataDetails.EntryDataId,' ', '') + '|' + RTRIM(LTRIM(CAST(dbo.EntryDataDetails.LineNumber AS varchar(50))))   INNER JOIN
				 dbo.xcuda_Item.PreviousInvoiceKey =  dbo.EntryDataDetails.EntryDataDetailsKey INNER JOIN
                 dbo.AsycudaDocumentBasicInfo WITH (NOLOCK) ON dbo.xcuda_Item.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 dbo.Primary_Supplementary_Unit WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.Primary_Supplementary_Unit.Tarification_Id
				 inner join EntryDataTypes WITH (NOLOCK) on EntryDataDetails.EntryData_Id = EntryDataTypes.EntryData_Id inner join
				 xcuda_HScode WITH (NOLOCK) on dbo.xcuda_Item.Item_Id = dbo.xcuda_HScode.Item_Id
Where CHARINDEX( xcuda_Item.PreviousInvoiceItemNumber ,EntryDataDetails.ItemNumber ) > 0 
--where xcuda_Item.PreviousInvoiceItemNumber like '%' +  EntryDataDetails.ItemNumber + '%' -- 
--where entrydataid like '%122089%' 
GO


SELECT AsycudaDocumentBasicInfo.AsycudaDocumentSetId, AsycudaDocumentBasicInfo.ApplicationSettingsId, EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryData_Id, xcuda_Item.Item_Id, 
                 EntryDataDetails.ItemNumber, EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(EntryDataDetails.LineNumber AS varchar(50)))) AS [key], AsycudaDocumentBasicInfo.DocumentType, 
                 AsycudaDocumentBasicInfo.Extended_customs_procedure + '-' + AsycudaDocumentBasicInfo.National_customs_procedure AS CustomsProcedure, 
                 Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, AsycudaDocumentBasicInfo.ImportComplete, xcuda_Item.ASYCUDA_Id, EntryDataTypes.Type AS EntryDataType, 
                 xcuda_HScode.Commodity_code AS Tariffcode, xcuda_Item.PreviousInvoiceKey, AsycudaDocumentBasicInfo.Reference
FROM    xcuda_HScode WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentBasicInfo WITH (NOLOCK) INNER JOIN
                 xcuda_Item WITH (NOLOCK) ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id INNER JOIN
                 Primary_Supplementary_Unit WITH (NOLOCK) ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id ON xcuda_HScode.Item_Id = xcuda_Item.Item_Id LEFT OUTER JOIN
                 EntryDataTypes WITH (NOLOCK) INNER JOIN
                 EntryDataDetails WITH (NOLOCK) ON EntryDataTypes.EntryData_Id = EntryDataDetails.EntryData_Id ON (xcuda_Item.PreviousInvoiceKey <> EntryDataDetails.EntryDataDetailsKey and xcuda_item.PreviousInvoiceNumber = EntryDataDetails.EntryDataId)
WHERE EntryDataDetails.EntryDataDetailsKey is null and AsycudaDocumentBasicInfo.CustomsProcedure in ('9074-000','4074-000') and ApplicationSettingsId = 2 --and xcuda_HScode.Precision_4 = 'KGP/SB385496BK'

intersect

SELECT AsycudaDocumentBasicInfo.AsycudaDocumentSetId, AsycudaDocumentBasicInfo.ApplicationSettingsId, EntryDataDetails.EntryDataDetailsId, EntryDataDetails.EntryData_Id, xcuda_Item.Item_Id, 
                 xcuda_HScode.Precision_4, EntryDataDetails.EntryDataId + '|' + RTRIM(LTRIM(CAST(EntryDataDetails.LineNumber AS varchar(50)))) AS [key], AsycudaDocumentBasicInfo.DocumentType, 
                 AsycudaDocumentBasicInfo.Extended_customs_procedure + '-' + AsycudaDocumentBasicInfo.National_customs_procedure AS CustomsProcedure, 
                 Primary_Supplementary_Unit.Suppplementary_unit_quantity AS Quantity, AsycudaDocumentBasicInfo.ImportComplete, xcuda_Item.ASYCUDA_Id, EntryDataTypes.Type AS EntryDataType, 
                 xcuda_HScode.Commodity_code AS Tariffcode, xcuda_Item.PreviousInvoiceKey, AsycudaDocumentBasicInfo.Reference
FROM    xcuda_HScode WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentBasicInfo WITH (NOLOCK) INNER JOIN
                 xcuda_Item WITH (NOLOCK) ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id INNER JOIN
                 Primary_Supplementary_Unit WITH (NOLOCK) ON xcuda_Item.Item_Id = Primary_Supplementary_Unit.Tarification_Id ON xcuda_HScode.Item_Id = xcuda_Item.Item_Id LEFT OUTER JOIN
                 EntryDataTypes WITH (NOLOCK) INNER JOIN
                 EntryDataDetails WITH (NOLOCK) ON EntryDataTypes.EntryData_Id = EntryDataDetails.EntryData_Id ON (xcuda_Item.PreviousInvoiceKey = EntryDataDetails.EntryDataDetailsKey /*or xcuda_item.PreviousInvoiceNumber = EntryDataDetails.EntryDataId*/)
WHERE EntryDataDetails.EntryDataDetailsKey is null and AsycudaDocumentBasicInfo.CustomsProcedure in ('9074-000','4074-000') and ApplicationSettingsId = 2-- and xcuda_HScode.Precision_4 = 'KGP/SB385496BK'

select * from xcuda_item where xcuda_Item.PreviousInvoiceKey like '%TR/007147%'
select * from EntryDataDetails where EntryDataDetailsKey like '%TR/007147%'




select * from AsycudaDocument where ASYCUDA_Id in (961,
960,
969,
973,
1007,
13234,
965,
919,
32655,
1008,
1000,
955,
964,
963,
1012,
1009,
953,
42415,
13241,
14828,
42316,
970,
972,
974,
1004,
954,
9830,
932,
968,
13236,
925,
1010,
921,
962,
958,
922,
971,
13235,
13237,
967,
999,
959,
9837,
966,
916,
1006,
975,
920,
956,
944,
948,
930,
3198,
14851,
9833,
3215,
14213,
927,
3196,
938,
9831,
14214,
907,
35890,
935,
9838,
9836,
917,
950,
940,
945,
946,
33981,
1005,
943,
9840,
9826,
1001,
9829,
14216,
9835,
2975,
9828,
928,
933,
9825,
929,
1002,
14315,
14219,
3195,
995,
942,
14316,
2983,
923,
937,
14215,
13911,
9834,
941,
949,
10995,
931,
924,
9827,
936,
37942,
37943,
915,
14217,
918,
952,
9832,
926,
934,
939,
3194,
853,
893
)