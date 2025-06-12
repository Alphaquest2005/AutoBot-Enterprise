const sql = require('mssql');

// Database configuration from your app.config
const config = {
    server: 'MINIJOE\\SQLDEVELOPER2022',
    database: 'WebSource-AutoBot',
    user: 'sa',
    password: 'pa$$word',
    options: {
        encrypt: false, // Set to true if you need encryption
        trustServerCertificate: true,
        enableArithAbort: true
    },
    pool: {
        max: 10,
        min: 0,
        idleTimeoutMillis: 30000
    }
};

async function fixProblematicPatterns() {
    let pool;
    
    try {
        console.log('🔍 **CONNECTING**: Connecting to SQL Server...');
        pool = await sql.connect(config);
        
        // Step 1: Show what we're about to delete
        console.log('\n=== PROBLEMATIC PATTERNS TO DELETE ===');
        const problematicQuery = `
            SELECT 
                r.Id,
                r.RegEx,
                r.Description,
                r.CreatedDate,
                COUNT(l.Id) as LineCount
            FROM RegularExpressions r
            LEFT JOIN Lines l ON l.RegularExpressionsId = r.Id
            WHERE r.RegEx = '(?<TotalDeduction>\\d+\\.\\d{2})'
            GROUP BY r.Id, r.RegEx, r.Description, r.CreatedDate
            ORDER BY r.CreatedDate DESC
        `;
        
        const problematicResult = await pool.request().query(problematicQuery);
        console.log('Found patterns to delete:', problematicResult.recordset);
        
        // Step 2: Count before deletion
        const beforeCountResult = await pool.request().query(`
            SELECT COUNT(*) as Count 
            FROM RegularExpressions 
            WHERE RegEx = '(?<TotalDeduction>\\\\d+\\\\.\\\\d{2})'
        `);
        const beforeCount = beforeCountResult.recordset[0].Count;
        console.log(`\n📊 **BEFORE_COUNT**: ${beforeCount} patterns before deletion`);
        
        if (beforeCount === 0) {
            console.log('✅ **NO_ACTION_NEEDED**: No problematic patterns found to delete');
            return;
        }
        
        // Step 3: Delete the problematic patterns
        console.log('\n🗑️ **DELETING**: Removing problematic AutoOmission patterns...');
        const deleteResult = await pool.request().query(`
            DELETE FROM RegularExpressions 
            WHERE RegEx = '(?<TotalDeduction>\\\\d+\\\\.\\\\d{2})'
               AND (Description LIKE '%AutoOmission%TotalDeduction%' 
                    OR Description LIKE '%AutoOmission_TotalDeduction%'
                    OR Description IS NULL)
        `);
        
        const deletedCount = deleteResult.rowsAffected[0];
        console.log(`🗑️ **DELETED**: Removed ${deletedCount} problematic patterns`);
        
        // Step 4: Verify deletion
        const afterCountResult = await pool.request().query(`
            SELECT COUNT(*) as Count 
            FROM RegularExpressions 
            WHERE RegEx = '(?<TotalDeduction>\\\\d+\\\\.\\\\d{2})'
        `);
        const afterCount = afterCountResult.recordset[0].Count;
        console.log(`📊 **AFTER_COUNT**: ${afterCount} patterns remaining`);
        
        // Step 5: Show remaining TotalDeduction patterns
        console.log('\n=== REMAINING TOTALDEDUCTION PATTERNS ===');
        const remainingQuery = `
            SELECT 
                r.Id,
                r.RegEx,
                r.Description,
                r.CreatedDate,
                LEN(r.RegEx) as PatternLength
            FROM RegularExpressions r
            WHERE r.RegEx LIKE '%TotalDeduction%'
            ORDER BY LEN(r.RegEx), r.CreatedDate
        `;
        
        const remainingResult = await pool.request().query(remainingQuery);
        console.log('Remaining TotalDeduction patterns:', remainingResult.recordset);
        
        // Step 6: Check Amazon template state
        console.log('\n=== AMAZON TEMPLATE VERIFICATION ===');
        const amazonQuery = `
            SELECT 
                i.Name as TemplateName,
                i.Id as TemplateId,
                COUNT(DISTINCT p.Id) as PartCount,
                COUNT(DISTINCT l.Id) as LineCount,
                COUNT(DISTINCT r.Id) as RegexCount
            FROM Invoices i
            LEFT JOIN Parts p ON p.TemplateId = i.Id  
            LEFT JOIN Lines l ON l.PartId = p.Id
            LEFT JOIN RegularExpressions r ON l.RegularExpressionsId = r.Id
            WHERE i.Id = 5
            GROUP BY i.Name, i.Id
        `;
        
        const amazonResult = await pool.request().query(amazonQuery);
        console.log('Amazon template state:', amazonResult.recordset);
        
        // Success summary
        if (deletedCount > 0) {
            console.log(`\n✅ **SUCCESS**: Successfully deleted ${deletedCount} problematic patterns`);
            console.log('🎯 **RESULT**: Grand Total ($166.30) should no longer be incorrectly mapped to TotalDeduction');
            console.log('🔧 **NEXT_STEP**: Run the Amazon invoice test to verify TotalsZero is now balanced');
        } else if (beforeCount === 0) {
            console.log('\n✅ **ALREADY_FIXED**: No problematic patterns were found - may have been fixed already');
        } else {
            console.log('\n⚠️ **WARNING**: Patterns were found but not deleted - check the deletion criteria');
        }
        
    } catch (err) {
        console.error('🚨 **ERROR**: Database operation failed:', err);
    } finally {
        if (pool) {
            await pool.close();
            console.log('\n🔌 **DISCONNECTED**: Database connection closed');
        }
    }
}

// Run the fix
console.log('🚨 **CRITICAL_FIX**: Starting deletion of conflicting AutoOmission TotalDeduction patterns');
console.log('💡 **PURPOSE**: Fix Grand Total ($166.30) being incorrectly mapped to TotalDeduction instead of InvoiceTotal');
console.log('👤 **USER_CONFIRMED**: "yes,,, that is completley wrong"\n');

fixProblematicPatterns();