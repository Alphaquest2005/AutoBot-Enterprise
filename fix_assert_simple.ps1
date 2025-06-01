# Simple PowerShell script to fix NUnit Assert syntax
$files = Get-ChildItem -Path "AutoBotUtilities.Tests" -Filter "*.cs" -Recurse

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    if ($content -match "Assert\.(AreEqual|IsTrue|IsFalse|IsNotNull|IsNull|Greater|AreSame)") {
        Write-Host "Fixing: $($file.Name)"
        
        # Fix the most common patterns
        $content = $content -replace 'Assert\.AreEqual\(([^,]+),\s*([^,)]+)\)', 'Assert.That($2, Is.EqualTo($1))'
        $content = $content -replace 'Assert\.AreEqual\(([^,]+),\s*([^,]+),\s*([^)]+)\)', 'Assert.That($2, Is.EqualTo($1), $3)'
        $content = $content -replace 'Assert\.IsTrue\(([^,)]+)\)', 'Assert.That($1, Is.True)'
        $content = $content -replace 'Assert\.IsTrue\(([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.True, $2)'
        $content = $content -replace 'Assert\.IsFalse\(([^,)]+)\)', 'Assert.That($1, Is.False)'
        $content = $content -replace 'Assert\.IsFalse\(([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.False, $2)'
        $content = $content -replace 'Assert\.IsNotNull\(([^,)]+)\)', 'Assert.That($1, Is.Not.Null)'
        $content = $content -replace 'Assert\.IsNotNull\(([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.Not.Null, $2)'
        $content = $content -replace 'Assert\.IsNull\(([^,)]+)\)', 'Assert.That($1, Is.Null)'
        $content = $content -replace 'Assert\.IsNull\(([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.Null, $2)'
        $content = $content -replace 'Assert\.Greater\(([^,]+),\s*([^,)]+)\)', 'Assert.That($1, Is.GreaterThan($2))'
        $content = $content -replace 'Assert\.Greater\(([^,]+),\s*([^,]+),\s*([^)]+)\)', 'Assert.That($1, Is.GreaterThan($2), $3)'
        $content = $content -replace 'Assert\.AreSame\(([^,]+),\s*([^,)]+)\)', 'Assert.That($2, Is.SameAs($1))'
        $content = $content -replace 'Assert\.AreSame\(([^,]+),\s*([^,]+),\s*([^)]+)\)', 'Assert.That($2, Is.SameAs($1), $3)'
        
        Set-Content -Path $file.FullName -Value $content -NoNewline
    }
}
Write-Host "Done!"
