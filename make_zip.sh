#!/bin/bash

# Script to create a local ZIP bundle of the Gespensterjaeger project
# Excludes Unity-generated folders and large binary files

echo "Creating ZIP bundle for Gespensterjaeger Bow Interaction..."

# Define the output filename with timestamp
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
OUTPUT_FILE="Gespensterjaeger_Bow_${TIMESTAMP}.zip"

# Create ZIP excluding unwanted directories and files
zip -r "$OUTPUT_FILE" . \
    -x "*.git/*" \
    -x "Library/*" \
    -x "Temp/*" \
    -x "Obj/*" \
    -x "Build/*" \
    -x "Builds/*" \
    -x "Logs/*" \
    -x "*.DS_Store" \
    -x "*.vs/*" \
    -x "*.vscode/*" \
    -x "*.idea/*" \
    -x "UserSettings/*" \
    -x "*.csproj" \
    -x "*.unityproj" \
    -x "*.sln" \
    -x "*.suo" \
    -x "*.user" \
    -x "*.userprefs" \
    -x "*.pidb" \
    -x "*.booproj" \
    -x "*.svd" \
    -x "*.pdb" \
    -x "*.mdb" \
    -x "*.opendb" \
    -x "*.VC.db" \
    -x "*.apk" \
    -x "*.aab" \
    -x "*.unitypackage"

if [ $? -eq 0 ]; then
    echo "✓ ZIP bundle created successfully: $OUTPUT_FILE"
    
    # Show size of created file
    SIZE=$(du -h "$OUTPUT_FILE" | cut -f1)
    echo "  File size: $SIZE"
    
    # Show what's included
    echo ""
    echo "Included in ZIP:"
    echo "  - Assets/"
    echo "  - Packages/"
    echo "  - ProjectSettings/"
    echo "  - README.md"
    echo "  - .gitignore"
    echo ""
    echo "Excluded from ZIP:"
    echo "  - Library/ (Unity cache)"
    echo "  - Temp/ (temporary files)"
    echo "  - Build artifacts"
    echo "  - IDE configuration files"
    echo "  - UserSettings/"
else
    echo "✗ Error creating ZIP bundle"
    exit 1
fi
