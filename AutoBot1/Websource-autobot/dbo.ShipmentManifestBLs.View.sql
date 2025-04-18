USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentManifestBLs]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[ShipmentManifestBLs]
AS
SELECT cast(row_number() OVER ( ORDER BY ShipmentBL.Id) AS int) AS Id, ShipmentManifest.Id AS ManifestId, ShipmentBL.Id AS BLId, ShipmentBL.BLNumber, ShipmentManifest.WayBill
FROM    ShipmentManifest  inner JOIN
                 ShipmentBL ON ShipmentManifest.ApplicationSettingsId = ShipmentBL.ApplicationSettingsId AND (ShipmentManifest.WayBill = ShipmentBL.BLNumber) -- or ShipmentManifest.Voyage like '%' + shipmentbl.voyage + '%') 2 shipments can come on same boat
GO
