--DECLARE @SQL AS NVARCHAR(4000) DECLARE @table_name AS NVARCHAR(255) DECLARE @column_name AS NVARCHAR(255) DECLARE @isnullable AS BIT 
--DECLARE CUR CURSOR FAST_FORWARD FOR 
--SELECT c.TABLE_NAME, c.COLUMN_NAME, CASE WHEN c.is_nullable = 'YES' THEN 1 ELSE 0 END AS is_nullable
--FROM    INFORMATION_SCHEMA.COLUMNS AS c INNER JOIN
--                     (SELECT TABLE_NAME
--                      FROM     INFORMATION_SCHEMA.TABLES
--                      WHERE  (TABLE_TYPE = 'BASE TABLE')) AS tables_1 ON c.TABLE_NAME = tables_1.TABLE_NAME
--WHERE (c.DATA_TYPE = 'varchar') AND (c.TABLE_CATALOG = 'Budget-ENTERPRISEDB') AND (c.TABLE_SCHEMA = 'dbo')
--												 --AND c.table_name = 'your_table' 
--		OPEN CUR FETCH NEXT FROM CUR 
--		INTO @table_name, @column_name, @isnullable 
--			WHILE @@FETCH_STATUS = 0 
--				BEGIN 
--					SELECT @SQL = 'ALTER TABLE [' + @table_name + '] ALTER COLUMN [' + @column_name + '] nvarchar' + (CASE WHEN @isnullable = 1 THEN '' ELSE ' NOT' END) + ' NULL;' 
					
--					EXEC sp_executesql @SQL 
--					FETCH NEXT FROM CUR 
--					INTO @table_name, @column_name, @isnullable
--			   END 
--		 CLOSE CUR;
--		 DEALLOCATE CUR;


SELECT 'ALTER TABLE ' + isnull(schema_name(syo.id), 'dbo') + '.[' +  syo.name +'] ' 
    + ' ALTER COLUMN [' + syc.name + '] NVARCHAR(' + case syc.length when -1 then '255' 
        ELSE convert(nvarchar(10),syc.length) end + ') '+ 
        case  syc.isnullable when 1 then ' NULL' ELSE ' NOT NULL' END +';' 
   FROM sysobjects syo 
   JOIN syscolumns syc ON 
     syc.id = syo.id 
   JOIN systypes syt ON 
     syt.xtype = syc.xtype 
   WHERE 
     syt.name = 'text' 
    and syo.xtype='U'


SELECT 'ALTER TABLE ' + isnull(schema_name(syo.id), 'dbo') + '.[' +  syo.name +'] ' 
    + ' ALTER COLUMN [' + syc.name + '] NVARCHAR(255)  NULL;' 
   FROM sysobjects syo 
   JOIN syscolumns syc ON 
     syc.id = syo.id 
   JOIN systypes syt ON 
     syt.xtype = syc.xtype 
   WHERE 
     syt.name = 'text' 
    and syo.xtype='U'
	and syc.name ='null'

SELECT 'EXEC sp_rename ''' + isnull(schema_name(syo.id), 'dbo') + '.[' +  syo.name +'].' + '[' + syc.name + ']'', ''Value'', ''COLUMN'';' 
   FROM sysobjects syo 
   JOIN syscolumns syc ON 
     syc.id = syo.id 
   JOIN systypes syt ON 
     syt.xtype = syc.xtype 
   WHERE 
   --  syt.name = 'text' 
   -- and 
	syo.xtype='U'
	and syc.name ='null'


	SELECT 'ALTER TABLE ' + isnull(schema_name(syo.id), 'dbo') + '.[' +  syo.name +'] ' 
    + ' ALTER COLUMN [' + syc.name + '] NVARCHAR(' + case syc.length when -1 then '255' 
        ELSE convert(nvarchar(10),syc.length) end + ') '+ 
        case  syc.isnullable when 1 then ' NULL' ELSE ' NOT NULL' END +';' 
   FROM sysobjects syo 
   JOIN syscolumns syc ON 
     syc.id = syo.id 
   JOIN systypes syt ON 
     syt.xtype = syc.xtype 
   WHERE 
     syt.name = 'text' 
    and syo.xtype='U'

