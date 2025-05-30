param(
    [Parameter(Mandatory=$true)]
    [string]$RootPath,

    [Parameter(Mandatory=$false)]
    [int]$MaxLinesPerFile = 300,

    [Parameter(Mandatory=$false)]
    [string[]]$ExcludePatterns = @("*.designer.cs", "*.g.cs", "*.g.i.cs", "*AssemblyInfo.cs") # Add patterns to exclude auto-generated files
)

# Function to find the next non-empty, non-comment line index
function Get-NextCodeLineIndex {
    param(
        [string[]]$Lines,
        [int]$StartIndex
    )
    for ($i = $StartIndex; $i -lt $Lines.Length; $i++) {
        $trimmedLine = $Lines[$i].Trim()
        if ($trimmedLine -ne "" -and !$trimmedLine.StartsWith("//") -and !$trimmedLine.StartsWith("/*") -and !$trimmedLine.StartsWith("*")) {
            return $i
        }
    }
    return -1 # Not found
}

# Function to find a potentially safer split point (end of a member)
function Find-MemberSplitPoint {
    param(
        [string[]]$Lines,
        [int]$TargetLineIndex # The line number around which we want to split
    )
    # Search backwards slightly from the target line for a potential end-of-member brace
    $searchEnd = $TargetLineIndex + 20 # Look a bit beyond the target
    if ($searchEnd -ge $Lines.Length) { $searchEnd = $Lines.Length - 1 }
    $searchStart = $TargetLineIndex - 20
    if ($searchStart -lt 0) { $searchStart = 0 }

    for ($i = $searchEnd; $i -ge $searchStart; $i--) {
        # Look for a line consisting only of a closing brace (and whitespace)
        if ($Lines[$i].Trim() -eq "}") {
            # Now check the *next* significant line to see if it looks like a new member start
            $nextLineIndex = Get-NextCodeLineIndex -Lines $Lines -StartIndex ($i + 1)
            if ($nextLineIndex -ne -1) {
                $nextLine = $Lines[$nextLineIndex].Trim()
                # Heuristic: Check for common member starting patterns
                if ($nextLine.StartsWith("public") -or
                    $nextLine.StartsWith("private") -or
                    $nextLine.StartsWith("internal") -or
                    $nextLine.StartsWith("protected") -or
                    $nextLine.StartsWith("static") -or
                    $nextLine.StartsWith("async") -or
                    $nextLine.StartsWith("void") -or # Common return type start
                    $nextLine.StartsWith("int") -or # Common return type start
                    $nextLine.StartsWith("string") -or # Common return type start
                    $nextLine.StartsWith("bool") -or # Common return type start
                    $nextLine.StartsWith("[") -or # Attribute
                    $nextLine.StartsWith("///") -or # XML Doc Comment
                    $nextLine -eq "}" # End of the class itself
                   ) {
                    Write-Verbose "Found potential member split point after line $i"
                    return $i + 1 # Split *after* this closing brace line
                }
            } elseif ($i + 1 -ge $Lines.Length) {
                 # If the brace is the very last line, it's a valid split point
                 return $i + 1
            }
        }
    }
    # Fallback: If no better point found near the target, split at the target line count (original risky behavior)
    Write-Warning "Could not find ideal member boundary split point near line $TargetLineIndex. Splitting directly."
    return $TargetLineIndex
}

