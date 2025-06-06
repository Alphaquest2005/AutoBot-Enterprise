USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[C71SummaryDetails]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[C71SummaryDetails]
AS
SELECT xC71_Seller_segment.Address, xC71_Value_declaration_form.Value_declaration_form_Id, CAST(xC71_Item.Net_Price AS float) AS Total, xC71_Value_declaration_form_Registered.Value_declaration_form_Id AS RegisteredId
FROM    xC71_Seller_segment INNER JOIN
                 xC71_Identification_segment ON xC71_Identification_segment.Identification_segment_Id = xC71_Seller_segment.Identification_segment_Id INNER JOIN
                 xC71_Value_declaration_form ON xC71_Identification_segment.Identification_segment_Id = xC71_Value_declaration_form.Value_declaration_form_Id INNER JOIN
                 xC71_Item ON xC71_Value_declaration_form.Value_declaration_form_Id = xC71_Item.Value_declaration_form_Id LEFT OUTER JOIN
                 xC71_Value_declaration_form_Registered ON xC71_Value_declaration_form.Value_declaration_form_Id = xC71_Value_declaration_form_Registered.Value_declaration_form_Id
GROUP BY xC71_Value_declaration_form.Value_declaration_form_Id, xC71_Seller_segment.Address, xC71_Value_declaration_form_Registered.Value_declaration_form_Id, CAST(xC71_Item.Net_Price AS float)
GO
