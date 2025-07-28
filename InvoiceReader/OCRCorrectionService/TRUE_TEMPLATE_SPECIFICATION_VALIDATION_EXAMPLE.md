# 🎯 TRUE TEMPLATE SPECIFICATION VALIDATION EXAMPLE

## ❌ CURRENT WRONG APPROACH
```csharp
// This validates AI recommendation QUALITY, not data against specs
var entityTypeRecommendations = recommendations?.Where(r => 
    r.Description.Contains("Invoice") || r.Description.Contains("InvoiceDetails"))
bool success = entityTypeRecommendations.Any();
```

## ✅ TRUE TEMPLATE SPECIFICATION VALIDATION

### **1. REQUIRED FIELDS VALIDATION**
```csharp
public static TemplateSpecification ValidateRequiredFields(this TemplateSpecification spec, Invoice invoice)
{
    if (spec.HasFailure) return spec; // Short-circuit

    var missingFields = new List<string>();
    
    // Based on Template_Specifications.md - Invoice Entity Required Fields:
    if (string.IsNullOrEmpty(invoice.InvoiceNo)) missingFields.Add("InvoiceNo");
    if (!invoice.InvoiceTotal.HasValue || invoice.InvoiceTotal == 0) missingFields.Add("InvoiceTotal");
    if (string.IsNullOrEmpty(invoice.SupplierCode) && string.IsNullOrEmpty(invoice.SupplierName)) 
        missingFields.Add("SupplierCode or SupplierName");
    
    bool success = !missingFields.Any();
    var result = success 
        ? TemplateValidationResult.Success("TEMPLATE_SPEC_REQUIRED_FIELDS", 
            "All required Invoice fields are present and valid")
        : TemplateValidationResult.Failure("TEMPLATE_SPEC_REQUIRED_FIELDS", 
            $"Missing required fields: {string.Join(", ", missingFields)}");
    
    spec.ValidationResults.Add(result);
    return spec;
}
```

### **2. DATA TYPE VALIDATION**
```csharp
public static TemplateSpecification ValidateDataTypes(this TemplateSpecification spec, Invoice invoice)
{
    if (spec.HasFailure) return spec; // Short-circuit

    var dataTypeErrors = new List<string>();
    
    // Based on Template_Specifications.md - Data Type System:
    if (invoice.InvoiceTotal.HasValue && invoice.InvoiceTotal < 0) 
        dataTypeErrors.Add("InvoiceTotal must be positive numeric value");
    
    if (!string.IsNullOrEmpty(invoice.InvoiceDate?.ToString()) && !DateTime.TryParse(invoice.InvoiceDate.ToString(), out _))
        dataTypeErrors.Add("InvoiceDate must be valid date format");
    
    if (!string.IsNullOrEmpty(invoice.InvoiceNo) && invoice.InvoiceNo.Length > 100)
        dataTypeErrors.Add("InvoiceNo exceeds maximum string length");
    
    bool success = !dataTypeErrors.Any();
    var result = success 
        ? TemplateValidationResult.Success("TEMPLATE_SPEC_DATA_TYPES", 
            "All field data types are valid according to template specifications")
        : TemplateValidationResult.Failure("TEMPLATE_SPEC_DATA_TYPES", 
            $"Data type violations: {string.Join(", ", dataTypeErrors)}");
    
    spec.ValidationResults.Add(result);
    return spec;
}
```

### **3. ENTITY TYPE MAPPING VALIDATION**
```csharp
public static TemplateSpecification ValidateEntityTypeMappings(this TemplateSpecification spec, List<TemplateField> templateFields)
{
    if (spec.HasFailure) return spec; // Short-circuit

    var mappingErrors = new List<string>();
    
    // Based on Template_Specifications.md - EntityType Mapping rules:
    foreach (var field in templateFields)
    {
        bool validMapping = field.EntityType switch
        {
            "Invoice" => IsValidInvoiceField(field.FieldName), // InvoiceNo, InvoiceDate, InvoiceTotal, etc.
            "InvoiceDetails" => IsValidInvoiceDetailsField(field.FieldName), // ItemNumber, Quantity, Cost, etc.
            "ShipmentBL" => IsValidShipmentBLField(field.FieldName), // BLNumber, Vessel, Container, etc.
            _ => false
        };
        
        if (!validMapping)
            mappingErrors.Add($"Field '{field.FieldName}' incorrectly mapped to EntityType '{field.EntityType}'");
    }
    
    bool success = !mappingErrors.Any();
    var result = success 
        ? TemplateValidationResult.Success("TEMPLATE_SPEC_ENTITY_MAPPING", 
            $"All {templateFields.Count} field mappings are valid for their EntityTypes")
        : TemplateValidationResult.Failure("TEMPLATE_SPEC_ENTITY_MAPPING", 
            $"Invalid mappings: {string.Join("; ", mappingErrors)}");
    
    spec.ValidationResults.Add(result);
    return spec;
}

private static bool IsValidInvoiceField(string fieldName)
{
    // Based on Template_Specifications.md Invoice Entity fields
    var validInvoiceFields = new[] { "InvoiceNo", "InvoiceDate", "InvoiceTotal", "SubTotal", 
        "Currency", "SupplierCode", "PONumber", "SupplierName" };
    return validInvoiceFields.Contains(fieldName);
}

private static bool IsValidInvoiceDetailsField(string fieldName)
{
    // Based on Template_Specifications.md InvoiceDetails Entity fields  
    var validDetailFields = new[] { "ItemNumber", "ItemDescription", "Quantity", 
        "Cost", "TotalCost", "Units", "ProductCode", "UnitPrice", "ExtendedPrice" };
    return validDetailFields.Contains(fieldName);
}
```

