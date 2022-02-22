SELECT [TODO-SubmitUnclassifiedItems].InvoiceNo, [TODO-SubmitUnclassifiedItems].LineNumber, [TODO-SubmitUnclassifiedItems].ItemNumber, [TODO-SubmitUnclassifiedItems].ItemDescription, 
                 [TODO-SubmitUnclassifiedItems].EmailId, [TODO-SubmitUnclassifiedItems].Type, [TODO-SubmitUnclassifiedItems].AsycudaDocumentSetId, [TODO-SubmitUnclassifiedItems].Declarant_Reference_Number, 
                 [TODO-SubmitUnclassifiedItems].ApplicationSettingsId, TariffCodes.TariffCode, TariffKeyWords.Keyword, TariffKeyWords.IsException
FROM    TariffCodes INNER JOIN
                 TariffKeyWords ON TariffCodes.TariffCode = TariffKeyWords.TariffCode RIGHT OUTER JOIN
                 [TODO-SubmitUnclassifiedItems] ON isnull(TariffKeyWords.ApplicationSettingsId, [TODO-SubmitUnclassifiedItems].ApplicationSettingsId) = [TODO-SubmitUnclassifiedItems].ApplicationSettingsId AND CHARINDEX(TariffKeyWords.Keyword, 
                 [TODO-SubmitUnclassifiedItems].ItemDescription) > 0


SELECT [TODO-SubmitUnclassifiedItems].InvoiceNo, [TODO-SubmitUnclassifiedItems].LineNumber, [TODO-SubmitUnclassifiedItems].ItemNumber, [TODO-SubmitUnclassifiedItems].ItemDescription, 
                 [TODO-SubmitUnclassifiedItems].EmailId, [TODO-SubmitUnclassifiedItems].Type, [TODO-SubmitUnclassifiedItems].AsycudaDocumentSetId, [TODO-SubmitUnclassifiedItems].Declarant_Reference_Number, 
                 [TODO-SubmitUnclassifiedItems].ApplicationSettingsId, TariffCodes.TariffCode, KeyWords.Keyword, KeyWords.IsException
FROM    TariffCodes INNER JOIN
                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
                      FROM     TariffKeyWords
                      WHERE  (IsException = 0)) AS KeyWords ON TariffCodes.TariffCode = KeyWords.TariffCode LEFT OUTER JOIN
                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
                      FROM     TariffKeyWords AS TariffKeyWords_1
                      WHERE  (IsException = 1)) AS Exceptions ON TariffCodes.TariffCode = Exceptions.TariffCode RIGHT OUTER JOIN
                 [TODO-SubmitUnclassifiedItems] ON (Exceptions.Keyword IS NULL OR
                 ISNULL(Exceptions.ApplicationSettingsId, [TODO-SubmitUnclassifiedItems].ApplicationSettingsId) = [TODO-SubmitUnclassifiedItems].ApplicationSettingsId AND CHARINDEX(Exceptions.Keyword, 
                 [TODO-SubmitUnclassifiedItems].ItemDescription) <= 0) AND ISNULL(KeyWords.ApplicationSettingsId, [TODO-SubmitUnclassifiedItems].ApplicationSettingsId) 
                 = [TODO-SubmitUnclassifiedItems].ApplicationSettingsId AND CHARINDEX(KeyWords.Keyword, [TODO-SubmitUnclassifiedItems].ItemDescription) > 0





SELECT InventoryItems.ItemNumber, InventoryItems.Description, InventoryItems.ApplicationSettingsId, TariffCodes.TariffCode, KeyWords.Keyword, KeyWords.IsException
FROM    TariffCodes INNER JOIN
                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
                      FROM     TariffKeyWords
                      WHERE  (IsException = 0)) AS KeyWords ON TariffCodes.TariffCode = KeyWords.TariffCode LEFT OUTER JOIN
                     (SELECT Id, TariffCode, Keyword, IsException, ApplicationSettingsId
                      FROM     TariffKeyWords AS TariffKeyWords_1
                      WHERE  (IsException = 1)) AS Exceptions ON TariffCodes.TariffCode = Exceptions.TariffCode RIGHT OUTER JOIN
                 InventoryItems ON ((ISNULL(Exceptions.ApplicationSettingsId, InventoryItems.ApplicationSettingsId) = InventoryItems.ApplicationSettingsId AND CHARINDEX(Exceptions.Keyword, InventoryItems.Description) <= 0) or exceptions.keyword is null) AND 
                 ISNULL(KeyWords.ApplicationSettingsId, InventoryItems.ApplicationSettingsId) = InventoryItems.ApplicationSettingsId AND CHARINDEX(KeyWords.Keyword, InventoryItems.Description) > 0
WHERE (InventoryItems.TariffCode IS NULL) AND (TariffCodes.TariffCode IS NOT NULL)