SELECT 'EXEC sp_rename ''' + isnull(schema_name(syo.id), 'dbo') + '.[' +  syo.name +']'', ''[x'+name+']'';' 
FROM    sys.sysobjects AS syo
WHERE (xtype = 'U') and syo.Name <> 'sysdiagrams'



















ALTER TABLE dbo.[xcuda_Gs_internal_freight]  ALTER COLUMN [Currency_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Delivery_terms]  ALTER COLUMN [Code] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Delivery_terms]  ALTER COLUMN [Place] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Delivery_terms]  ALTER COLUMN [Situation] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[AsycudaDocumentEntryData]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[xcuda_Departure_arrival_information]  ALTER COLUMN [Identity] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Departure_arrival_information]  ALTER COLUMN [Nationality] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Gs_insurance]  ALTER COLUMN [Currency_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Export_release]  ALTER COLUMN [Date_of_exit] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Export_release]  ALTER COLUMN [Time_of_exit] NVARCHAR(255)  NULL;





ALTER TABLE dbo.[xcuda_Previous_doc]  ALTER COLUMN [Summary_declaration] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Gs_deduction]  ALTER COLUMN [Currency_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_item_deduction]  ALTER COLUMN [Currency_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[EntryData_Sales]  ALTER COLUMN [INVNumber] NVARCHAR(255)  NOT NULL;
ALTER TABLE dbo.[EntryData_Sales]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[EntryData_Sales]  ALTER COLUMN [CustomerName] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_item_external_freight]  ALTER COLUMN [Currency_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[EntryData_Adjustments]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[xcuda_item_insurance]  ALTER COLUMN [Currency_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[EntryData_OpeningStock]  ALTER COLUMN [OPSNumber] NVARCHAR(255)  NOT NULL;
ALTER TABLE dbo.[EntryData_OpeningStock]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Packages_number] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Previous_Packages_number] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Hs_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Commodity_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Previous_item_number] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Goods_origin] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Prev_reg_ser] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Prev_reg_nbr] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Prev_reg_dat] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Prev_reg_cuo] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Current_item_number] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_PreviousItem]  ALTER COLUMN [Prev_decl_HS_spec] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_item_internal_freight]  ALTER COLUMN [Currency_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[EntryData_PurchaseOrders]  ALTER COLUMN [PONumber] NVARCHAR(255)  NOT NULL;
ALTER TABLE dbo.[EntryData_PurchaseOrders]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[xcuda_Taxation_line]  ALTER COLUMN [Duty_tax_Base] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Taxation_line]  ALTER COLUMN [Duty_tax_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Taxation_line]  ALTER COLUMN [Duty_tax_MP] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Exporter]  ALTER COLUMN [Exporter_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Exporter]  ALTER COLUMN [Exporter_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_item_other_cost]  ALTER COLUMN [Currency_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[TariffSupUnitLkps]  ALTER COLUMN [SuppUnitCode2] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[TariffSupUnitLkps]  ALTER COLUMN [SuppUnitName2] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Means_of_transport]  ALTER COLUMN [Inland_mode_of_transport] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[TariffCategory]  ALTER COLUMN [TariffCategoryCode] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[TariffCategory]  ALTER COLUMN [Description] NVARCHAR(1998)  NULL;
ALTER TABLE dbo.[TariffCategory]  ALTER COLUMN [ParentTariffCategoryCode] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Office_segment]  ALTER COLUMN [Customs_clearance_office_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Office_segment]  ALTER COLUMN [Customs_Clearance_office_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[TariffCategoryCodeSuppUnit]  ALTER COLUMN [TariffCategoryCode] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[xcuda_General_information]  ALTER COLUMN [Value_details] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_General_information]  ALTER COLUMN [CAP] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_General_information]  ALTER COLUMN [Additional_information] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_General_information]  ALTER COLUMN [Comments_free_text] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Property]  ALTER COLUMN [Sad_flow] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Property]  ALTER COLUMN [Date_of_declaration] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Property]  ALTER COLUMN [Selected_page] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Property]  ALTER COLUMN [Place_of_declaration] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_receipt]  ALTER COLUMN [Number] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_receipt]  ALTER COLUMN [Date] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Registration]  ALTER COLUMN [Number] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Registration]  ALTER COLUMN [Date] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Export]  ALTER COLUMN [Export_country_code] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Export]  ALTER COLUMN [Export_country_name] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Export]  ALTER COLUMN [Export_country_region] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Seals]  ALTER COLUMN [Number] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Signature]  ALTER COLUMN [Date] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[Sales -> to OPS - Overs]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[Sales -> to OPS - Overs]  ALTER COLUMN [ItemNumber] NVARCHAR(510)  NULL;
ALTER TABLE dbo.[Sales -> to OPS - Overs]  ALTER COLUMN [Description] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_ASYCUDA_ExtendedProperties]  ALTER COLUMN [CNumber] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_ASYCUDA_ExtendedProperties]  ALTER COLUMN [ReferenceNumber] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_ASYCUDA_ExtendedProperties]  ALTER COLUMN [Description] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_ASYCUDA_ExtendedProperties]  ALTER COLUMN [BLNumber] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[Sales -> to OPS - Shorts]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[Sales -> to OPS - Shorts]  ALTER COLUMN [ItemNumber] NVARCHAR(510)  NULL;
ALTER TABLE dbo.[Sales -> to OPS - Shorts]  ALTER COLUMN [Description] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Destination]  ALTER COLUMN [Destination_country_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Destination]  ALTER COLUMN [Destination_country_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_date] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_itmlink] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_country] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_city] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_street] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_telephone] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_fax] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_zip_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_invoice_nbr] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_invoice_amt] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Suppliers_documents]  ALTER COLUMN [Suppliers_document_type_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[InventoryItems]  ALTER COLUMN [ItemNumber] NVARCHAR(510)  NOT NULL;
ALTER TABLE dbo.[InventoryItems]  ALTER COLUMN [Description] NVARCHAR(255)  NOT NULL;
ALTER TABLE dbo.[InventoryItems]  ALTER COLUMN [Category] NVARCHAR(120)  NULL;
ALTER TABLE dbo.[InventoryItems]  ALTER COLUMN [TariffCode] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[sales Errors -> to OPS - Overs]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[sales Errors -> to OPS - Overs]  ALTER COLUMN [ItemNumber] NVARCHAR(510)  NULL;
ALTER TABLE dbo.[sales Errors -> to OPS - Overs]  ALTER COLUMN [Description] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Transport]  ALTER COLUMN [Location_of_goods] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Nbers]  ALTER COLUMN [Number_of_loading_lists] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Nbers]  ALTER COLUMN [Total_number_of_items] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Type]  ALTER COLUMN [Type_of_declaration] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Type]  ALTER COLUMN [Declaration_gen_procedure_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [TariffCode] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [Description] NVARCHAR(1998)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [RateofDuty] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [EnvironmentalLevy] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [CustomsServiceCharge] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [ExciseTax] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [VatRate] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [PetrolTax] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [Units] NVARCHAR(1998)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [SiteRev3] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[TariffCodes]  ALTER COLUMN [TariffCategoryCode] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Packages]  ALTER COLUMN [Kind_of_packages_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Packages]  ALTER COLUMN [Kind_of_packages_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Packages]  ALTER COLUMN [Marks1_of_packages] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Packages]  ALTER COLUMN [Marks2_of_packages] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[WeightCalculationMethods]  ALTER COLUMN [Name] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[History-Batches]  ALTER COLUMN [BatchName] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[History-Batches]  ALTER COLUMN [Comments] NVARCHAR(255)  NOT NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [Description] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [SoftwareName] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowCounterPoint] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowTariffCodes] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowWareHouse] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowXBond] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowAsycudaManager] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowQuickBooks] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowExportToExcel] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowAutoWeightCalculation] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowEntryPerIM7] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowSalesToPI] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowEffectiveAssessmentDate] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowAutoFreightCalculation] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowSubItems] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowEntryDoNotAllocate] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowPreviousItems] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowOversShort] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowContainers] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowNonXEntries] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowValidateTariffCodes] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [AllowCleanBond] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [OrderEntriesBy] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [WeightCalculationMethod] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[ApplicationSettings]  ALTER COLUMN [DeclarantCode] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Country]  ALTER COLUMN [Country_first_destination] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Country]  ALTER COLUMN [Country_of_origin_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Country]  ALTER COLUMN [Trading_country] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Gs_Invoice]  ALTER COLUMN [Currency_code] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Gs_Invoice]  ALTER COLUMN [Currency_name] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[EntryDataDetails]  ALTER COLUMN [EntryDataId] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[EntryDataDetails]  ALTER COLUMN [ItemNumber] NVARCHAR(510)  NOT NULL;
ALTER TABLE dbo.[EntryDataDetails]  ALTER COLUMN [Units] NVARCHAR(30)  NULL;
ALTER TABLE dbo.[EntryDataDetails]  ALTER COLUMN [ItemDescription] NVARCHAR(255)  NOT NULL;
ALTER TABLE dbo.[EntryDataDetails]  ALTER COLUMN [Status] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[EntryDataDetails]  ALTER COLUMN [PreviousInvoiceNumber] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[EntryDataDetails]  ALTER COLUMN [CNumber] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[EntryDataDetails]  ALTER COLUMN [Comment] NVARCHAR(510)  NULL;
ALTER TABLE dbo.[xcuda_Tarification]  ALTER COLUMN [Extended_customs_procedure] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Tarification]  ALTER COLUMN [National_customs_procedure] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Tarification]  ALTER COLUMN [Value_item] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Tarification]  ALTER COLUMN [Attached_doc_item] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Traders_Financial]  ALTER COLUMN [Financial_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Traders_Financial]  ALTER COLUMN [Financial_name] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Taxation]  ALTER COLUMN [Counter_of_normal_mode_of_payment] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Taxation]  ALTER COLUMN [Displayed_item_taxes_amount] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Taxation]  ALTER COLUMN [Item_taxes_mode_of_payment] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Declarant]  ALTER COLUMN [Declarant_code] NVARCHAR(20)  NULL;
ALTER TABLE dbo.[xcuda_Declarant]  ALTER COLUMN [Declarant_name] NVARCHAR(510)  NULL;
ALTER TABLE dbo.[xcuda_Declarant]  ALTER COLUMN [Declarant_representative] NVARCHAR(510)  NULL;
ALTER TABLE dbo.[xcuda_Declarant]  ALTER COLUMN [Number] NVARCHAR(60)  NULL;
ALTER TABLE dbo.[History-Batch-Issues]  ALTER COLUMN [ItemNumber] NVARCHAR(100)  NOT NULL;
ALTER TABLE dbo.[History-Batch-Issues]  ALTER COLUMN [Issue] NVARCHAR(255)  NOT NULL;
ALTER TABLE dbo.[AsycudaDocumentSet]  ALTER COLUMN [Declarant_Reference_Number] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[AsycudaDocumentSet]  ALTER COLUMN [Country_of_origin_code] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[AsycudaDocumentSet]  ALTER COLUMN [Currency_Code] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[AsycudaDocumentSet]  ALTER COLUMN [Description] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[AsycudaDocumentSet]  ALTER COLUMN [Manifest_Number] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[AsycudaDocumentSet]  ALTER COLUMN [BLNumber] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[AsycudaDocumentSet]  ALTER COLUMN [ApportionMethod] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Supplementary_unit]  ALTER COLUMN [Suppplementary_unit_code] NVARCHAR(8)  NULL;
ALTER TABLE dbo.[xcuda_Supplementary_unit]  ALTER COLUMN [Suppplementary_unit_name] NVARCHAR(20)  NULL;
ALTER TABLE dbo.[Customs_Procedure]  ALTER COLUMN [Extended_customs_procedure] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[Customs_Procedure]  ALTER COLUMN [National_customs_procedure] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Identification]  ALTER COLUMN [Manifest_reference_number] NVARCHAR(100)  NULL;
ALTER TABLE dbo.[xcuda_Valuation]  ALTER COLUMN [Calculation_working_mode] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[xcuda_Item_Invoice]  ALTER COLUMN [Currency_code] NVARCHAR(255)  NULL;
ALTER TABLE dbo.[AsycudaSalesAllocations]  ALTER COLUMN [Status] NVARCHAR(255)  NULL;
