#!/usr/bin/env python3

import re
import sys

def fix_variable_conflicts(filename):
    with open(filename, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Fix remaining documentType variables that haven't been fixed
    # We already have documentType1, documentType2, documentTypeDecimal1
    # So let's continue from there
    
    variable_counters = {
        'documentType': 3,  # Starting from 3 since we have 1, 2, and Decimal1
        'templateSpec': 3,   # Starting from 3 since we have 1, 2
        'validatedSpec': 3,  # Starting from 3 since we have 1, 2  
        'templateSpecificationSuccess': 3  # Starting from 3
    }
    
    lines = content.split('\n')
    modified = False
    
    for i, line in enumerate(lines):
        # Skip lines that already have numbered variables
        if re.search(r'(documentType|templateSpec|validatedSpec|templateSpecificationSuccess)\d+', line):
            continue
            
        # Fix documentType declarations that aren't already numbered
        if 'string documentType = "Invoice";' in line and 'documentType1' not in line and 'documentType2' not in line and 'documentTypeDecimal1' not in line:
            counter = variable_counters['documentType']
            lines[i] = line.replace('string documentType = "Invoice";', f'string documentType{counter} = "Invoice";')
            variable_counters['documentType'] += 1
            modified = True
            
        # Fix templateSpec declarations
        elif 'var templateSpec = TemplateSpecification.CreateForUtilityOperation(' in line and 'templateSpec1' not in line and 'templateSpec2' not in line:
            counter = variable_counters['templateSpec']  
            lines[i] = line.replace('var templateSpec =', f'var templateSpec{counter} =')
            variable_counters['templateSpec'] += 1
            modified = True
            
        # Fix validatedSpec declarations
        elif 'var validatedSpec = templateSpec' in line and 'validatedSpec1' not in line and 'validatedSpec2' not in line:
            counter = variable_counters['validatedSpec']
            lines[i] = line.replace('var validatedSpec =', f'var validatedSpec{counter} =')
            variable_counters['validatedSpec'] += 1  
            modified = True
            
        # Fix templateSpecificationSuccess declarations
        elif 'bool templateSpecificationSuccess = ' in line and 'templateSpecificationSuccess1' not in line and 'templateSpecificationSuccess2' not in line and 'templateSpecificationSuccess3' not in line:
            counter = variable_counters['templateSpecificationSuccess']
            lines[i] = line.replace('bool templateSpecificationSuccess =', f'bool templateSpecificationSuccess{counter} =')
            variable_counters['templateSpecificationSuccess'] += 1
            modified = True
    
    if modified:
        with open(filename, 'w', encoding='utf-8') as f:
            f.write('\n'.join(lines))
        print(f"Fixed variable conflicts in {filename}")
        return True
    else:
        print(f"No variable conflicts found to fix in {filename}")
        return False

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python3 fix_variables.py <filename>")
        sys.exit(1)
    
    fix_variable_conflicts(sys.argv[1])