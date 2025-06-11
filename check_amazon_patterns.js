const fs = require('fs');

// Read the template context file we captured earlier
const templateContext = JSON.parse(fs.readFileSync('template_context_amazon.json', 'utf8'));

console.log("🔍 **AMAZON_TEMPLATE_ANALYSIS**: Current regex patterns for Amazon template");
console.log("");

templateContext.lines.forEach(line => {
    console.log(`📋 **LINE_${line.LineId}**: ${line.LineName}`);
    console.log(`   📝 **REGEX**: ${line.RegexPattern}`);
    
    if (line.Fields && line.Fields.length > 0) {
        line.Fields.forEach(field => {
            console.log(`   🎯 **FIELD**: ${field.Key} → ${field.Field} (${field.EntityType})`);
        });
    } else {
        console.log(`   ❌ **NO_FIELDS**: No field mappings found`);
    }
    console.log("");
});

// Look specifically for Free Shipping patterns
const freeShippingLines = templateContext.lines.filter(line => 
    line.RegexPattern && line.RegexPattern.toLowerCase().includes('free'));
    
console.log("🔍 **FREE_SHIPPING_PATTERNS**: Lines that might capture Free Shipping");
if (freeShippingLines.length > 0) {
    freeShippingLines.forEach(line => {
        console.log(`✅ **FOUND**: Line ${line.LineId} - ${line.LineName}`);
        console.log(`   📝 **PATTERN**: ${line.RegexPattern}`);
    });
} else {
    console.log("❌ **MISSING**: No Free Shipping patterns found in Amazon template");
}