### **4. REGEX PATTERN QUALITY VALIDATION**
```csharp
public static TemplateSpecification ValidateRegexPatterns(this TemplateSpecification spec, List<TemplatePattern> patterns)
{
    if (spec.HasFailure) return spec; // Short-circuit

    var patternErrors = new List<string>();
    
    foreach (var pattern in patterns)
    {
        // Validate named capture groups with EntityType prefix
        if (!pattern.RegexPattern.Contains("(?<"))
            patternErrors.Add($"Pattern '{pattern.PatternName}' missing named capture groups");
        
        // Validate EntityType prefix in capture groups (e.g., (?<Invoice_InvoiceNo>...))
        var expectedPrefix = $"(?<{pattern.EntityType}_";
        if (!pattern.RegexPattern.Contains(expectedPrefix))
            patternErrors.Add($"Pattern '{pattern.PatternName}' missing EntityType prefix in capture group");
        
        // Validate regex syntax
        try { new Regex(pattern.RegexPattern); }
        catch { patternErrors.Add($"Pattern '{pattern.PatternName}' has invalid regex syntax"); }
    }
    
    bool success = !patternErrors.Any();
    var result = success 
        ? TemplateValidationResult.Success("TEMPLATE_SPEC_REGEX_QUALITY", 
            $"All {patterns.Count} regex patterns are well-formed with proper EntityType naming")
        : TemplateValidationResult.Failure("TEMPLATE_SPEC_REGEX_QUALITY", 
            $"Pattern issues: {string.Join("; ", patternErrors)}");
    
    spec.ValidationResults.Add(result);
    return spec;
}
```

### **5. BUSINESS RULE COMPLIANCE VALIDATION**
```csharp
public static TemplateSpecification ValidateBusinessRules(this TemplateSpecification spec, Invoice invoice, List<TemplateField> templateFields)
{
    if (spec.HasFailure) return spec; // Short-circuit

    var businessRuleViolations = new List<string>();
    
    // Based on Template_Specifications.md IsRequired Field Strategy:
    var requiredFields = templateFields.Where(f => f.IsRequired).ToList();
    var multiPatternFields = templateFields.GroupBy(f => f.FieldName).Where(g => g.Count() > 1);
    
    foreach (var multiField in multiPatternFields)
    {
        if (multiField.Any(f => f.IsRequired))
            businessRuleViolations.Add($"Field '{multiField.Key}' has multiple patterns but some marked as required (should be IsRequired=0)");
    }
    
    // Validate AppendValues logic
    var appendFields = templateFields.Where(f => f.AppendValues).ToList();
    foreach (var field in appendFields)
    {
        if (field.DataType != "String")
            businessRuleViolations.Add($"Field '{field.FieldName}' has AppendValues=true but DataType is not String");
    }
    
    bool success = !businessRuleViolations.Any();
    var result = success 
        ? TemplateValidationResult.Success("TEMPLATE_SPEC_BUSINESS_RULES", 
            "All template fields comply with business rules from Template_Specifications.md")
        : TemplateValidationResult.Failure("TEMPLATE_SPEC_BUSINESS_RULES", 
            $"Business rule violations: {string.Join("; ", businessRuleViolations)}");
    
    spec.ValidationResults.Add(result);
    return spec;
}
```

## ✅ TRUE VALIDATION USAGE EXAMPLE

```csharp
// Create template specification for detected document type
var templateSpec = TemplateSpecification.CreateForInvoice(); // Based on actual document type

// Validate ACTUAL DATA against template specifications
var validatedSpec = templateSpec
    .ValidateRequiredFields(invoice)           // Check if invoice has required fields
    .ValidateDataTypes(invoice)                // Check if field values have correct data types  
    .ValidateEntityTypeMappings(templateFields) // Check if template mappings are correct
    .ValidateRegexPatterns(regexPatterns)      // Check if regex patterns are well-formed
    .ValidateBusinessRules(invoice, templateFields); // Check business rule compliance

// Log validation results
validatedSpec.LogValidationResults(_logger);

// Get overall validation result
bool templateSpecificationSuccess = validatedSpec.IsValid;
```

## 🎯 KEY DIFFERENCES

| Aspect | Wrong Approach (Current) | Correct Approach (True Validation) |
|--------|-------------------------|-----------------------------------|
| **Validates** | AI recommendation quality | Actual data against specifications |
| **Input Data** | List<PromptRecommendation> | Invoice, TemplateField[], RegexPattern[] |
| **Validation Logic** | Contains("Invoice") keyword checks | Required field presence, data type validation |
| **Purpose** | Check if AI suggestions are good | Check if data meets template specs |
| **Based On** | Arbitrary keyword matching | Template_Specifications.md rules |
| **Business Value** | Low - just recommendation quality | High - ensures data integrity |

## 🚨 CONCLUSION

**My current implementation is fundamentally wrong.** It validates recommendation quality, not template specification compliance. True template specification validation must validate actual business data against the rules defined in Template_Specifications.md.