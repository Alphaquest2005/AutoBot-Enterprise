USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[PreviousItemsEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[PreviousItemsEx]
AS
SELECT xcuda_PreviousItem.Packages_number, xcuda_PreviousItem.Previous_Packages_number, xcuda_PreviousItem.Hs_code, xcuda_PreviousItem.Commodity_code, xcuda_PreviousItem.Previous_item_number, 
                 xcuda_PreviousItem.Goods_origin, xcuda_PreviousItem.Net_weight AS Net_weight, xcuda_PreviousItem.Prev_net_weight  AS Prev_net_weight, 
                 xcuda_PreviousItem.Prev_reg_ser, xcuda_PreviousItem.Prev_reg_nbr, xcuda_PreviousItem.Prev_reg_year, xcuda_PreviousItem.Prev_reg_cuo, xcuda_PreviousItem.Suplementary_Quantity, 
                 xcuda_PreviousItem.Preveious_suplementary_quantity, xcuda_PreviousItem.Current_value, xcuda_PreviousItem.Previous_value, xcuda_PreviousItem.Current_item_number, xcuda_PreviousItem.PreviousItem_Id, 
                 xcuda_PreviousItem.ASYCUDA_Id,
				 xcuda_PreviousItem.QtyAllocated,
				 pmatch.PreviousDocumentItemId,
				 ROUND(xcuda_PreviousItem.Current_value, 2) AS RndCurrent_Value, 
                 xcuda_Declarant.number AS ReferenceNumber,
				 xcuda_Registration.number as CNumber,
				 xcuda_Registration.date as RegistrationDate,
				 xcuda_PreviousItem.PreviousItem_Id AS AsycudaDocumentItemId,


				 ISNULL(case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date then xcuda_Registration.Date else xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end, CASE WHEN ismanuallyassessed 
                 = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE dbo.xcuda_Registration.Date END, xcuda_Registration.Date) 
                 ELSE isnull(registrationdate, case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date  then xcuda_Registration.Date else xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end) END) AS AssessmentDate,
				 
				 xcuda_PreviousItem.Prev_decl_HS_spec, 
				 pmatch.SalesFactor,
				 Type_of_declaration + Declaration_gen_procedure_code AS DocumentType,
				 CASE WHEN Customs_Procedure.IsPaid = 1 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid,
				 xcuda_PreviousItem.Prev_decl_HS_spec as ItemNumber, 
                 xcuda_PreviousItem.Previous_item_number AS pLineNumber,
				 asycudadocumentset.ApplicationSettingsId,
				 ISNULL(AsycudaItemDutyLiablity.DutyLiability, 0) AS TotalDutyLiablity,
                 ISNULL(ROUND(AsycudaItemDutyLiablity.DutyLiability * (xcuda_PreviousItem.Suplementary_Quantity / xcuda_PreviousItem.Preveious_suplementary_quantity), 4), 0) AS DutyLiablity,
				 Customs_Procedure.Customs_ProcedureId,
				 Customs_Procedure.CustomsProcedure, xItem.EntryDataType
FROM    xcuda_PreviousItem INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON xcuda_PreviousItem.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 Customs_Procedure ON xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId INNER JOIN
                 CustomsOperations ON Customs_Procedure.CustomsOperationId = CustomsOperations.Id INNER JOIN
				 Document_Type on Customs_Procedure.Document_TypeId = Document_Type.Document_TypeId INNER JOIN
                 xcuda_Declarant ON xcuda_PreviousItem.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id INNER JOIN
                 xcuda_Registration ON xcuda_PreviousItem.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id INNER JOIN
				 AsycudaDocumentSet ON AsycudaDocumentSet.AsycudaDocumentSetId = xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId INNER JOIN
                 [PreviousItemsEx-pItemMatchBasic] AS pmatch ON xcuda_PreviousItem.PreviousItem_Id = pmatch.PreviousItem_Id INNER JOIN
				 xcuda_item AS xItem ON xcuda_PreviousItem.PreviousItem_Id = xItem.Item_Id left outer JOIN
                 AsycudaItemDutyLiablity ON pmatch.PreviousDocumentItemId = AsycudaItemDutyLiablity.Item_Id 				 
WHERE (xcuda_ASYCUDA_ExtendedProperties.Cancelled <> 1) AND (CustomsOperations.Name = 'Warehouse' OR
                 CustomsOperations.Name = 'Exwarehouse') --and xcuda_PreviousItem.PreviousItem_Id = 3594718
GO
