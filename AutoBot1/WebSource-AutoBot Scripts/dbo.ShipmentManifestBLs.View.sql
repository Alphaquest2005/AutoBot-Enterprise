USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentManifestBLs]    Script Date: 4/8/2025 8:33:17 AM ******/
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
