USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[PreProcessShipmentSP]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[PreProcessShipmentSP] as
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
					delete from ShipmentInvoicePOs

					INSERT INTO ShipmentInvoicePOs
                                    (EntryData_Id, InvoiceId)
                    SELECT EntryData_PurchaseOrders.EntryData_Id, ShipmentInvoice.Id--, EntryData_PurchaseOrders.PONumber, ShipmentInvoice.InvoiceNo
					FROM    ShipmentInvoicePOManualMatches INNER JOIN
									 EntryData_PurchaseOrders ON ShipmentInvoicePOManualMatches.PONumber = EntryData_PurchaseOrders.PONumber INNER JOIN
									 ShipmentInvoice ON ShipmentInvoicePOManualMatches.InvoiceNo = ShipmentInvoice.InvoiceNo INNER JOIN
									 EntryData ON EntryData_PurchaseOrders.EntryData_Id = EntryData.EntryData_Id AND ShipmentInvoice.ApplicationSettingsId = EntryData.ApplicationSettingsId LEFT OUTER JOIN
									 ShipmentInvoicePOs ON EntryData_PurchaseOrders.EntryData_Id = ShipmentInvoicePOs.EntryData_Id AND ShipmentInvoice.Id = ShipmentInvoicePOs.InvoiceId
					WHERE (ShipmentInvoicePOs.Id IS NULL)-- and EntryData_PurchaseOrders.PONumber in ('02986', '02989')

					 INSERT INTO ShipmentInvoicePOs
                                        (EntryData_Id, InvoiceId)
                    SELECT [ShipmentInvoicePOMatches-InvoiceNo].EntryData_Id, [ShipmentInvoicePOMatches-InvoiceNo].InvoiceId--, [ShipmentInvoicePOMatches-InvoiceNo].PONumber, [ShipmentInvoicePOMatches-InvoiceNo].InvoiceNo
                    FROM    [ShipmentInvoicePOMatches-InvoiceNo] LEFT OUTER JOIN
	                    ShipmentInvoicePOs ON [ShipmentInvoicePOMatches-InvoiceNo].EntryData_Id = ShipmentInvoicePOs.EntryData_Id or [ShipmentInvoicePOMatches-InvoiceNo].InvoiceId = ShipmentInvoicePOs.InvoiceId
                    WHERE (ShipmentInvoicePOs.Id IS NULL) --and [ShipmentInvoicePOMatches-InvoiceNo].PONumber in ('02986', '02989')
                   
                    INSERT INTO ShipmentInvoicePOs
                                        (EntryData_Id, InvoiceId)
                    SELECT [ShipmentInvoicePOMatches-Items].POId, [ShipmentInvoicePOMatches-Items].InvId--, [ShipmentInvoicePOMatches-Items].PONumber, [ShipmentInvoicePOMatches-Items].InvoiceNo
                    FROM    [ShipmentInvoicePOMatches-Items] LEFT OUTER JOIN
	                    ShipmentInvoicePOs ON [ShipmentInvoicePOMatches-Items].POId = ShipmentInvoicePOs.EntryData_Id or [ShipmentInvoicePOMatches-Items].InvId = ShipmentInvoicePOs.InvoiceId
                    WHERE (ShipmentInvoicePOs.Id IS NULL) -- and [ShipmentInvoicePOMatches-Items].PONumber in ('02986', '02989')

				

					 INSERT INTO ShipmentInvoicePOs
                                        (EntryData_Id, InvoiceId)
                    SELECT distinct ShipmentPOs.EntryData_Id, ShipmentInvoice.Id--, EntryData.EntryDataId, ShipmentInvoice.InvoiceNo
                    FROM    ShipmentInvoicePOMatches INNER JOIN
	                    ShipmentPOs ON ShipmentInvoicePOMatches.PONumber = ShipmentPOs.InvoiceNo INNER JOIN
	                    ShipmentInvoice ON ShipmentInvoicePOMatches.InvoiceNo = ShipmentInvoice.InvoiceNo INNER JOIN
	                    EntryData ON ShipmentPOs.EntryData_Id = EntryData.EntryData_Id AND ShipmentInvoice.ApplicationSettingsId = EntryData.ApplicationSettingsId LEFT OUTER JOIN
	                    ShipmentInvoicePOs ON ShipmentPOs.EntryData_Id = ShipmentInvoicePOs.EntryData_Id or ShipmentInvoice.Id = ShipmentInvoicePOs.InvoiceId
                    WHERE (ShipmentInvoicePOs.Id IS NULL) --and EntryData.EntryDataId in ('02986', '02989')


					 INSERT INTO ShipmentInvoicePOs
                                        (EntryData_Id, InvoiceId)
                    SELECT [ShipmentInvoicePOMatches-InvoiceTotals].EntryData_id, [ShipmentInvoicePOMatches-InvoiceTotals].InvoiceId--, [ShipmentInvoicePOMatches-InvoiceTotals].PONumber, [ShipmentInvoicePOMatches-InvoiceTotals].InvoiceNo
                    FROM    [ShipmentInvoicePOMatches-InvoiceTotals] LEFT OUTER JOIN
	                    ShipmentInvoicePOs ON [ShipmentInvoicePOMatches-InvoiceTotals].EntryData_id = ShipmentInvoicePOs.EntryData_Id or [ShipmentInvoicePOMatches-InvoiceTotals].InvoiceId = ShipmentInvoicePOs.InvoiceId
                    WHERE (ShipmentInvoicePOs.Id IS NULL) -- and [ShipmentInvoicePOMatches-InvoiceTotals].PONumber in ('02986', '02989')

					     INSERT INTO ShipmentInvoicePOs
                                        (EntryData_Id, InvoiceId)
                    SELECT [ShipmentInvoicePOMatches-Totals].EntryData_id, [ShipmentInvoicePOMatches-Totals].InvoiceId--, [ShipmentInvoicePOMatches-Totals].PONumber, [ShipmentInvoicePOMatches-Totals].InvoiceNo
                    FROM    [ShipmentInvoicePOMatches-Totals] LEFT OUTER JOIN
	                    ShipmentInvoicePOs ON [ShipmentInvoicePOMatches-Totals].EntryData_id = ShipmentInvoicePOs.EntryData_Id or [ShipmentInvoicePOMatches-Totals].InvoiceId = ShipmentInvoicePOs.InvoiceId
                    WHERE (ShipmentInvoicePOs.Id IS NULL) -- and [ShipmentInvoicePOMatches-Totals].PONumber in ('02986', '02989')

                UPDATE ShipmentInvoiceDetails
                SET         SalesFactor = ShipmentInvoicePOSalesFactor.QtyFactor
                FROM    ShipmentInvoicePOSalesFactor INNER JOIN
                                    ShipmentInvoiceDetails ON ShipmentInvoicePOSalesFactor.INVDetailsId = ShipmentInvoiceDetails.Id

                INSERT INTO InventoryItemAlias
                                    (InventoryItemId, AliasItemId)
                SELECT distinct ShipmentInvoicePOItemQueryMatches.POInventoryItemId, ShipmentInvoicePOItemQueryMatches.INVInventoryItemId --   , POItemCode
                FROM   (select top (1) with ties  * 
				from [ShipmentInvoicePOItemQueryMatches]/*-byItemCode*/ t 
				order by row_number() over (partition by invdetailsid order by rankno)) as  ShipmentInvoicePOItemQueryMatches LEFT OUTER JOIN
													InventoryItemAliasEx AS InventoryItemAlias_1 ON ShipmentInvoicePOItemQueryMatches.POInventoryItemId = InventoryItemAlias_1.InventoryItemId AND 
													ShipmentInvoicePOItemQueryMatches.INVItemCode = InventoryItemAlias_1.AliasName
								WHERE (InventoryItemAlias_1.AliasId IS NULL) and INVInventoryItemId <> POInventoryItemId --and poitemcode = 'MRL/FAA009F'
END
GO
