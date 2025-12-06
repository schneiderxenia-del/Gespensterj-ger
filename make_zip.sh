#!/bin/bash
# Script zum Erstellen eines Release-ZIP-Archives des Unity-Projekts
# Für Git LFS Repositories

set -e  # Beende bei Fehler

PROJECT_NAME="Gespensterjaeger"
VERSION="1.0.0"
OUTPUT_DIR="./builds"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
ZIP_NAME="${PROJECT_NAME}_v${VERSION}_${TIMESTAMP}.zip"

echo "=================================="
echo "Gespensterjaeger Build ZIP Creator"
echo "=================================="
echo ""

# Prüfe ob git lfs installiert ist
if ! command -v git-lfs &> /dev/null; then
    echo "⚠️  WARNUNG: Git LFS ist nicht installiert!"
    echo "Bitte installiere Git LFS: https://git-lfs.github.com/"
    echo ""
    read -p "Trotzdem fortfahren? (y/n) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        exit 1
    fi
fi

# Erstelle Output-Verzeichnis
mkdir -p "$OUTPUT_DIR"

echo "📦 Erstelle ZIP-Archiv..."
echo "Name: $ZIP_NAME"
echo ""

# Dateien/Ordner die inkludiert werden sollen
INCLUDE_PATTERNS=(
    "Assets/"
    "Packages/"
    "ProjectSettings/"
    "README.md"
    ".gitignore"
    ".gitattributes"
)

# Dateien/Ordner die ausgeschlossen werden sollen
EXCLUDE_PATTERNS=(
    "Library/"
    "Temp/"
    "Obj/"
    "Build/"
    "Builds/"
    "Logs/"
    "UserSettings/"
    "*.csproj"
    "*.sln"
    "*.suo"
    "*.user"
    ".vs/"
    ".vscode/"
    ".idea/"
    "builds/"
)

# Erstelle temporäres Verzeichnis für den Export
TEMP_DIR=$(mktemp -d)
EXPORT_DIR="$TEMP_DIR/$PROJECT_NAME"
mkdir -p "$EXPORT_DIR"

echo "📁 Kopiere Projektdateien..."

# Kopiere Include-Dateien
for pattern in "${INCLUDE_PATTERNS[@]}"; do
    if [ -e "$pattern" ]; then
        echo "  ✓ $pattern"
        if [ -d "$pattern" ]; then
            mkdir -p "$EXPORT_DIR/$(dirname $pattern)"
            cp -r "$pattern" "$EXPORT_DIR/$pattern"
        else
            cp "$pattern" "$EXPORT_DIR/$pattern"
        fi
    fi
done

# Entferne Exclude-Dateien
echo ""
echo "🗑️  Entferne ausgeschlossene Dateien..."
for pattern in "${EXCLUDE_PATTERNS[@]}"; do
    find "$EXPORT_DIR" -name "$pattern" -type d -exec rm -rf {} + 2>/dev/null || true
    find "$EXPORT_DIR" -name "$pattern" -type f -delete 2>/dev/null || true
done

# Erstelle ZIP
echo ""
echo "🗜️  Komprimiere..."
cd "$TEMP_DIR"
zip -r -q "$ZIP_NAME" "$PROJECT_NAME"

# Verschiebe ZIP ins Output-Verzeichnis
mv "$ZIP_NAME" "$OUTPUT_DIR/$ZIP_NAME"

# Cleanup
rm -rf "$TEMP_DIR"

# Ausgabe
OUTPUT_PATH="$OUTPUT_DIR/$ZIP_NAME"
FILE_SIZE=$(du -h "$OUTPUT_PATH" | cut -f1)

echo ""
echo "✅ Erfolgreich erstellt!"
echo "=================================="
echo "Datei: $OUTPUT_PATH"
echo "Größe: $FILE_SIZE"
echo ""
echo "📋 Inhalt des Archives:"
unzip -l "$OUTPUT_PATH" | head -n 20
echo "..."
echo ""
echo "🎉 Fertig!"
