USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentRiderManifests]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[ShipmentRiderManifests]
AS
SELECT distinct  cast(row_number() OVER ( ORDER BY ShipmentRider.Id) AS int) AS id,
			dbo.ShipmentRider.Id as RiderId, ShipmentRiderDetails.id as RiderDetailId, ShipmentManifest.Id as ManifestId, dbo.ShipmentRider.ETA, dbo.ShipmentManifest.WayBill As BLNumber, dbo.ShipmentRiderDetails.Pieces as Quantity,  dbo.ShipmentRiderDetails.WarehouseCode as Marks, ShipmentManifest.Marks as ManifestMark			
FROM    dbo.ShipmentRider INNER JOIN
                 dbo.ShipmentRiderDetails ON dbo.ShipmentRider.Id = dbo.ShipmentRiderDetails.RiderId INNER JOIN
                 dbo.ShipmentManifest ON  ShipmentRider.ApplicationSettingsId = ShipmentManifest.ApplicationSettingsId
where ShipmentRiderDetails.WarehouseCode in (select value from STRING_SPLIT(replace(replace(ShipmentManifest.Marks,' ',','),'-',','),','))
GO
