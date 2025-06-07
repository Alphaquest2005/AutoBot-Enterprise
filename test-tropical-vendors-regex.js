// Test Tropical Vendors Regex Pattern Against Actual Text
// Analyze why the current pattern doesn't match and create a corrected one

import fs from 'fs';

// Current regex pattern from database
const currentPattern = /(?<ItemCode>\d{5,6}-[\w\s]{3,6}-[\w\s]{2,6})\s(?<Description>.+?)\s(?<Quanttity>\d+)\s(?<ItemPrice>\d+\.\d{4})\s(?<ExtendedPrice>\d+\.\d{2})/g;

// Sample product lines from the text file
const sampleLines = [
  "11033-2Y2-M7W9 Crocband Flip 1 15.0000 15.00",
  "11033-2Y2-M8W10 Crocband Flip 2 15.0000 30.00", 
  "11033-2Y2-M9W 11 Crocband Flip 3 15.0000 45.00",
  "11033-37P-M10W12 Crocband Flip Agr/Whi 3 15.0000 45.00",
  "206340-001-M12 CLASSIC ALL TERRAIN CLOG BLK 2 30.0000 60.00",
  "206340-OWV-M10W12 All Terrain Clog Chk 1 27.5000 27.50",
  "206453-060-W6 BROOKLYN LOW WEDGE BLK/BLK 2 27.5000 55.00"
];

console.log('🔍 TROPICAL VENDORS REGEX PATTERN TESTING');
console.log('==========================================\n');

console.log('📊 CURRENT PATTERN FROM DATABASE:');
console.log(currentPattern.source);
console.log();

console.log('🧪 TESTING CURRENT PATTERN AGAINST SAMPLE LINES:');
console.log('─'.repeat(60));

let matchCount = 0;
sampleLines.forEach((line, index) => {
  currentPattern.lastIndex = 0; // Reset regex
  const match = currentPattern.exec(line);
  
  console.log(`Line ${index + 1}: ${line}`);
  if (match) {
    console.log('✅ MATCH FOUND:');
    console.log(`   ItemCode: ${match.groups.ItemCode}`);
    console.log(`   Description: ${match.groups.Description}`);
    console.log(`   Quantity: ${match.groups.Quanttity}`);
    console.log(`   ItemPrice: ${match.groups.ItemPrice}`);
    console.log(`   ExtendedPrice: ${match.groups.ExtendedPrice}`);
    matchCount++;
  } else {
    console.log('❌ NO MATCH');
  }
  console.log();
});

console.log(`📊 RESULTS: ${matchCount}/${sampleLines.length} lines matched\n`);

// Analyze the pattern issues
console.log('🔍 PATTERN ANALYSIS:');
console.log('─'.repeat(40));

console.log('Current pattern expects:');
console.log('(?<ItemCode>\\d{5,6}-[\\w\\s]{3,6}-[\\w\\s]{2,6})\\s(?<Description>.+?)\\s(?<Quanttity>\\d+)\\s(?<ItemPrice>\\d+\\.\\d{4})\\s(?<ExtendedPrice>\\d+\\.\\d{2})');
console.log();

console.log('Actual format analysis:');
sampleLines.forEach((line, index) => {
  const parts = line.split(/\s+/);
  console.log(`Line ${index + 1} parts:`, parts);
});

console.log('\n🔧 PROPOSED CORRECTED PATTERN:');
// Create a more flexible pattern that handles the actual format
const correctedPattern = /(?<ItemCode>\d{5,6}-[\w\d]+-[\w\d]+(?:W\d+)?)\s+(?<Description>.+?)\s+(?<Quantity>\d+)\s+(?<ItemPrice>\d+\.\d{4})\s+(?<ExtendedPrice>\d+\.\d{2})/g;

console.log('New pattern:');
console.log(correctedPattern.source);
console.log();

console.log('🧪 TESTING CORRECTED PATTERN:');
console.log('─'.repeat(60));

let correctedMatchCount = 0;
sampleLines.forEach((line, index) => {
  correctedPattern.lastIndex = 0; // Reset regex
  const match = correctedPattern.exec(line);
  
  console.log(`Line ${index + 1}: ${line}`);
  if (match) {
    console.log('✅ MATCH FOUND:');
    console.log(`   ItemCode: ${match.groups.ItemCode}`);
    console.log(`   Description: ${match.groups.Description}`);
    console.log(`   Quantity: ${match.groups.Quantity}`);
    console.log(`   ItemPrice: ${match.groups.ItemPrice}`);
    console.log(`   ExtendedPrice: ${match.groups.ExtendedPrice}`);
    correctedMatchCount++;
  } else {
    console.log('❌ NO MATCH');
  }
  console.log();
});

console.log(`📊 CORRECTED RESULTS: ${correctedMatchCount}/${sampleLines.length} lines matched\n`);

// Test against actual file content
console.log('🔍 TESTING AGAINST ACTUAL FILE CONTENT:');
console.log('─'.repeat(50));

try {
  const fileContent = fs.readFileSync('AutoBotUtilities.Tests/Test Data/06FLIP-SO-0016205IN-20250514-000.PDF.txt', 'utf8');
  
  // Find all product lines in the file
  const productLinePattern = /^\d{5,6}-[\w\d]+-[\w\d]+(?:W\d+)?\s+.+?\s+\d+\s+\d+\.\d{4}\s+\d+\.\d{2}$/gm;
  const foundLines = fileContent.match(productLinePattern) || [];
  
  console.log(`Found ${foundLines.length} potential product lines in file`);
  
  if (foundLines.length > 0) {
    console.log('\nFirst 5 found lines:');
    foundLines.slice(0, 5).forEach((line, index) => {
      console.log(`${index + 1}: ${line}`);
    });
    
    // Test corrected pattern against found lines
    let fileMatchCount = 0;
    foundLines.forEach(line => {
      correctedPattern.lastIndex = 0;
      if (correctedPattern.exec(line)) {
        fileMatchCount++;
      }
    });
    
    console.log(`\n📊 FILE RESULTS: ${fileMatchCount}/${foundLines.length} found lines matched with corrected pattern`);
  }
  
} catch (error) {
  console.log('❌ Could not read file:', error.message);
}

console.log('\n💡 CONCLUSION:');
console.log('─'.repeat(20));
if (correctedMatchCount > matchCount) {
  console.log('✅ Corrected pattern performs better than current pattern');
  console.log('🔧 Recommendation: Update database regex pattern for Line 2148');
} else {
  console.log('⚠️  Pattern needs further refinement');
}

console.log('\n🎯 NEXT STEPS:');
console.log('1. Update OCR database with corrected regex pattern');
console.log('2. Test with updated pattern to verify 66+ items are extracted');
console.log('3. Validate end-to-end processing');
