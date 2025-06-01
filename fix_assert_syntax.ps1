# PowerShell script to fix NUnit Assert syntax in test files
# Converts old syntax to new Assert.That syntax

$testFiles = Get-ChildItem -Path "AutoBotUtilities.Tests" -Filter "*.cs" -Recurse

foreach ($file in $testFiles) {
    Write-Host "Processing: $($file.FullName)"
    
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Fix Assert.AreEqual(expected, actual) -> Assert.That(actual, Is.EqualTo(expected))
    $content = $content -replace 'Assert\.AreEqual\(([^,]+),\s*([^,)]+)\)', 'Assert.That($2, Is.EqualTo($1))'
    
    # Fix Assert.AreEqual(expected, actual, message) -> Assert.That(actual, Is.EqualTo(expected), message)
    $content = $content -replace 'Assert\.AreEqual\(([^,]+),\s*([^,]+),\s*([^)]+)\)', 'Assert.That($2, Is.EqualTo($1), $3)'
    
    # Fix Assert.IsTrue(condition) -> Assert.That(condition, Is.True)
    $content = $content -replace 'Assert\.IsTrue\(([^,)]+)\)', 'Assert.That($1, Is.True)'
    
    # Fix Assert.IsTrue(condition, message) -> Assert.That(condition, Is.True, message)
    $content = $content -replace 'Assert\.IsTrue\(([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.True, $2)'
    
    # Fix Assert.IsFalse(condition) -> Assert.That(condition, Is.False)
    $content = $content -replace 'Assert\.IsFalse\(([^,)]+)\)', 'Assert.That($1, Is.False)'
    
    # Fix Assert.IsFalse(condition, message) -> Assert.That(condition, Is.False, message)
    $content = $content -replace 'Assert\.IsFalse\(([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.False, $2)'
    
    # Fix Assert.IsNotNull(object) -> Assert.That(object, Is.Not.Null)
    $content = $content -replace 'Assert\.IsNotNull\(([^,)]+)\)', 'Assert.That($1, Is.Not.Null)'
    
    # Fix Assert.IsNotNull(object, message) -> Assert.That(object, Is.Not.Null, message)
    $content = $content -replace 'Assert\.IsNotNull\(([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.Not.Null, $2)'
    
    # Fix Assert.IsNull(object) -> Assert.That(object, Is.Null)
    $content = $content -replace 'Assert\.IsNull\(([^,)]+)\)', 'Assert.That($1, Is.Null)'
    
    # Fix Assert.IsNull(object, message) -> Assert.That(object, Is.Null, message)
    $content = $content -replace 'Assert\.IsNull\(([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.Null, $2)'
    
    # Fix Assert.Greater(actual, expected) -> Assert.That(actual, Is.GreaterThan(expected))
    $content = $content -replace 'Assert\.Greater\(([^,]+),\s*([^,)]+)\)', 'Assert.That($1, Is.GreaterThan($2))'
    
    # Fix Assert.Greater(actual, expected, message) -> Assert.That(actual, Is.GreaterThan(expected), message)
    $content = $content -replace 'Assert\.Greater\(([^,]+),\s*([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.GreaterThan($2), $3)'
    
    # Fix Assert.AreSame(expected, actual) -> Assert.That(actual, Is.SameAs(expected))
    $content = $content -replace 'Assert\.AreSame\(([^,]+),\s*([^,)]+)\)', 'Assert.That($2, Is.SameAs($1))'
    
    # Fix Assert.AreSame(expected, actual, message) -> Assert.That(actual, Is.SameAs(expected), message)
    $content = $content -replace 'Assert\.AreSame\(([^,]+),\s*([^,]+),\s*([^)]+)\)', 'Assert.That($2, Is.SameAs($1), $3)'
    
    # Only write if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  Updated: $($file.Name)"
    } else {
        Write-Host "  No changes: $($file.Name)"
    }
}

Write-Host "Assert syntax fix completed!"
