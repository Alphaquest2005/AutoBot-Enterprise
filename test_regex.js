// Test the Free Shipping regex patterns

const sampleText = `
Item(s) Subtotal: $161.95
Shipping & Handling: $6.99
Free Shipping: -$0.46
Free Shipping: -$6.53
Estimated tax to be collected: $11.34
Gift Card Amount: -$6.99
Grand Total: $166.30
`;

console.log("🔍 **REGEX_TESTING**: Testing Free Shipping patterns");

// Current pattern from template (broken)
const currentPattern = /Free Shipping:[\s\-\$]+(?<Currency>\w{3})[\s\-\$]+(?<FreeShipping>[\d,.]+)/g;

// New proposed pattern (simplified)
const newPattern = /Free Shipping:\s*-?\$?(?<FreeShipping>[\d,.]+)/g;

console.log("\n📋 **CURRENT_PATTERN**: " + currentPattern.source);
let matches = [...sampleText.matchAll(currentPattern)];
console.log("   📊 **MATCHES**: " + matches.length);
matches.forEach((match, i) => {
    console.log(`   ✅ **MATCH_${i+1}**: ${match[0]} → FreeShipping=${match.groups?.FreeShipping}`);
});

console.log("\n📋 **NEW_PATTERN**: " + newPattern.source);
matches = [...sampleText.matchAll(newPattern)];
console.log("   📊 **MATCHES**: " + matches.length);
matches.forEach((match, i) => {
    console.log(`   ✅ **MATCH_${i+1}**: ${match[0]} → FreeShipping=${match.groups?.FreeShipping}`);
});

// Calculate total
if (matches.length > 0) {
    const total = matches.reduce((sum, match) => {
        return sum + parseFloat(match.groups?.FreeShipping || 0);
    }, 0);
    console.log(`\n🎯 **TOTAL_FREE_SHIPPING**: ${total} (should be 6.99)`);
}
