-- Add new Action for importing shipment info from Info.txt
SET IDENTITY_INSERT [dbo].[Actions] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[Actions] WHERE Id = 122)
BEGIN
    INSERT INTO [dbo].[Actions] ([Id], [Name], [TestMode], [IsDataSpecific])
    VALUES (122, N'ImportShipmentInfoFromTxt', 0, 1);
    PRINT 'Action ImportShipmentInfoFromTxt added with ID 122.';
END
ELSE
BEGIN
    -- Optionally update existing action if needed, or just report
    -- UPDATE [dbo].[Actions] SET Name = N'ImportShipmentInfoFromTxt', TestMode = 0, IsDataSpecific = 1 WHERE Id = 122;
    PRINT 'Action with ID 122 already exists.';
END

SET IDENTITY_INSERT [dbo].[Actions] OFF;
GO