# Get all C# files recursively
Get-ChildItem -Path $RootPath -Recurse -Filter *.cs | ForEach-Object {
    $file = $_
    $filePath = $file.FullName
    $fileName = $file.Name
    $directory = $file.DirectoryName

    # Skip excluded files
    $excluded = $false
    foreach ($pattern in $ExcludePatterns) {
        if ($fileName -like $pattern) {
            $excluded = $true
            break
        }
    }
    if ($excluded) {
        Write-Host "Skipping excluded file: $filePath"
        return # Continue to next file using 'return' in ForEach-Object
    }

    # Read content and check line count
    try {
        # Read file content once
        $lines = Get-Content -Path $filePath -Raw -Encoding UTF8 -ErrorAction Stop
        $lineArray = $lines -split '\r?\n'
        $lineCount = $lineArray.Length

        if ($lineCount -gt $MaxLinesPerFile) {
            Write-Host "Processing '$fileName' ($lineCount lines)..."

            # Find class/interface/struct declaration and add 'partial' if missing
            $classDeclarationLineIndex = -1
            $classDeclaration = ""
            $namespaceLine = ""
            $usings = @()
            $isPartial = $false
            $classBodyStartIndex = -1
            $namespaceBodyStartIndex = -1

            for ($i = 0; $i -lt $lineArray.Length; $i++) {
                 if ($lineArray[$i] -match '^\s*using\s+.*;') {
                    $usings += $lineArray[$i]
                 }
                 elseif ($namespaceLine -eq "" -and $lineArray[$i] -match '^\s*namespace\s+.*') {
                    $namespaceLine = $lineArray[$i]
                    # Find the opening brace for the namespace
                    for ($j = $i + 1; $j -lt $lineArray.Length; $j++) {
                        if ($lineArray[$j].Trim() -eq "{") { $namespaceBodyStartIndex = $j; break }
                    }
                 }
                 elseif ($classDeclarationLineIndex -eq -1 -and $lineArray[$i] -match '^\s*(public|internal|private)?\s*(sealed|abstract|static)?\s*(class|interface|struct)\s+(\w+)') {
                    $classDeclarationLineIndex = $i
                    $classDeclaration = $lineArray[$i]
                    if ($classDeclaration -match '\bpartial\b') {
                        $isPartial = $true
                    }
                     # Find the opening brace for the class
                    for ($j = $i + 1; $j -lt $lineArray.Length; $j++) {
                        if ($lineArray[$j].Trim() -eq "{") { $classBodyStartIndex = $j; break }
                    }
                    # Don't break immediately, might need namespace line found later
                 }
            }

            if ($classDeclarationLineIndex -eq -1) {
                Write-Warning "Could not find class/interface/struct declaration in '$fileName'. Skipping."
                return
            }
             if ($classBodyStartIndex -eq -1) {
                Write-Warning "Could not find opening brace '{' for class in '$fileName'. Skipping."
                return
            }

            # Modify original class declaration to be partial if it isn't already
            $modifiedClassDeclaration = $lineArray[$classDeclarationLineIndex] # Start with original
            if (-not $isPartial) {
                $originalClassLine = $lineArray[$classDeclarationLineIndex]
                # Use regex to insert 'partial' carefully
                $pattern = '^(\s*(?:public|internal|private)?\s*(?:sealed|abstract|static)?\s*)(class|interface|struct)(\s+\w+.*)$'
                if ($originalClassLine -match $pattern) {
                    $modifiedClassDeclaration = $originalClassLine -replace $pattern, ('${1}partial ${2}${3}')
                    Write-Host "  Made class/struct/interface partial."
                } else {
                     Write-Warning "  Could not parse declaration to add partial: $originalClassLine"
                     # Proceeding without making it partial, might cause issues if split
                }
            }

            # --- Splitting Logic ---
            $currentLineIndexInOriginal = $classBodyStartIndex + 1 # Start reading content *after* class opening brace
            $partNumber = 1
            $baseName = $file.BaseName
            $linesProcessedInOriginal = $classBodyStartIndex + 1 # How many lines of the original file are included up to the class opening brace

            while ($currentLineIndexInOriginal -lt $lineArray.Length -1) { # Stop before the last closing brace of the class
                $linesRemainingInBody = ($lineArray.Length - 1) - $currentLineIndexInOriginal
                $linesToPotentiallyTake = $MaxLinesPerFile

                # Adjust lines for the first part to account for header (usings, namespace, class decl)
                if ($partNumber -eq 1) {
                    $linesToPotentiallyTake = $MaxLinesPerFile - $linesProcessedInOriginal
                    if ($linesToPotentiallyTake -lt 50) {$linesToPotentiallyTake = 50} # Ensure first part isn't too small
                }

                $splitPointIndex = $currentLineIndexInOriginal + $linesRemainingInBody # Default to taking all remaining lines
                $linesInThisPart = $linesRemainingInBody

                if ($linesRemainingInBody -gt $linesToPotentiallyTake * 1.1) { # Only split if significantly more lines remain
                    $targetSplitLine = $currentLineIndexInOriginal + $linesToPotentiallyTake
                    $splitPointIndex = Find-MemberSplitPoint -Lines $lineArray -TargetLineIndex $targetSplitLine
                    $linesInThisPart = $splitPointIndex - $currentLineIndexInOriginal
                }

                if ($linesInThisPart -le 0) { # Safety check if split point logic fails
                     Write-Warning "  Calculated zero lines for part $partNumber. Breaking loop."
                     break
                }

                $endIndexExclusive = $currentLineIndexInOriginal + $linesInThisPart

                # Determine output filename
                $outputFileName = ""
                if ($partNumber -eq 1) {
                    $outputFileName = $filePath # Overwrite original file
                } else {
                    $outputFileName = Join-Path -Path $directory -ChildPath "$($baseName).Part$($partNumber).cs"
                }

                # Prepare content for this part
                $partContent = @()
                if ($partNumber -eq 1) {
                    # First part: includes original header up to class opening brace
                    $partContent += $lineArray[0..$classDeclarationLineIndex-1] # Usings, namespace, etc.
                    $partContent += $modifiedClassDeclaration # The (potentially modified) class declaration
                    $partContent += $lineArray[$classDeclarationLineIndex+1..$classBodyStartIndex] # Lines between decl and brace, including brace
                } else {
                    # Subsequent parts: add header boilerplate
                    $partContent += $usings
                    $partContent += ""
                    if ($namespaceLine) { $partContent += $namespaceLine; $partContent += "{" }
                    $partContent += $modifiedClassDeclaration # Add the partial class declaration
                    $partContent += "{" # Add opening brace for class
                }

                # Add the lines for this specific part's body
                $partContent += $lineArray[$currentLineIndexInOriginal..($endIndexExclusive - 1)]

                # Add closing braces
                if ($partNumber -eq 1) {
                     # If this is the *only* part (no split happened), add the original closing brace(s)
                     if ($currentLineIndexInOriginal + $linesInThisPart -ge $lineArray.Length -1) {
                         $partContent += $lineArray[($lineArray.Length-1)] # Add the last line (class closing brace)
                         if ($namespaceLine -and $lineArray.Length > 1) {
                             # Potentially add namespace closing brace if it was the second to last line
                             # This is heuristic, assumes simple namespace { class { ... } } structure
                             # A more robust parser would be needed for complex cases
                         }
                     }
                } else {
                     # Subsequent parts always need closing braces added
                     $partContent += "}" # Close class
                     if ($namespaceLine) { $partContent += "}" } # Close namespace
                }


                # Write the content to the file
                Write-Host "  Writing part $partNumber to '$outputFileName' ($($linesInThisPart) body lines)"
                try {
                    # Ensure directory exists (should already, but safety check)
                    if (!(Test-Path -Path (Split-Path $outputFileName -Parent))) {
                        New-Item -ItemType Directory -Path (Split-Path $outputFileName -Parent) | Out-Null
                    }
                    Set-Content -Path $outputFileName -Value ($partContent -join [Environment]::NewLine) -Encoding UTF8 -ErrorAction Stop -Force
                } catch {
                    Write-Error "Failed to write file '$outputFileName': $_"
                }

                $currentLineIndexInOriginal = $endIndexExclusive
                $partNumber++

                 # Safety break to prevent infinite loops in case of logic error
                if ($partNumber -gt 50) {
                    Write-Error "Exceeded maximum part number (50) for file '$fileName'. Stopping split for this file."
                    break
                }
            }
             # Overwrite the original file with the potentially modified first part
             # This happens implicitly if $partNumber remains 1 and the file is written
             # If $partNumber > 1, the first part was already written to the original path
             if ($partNumber -gt 1) {
                 Write-Host "  Original file '$fileName' updated with first part and partial modifier (if needed)."
             }

        } # End if lineCount > MaxLinesPerFile
    } catch {
        Write-Error "Error processing file '$filePath': $_"
    }
}

Write-Host "Script finished. PLEASE REVIEW ALL MODIFIED AND CREATED FILES."