USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentFreightManifests]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ShipmentFreightManifests]
AS
SELECT cast(row_number() OVER ( ORDER BY ShipmentFreightDetails.Id) AS int) AS Id, ShipmentFreightDetails.Id AS FreightDetailId, ShipmentFreight.Id AS FreightId, ShipmentManifest.Id AS ManifestId, ShipmentManifest.RegistrationNumber, ShipmentFreight.InvoiceNumber, ShipmentFreightDetails.WarehouseCode
FROM    ShipmentFreightDetails INNER JOIN
                 ShipmentFreight ON ShipmentFreightDetails.FreightId = ShipmentFreight.Id INNER JOIN
                 ShipmentManifest ON ShipmentFreight.ApplicationSettingsId = ShipmentManifest.ApplicationSettingsId AND  trim(ShipmentManifest.Marks) like '%' + trim(ShipmentFreightDetails.WarehouseCode) + '%'
where ShipmentFreightDetails.WarehouseCode <> ''

GO
