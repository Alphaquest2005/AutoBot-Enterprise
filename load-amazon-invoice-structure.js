// Load complete Amazon invoice structure into memory to understand field mapping complexity
import sql from 'mssql';

const dbConfig = {
  user: 'sa',
  password: 'pa$$word',
  server: 'MINIJOE\\SQLDEVELOPER2022',
  database: 'WebSource-AutoBot',
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

async function loadAmazonInvoiceStructure() {
  console.log('🔍 Loading Complete Amazon Invoice Structure...\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. Find Amazon invoice
    console.log('=== AMAZON INVOICE DETAILS ===');
    const amazonInvoice = await sql.query(`
      SELECT Id, Name, FileTypeId, ApplicationSettingsId, IsActive
      FROM [OCR-Invoices]
      WHERE Name = 'Amazon' AND IsActive = 1
    `);

    if (amazonInvoice.recordset.length === 0) {
      console.log('❌ Amazon invoice not found!');
      return;
    }

    const amazonId = amazonInvoice.recordset[0].Id;
    console.log('Amazon Invoice:', amazonInvoice.recordset[0]);
    console.log(`Amazon Invoice ID: ${amazonId}\n`);

    // 2. Load all parts for Amazon invoice
    console.log('=== AMAZON PARTS STRUCTURE ===');
    const amazonParts = await sql.query(`
      SELECT p.Id, p.TemplateId, p.PartTypeId, pt.Name as PartTypeName
      FROM [OCR-Parts] p
      INNER JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      WHERE p.TemplateId = ${amazonId}
      ORDER BY p.Id
    `);
    console.log(`Found ${amazonParts.recordset.length} parts for Amazon invoice:`);
    console.table(amazonParts.recordset);

    // 3. Use OCR-LinesView to get all lines for Amazon
    console.log('\n=== AMAZON LINES STRUCTURE (Using OCR-LinesView) ===');
    const amazonLines = await sql.query(`
      SELECT Invoice, Part, Line, MultiLine, RegEx, Id as LineId, RegExId, ParentId,
             PartId, IsColumn, DistinctValues, IsActive, Comments
      FROM [OCR-LinesView]
      WHERE Invoice = 'Amazon'
      ORDER BY PartId, Id
    `);

    console.log(`Found ${amazonLines.recordset.length} lines for Amazon invoice:`);
    console.table(amazonLines.recordset);

    // 4. Use OCR-PartLineFields view to get all fields for Amazon
    console.log('\n=== AMAZON FIELDS STRUCTURE (Using OCR-PartLineFields) ===');
    const amazonFields = await sql.query(`
      SELECT Invoice, Part, Line, RegEx, [Key], Field, EntityType, IsRequired,
             DataType, Value, AppendValues, FieldId, ParentId, LineId
      FROM [OCR-PartLineFields]
      WHERE Invoice = 'Amazon'
      ORDER BY Part, Line, Field
    `);

    console.log(`Found ${amazonFields.recordset.length} fields for Amazon invoice:`);
    console.table(amazonFields.recordset);

    // 5. Show field format regex for Amazon
    console.log('\n=== AMAZON FIELD FORMAT REGEX ===');
    const formatRegex = await sql.query(`
      SELECT ffr.Id, ffr.FieldId, ffr.RegExId, ffr.ReplacementRegExId,
             f.Field, f.[Key], f.EntityType,
             r1.RegEx as FieldRegex,
             r2.RegEx as ReplacementRegex
      FROM [OCR-FieldFormatRegEx] ffr
      INNER JOIN [OCR-Fields] f ON ffr.FieldId = f.Id
      INNER JOIN [OCR-Lines] l ON f.LineId = l.Id
      INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
      LEFT JOIN [OCR-RegularExpressions] r1 ON ffr.RegExId = r1.Id
      LEFT JOIN [OCR-RegularExpressions] r2 ON ffr.ReplacementRegExId = r2.Id
      WHERE p.TemplateId = ${amazonId}
      ORDER BY f.Field
    `);

    console.log(`Found ${formatRegex.recordset.length} field format regex patterns:`);
    if (formatRegex.recordset.length > 0) {
      console.table(formatRegex.recordset);
    }

    // 6. Show field conflicts within Amazon invoice
    console.log('\n=== FIELD NAME CONFLICTS WITHIN AMAZON ===');
    const amazonFieldConflicts = await sql.query(`
      SELECT f.Field, f.[Key], COUNT(*) as OccurrenceCount,
             COUNT(DISTINCT f.EntityType) as DifferentEntityTypes,
             COUNT(DISTINCT f.DataType) as DifferentDataTypes
      FROM [OCR-Fields] f
      INNER JOIN [OCR-Lines] l ON f.LineId = l.Id
      INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
      WHERE p.TemplateId = ${amazonId}
      GROUP BY f.Field, f.[Key]
      HAVING COUNT(*) > 1
      ORDER BY COUNT(*) DESC
    `);

    console.log(`Found ${amazonFieldConflicts.recordset.length} field conflicts within Amazon invoice:`);
    if (amazonFieldConflicts.recordset.length > 0) {
      console.table(amazonFieldConflicts.recordset);
    }

    // 7. Summary statistics
    console.log('\n=== AMAZON INVOICE SUMMARY ===');
    const summary = await sql.query(`
      SELECT
        COUNT(DISTINCT p.Id) as TotalParts,
        COUNT(DISTINCT l.Id) as TotalLines,
        COUNT(DISTINCT f.Id) as TotalFields,
        COUNT(DISTINCT f.Field) as UniqueFieldNames,
        COUNT(DISTINCT r.Id) as TotalRegexPatterns
      FROM [OCR-Parts] p
      LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      LEFT JOIN [OCR-RegularExpressions] r ON l.RegExId = r.Id
      WHERE p.TemplateId = ${amazonId}
    `);

    console.log('Amazon Invoice Structure Summary:');
    console.table(summary.recordset);

  } catch (error) {
    console.error('❌ Database error:', error.message);
    console.error('Stack trace:', error.stack);
  } finally {
    await sql.close();
  }
}

loadAmazonInvoiceStructure();
