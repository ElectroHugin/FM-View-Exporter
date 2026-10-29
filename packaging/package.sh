#!/usr/bin/env bash
# Baut aus dem fertigen Plugin zwei Zips:
#   FMViewExporter-<version>-plugin.zip         nur das Plugin
#   FMViewExporter-<version>-komplettpaket.zip  BepInEx + Plugin, direkt in den Spielordner entpacken
set -euo pipefail

VERSION="${1:-dev}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
# shellcheck source=bepinex.env
source "$ROOT/packaging/bepinex.env"
BEPINEX_URL="${BEPINEX_URL_OVERRIDE:-$BEPINEX_URL}"

DIST="$ROOT/dist"
STAGE="$DIST/stage"
PLUGIN_DLL="$ROOT/src/FMViewExporter/bin/Release/net6.0/FMViewExporter.dll"

rm -rf "$DIST"
mkdir -p "$STAGE"

# Nur das Plugin
PLUGIN="$STAGE/plugin/BepInEx/plugins/FMViewExporter"
mkdir -p "$PLUGIN"
cp "$PLUGIN_DLL" "$PLUGIN/"
cp "$ROOT/LICENSE" "$PLUGIN/LICENSE.txt"
cp "$ROOT/packaging/LIESMICH.txt" "$PLUGIN/"
(cd "$STAGE/plugin" && zip -qr "$DIST/FMViewExporter-$VERSION-plugin.zip" .)

# Komplettpaket mit BepInEx
FULL="$STAGE/full"
mkdir -p "$FULL"
curl -fsSL --retry 3 "$BEPINEX_URL" -o "$STAGE/bepinex.zip"
ACTUAL_SHA="$(sha256sum "$STAGE/bepinex.zip" | cut -d' ' -f1)"
echo "BepInEx #$BEPINEX_BUILD zip sha256: $ACTUAL_SHA"
if [ -n "${BEPINEX_SHA256:-}" ] && [ "$ACTUAL_SHA" != "$BEPINEX_SHA256" ]; then
  echo "::error::BepInEx-Pruefsumme stimmt nicht: erwartet $BEPINEX_SHA256, erhalten $ACTUAL_SHA"
  exit 1
fi
unzip -q "$STAGE/bepinex.zip" -d "$FULL"
check_file() {
  local file="$1" expected="$2" actual
  actual="$(sha256sum "$file" | cut -d' ' -f1)"
  if [ -n "$expected" ] && [ "$actual" != "$expected" ]; then
    echo "::error::Pruefsumme von $(basename "$file") stimmt nicht: erwartet $expected, erhalten $actual"
    exit 1
  fi
}
check_file "$FULL/BepInEx/core/BepInEx.Core.dll" "${BEPINEX_CORE_SHA256:-}"
check_file "$FULL/winhttp.dll" "${WINHTTP_SHA256:-}"
cp -r "$STAGE/plugin/BepInEx" "$FULL/"
mkdir -p "$FULL/BepInEx/plugins/FMViewExporter/licenses"
cp "$ROOT/packaging/THIRD-PARTY-NOTICES.txt" "$FULL/BepInEx/plugins/FMViewExporter/"
cp "$ROOT/packaging/licenses/"*.txt "$FULL/BepInEx/plugins/FMViewExporter/licenses/"
cp "$ROOT/packaging/LIESMICH.txt" "$FULL/FM View Exporter - LIESMICH.txt"
(cd "$FULL" && zip -qr "$DIST/FMViewExporter-$VERSION-komplettpaket.zip" .)

WINHTTP_SHA="$(sha256sum "$FULL/winhttp.dll" | cut -d' ' -f1)"
(cd "$DIST" && sha256sum ./*.zip > SHA256SUMS.txt)

cat > "$DIST/release-notes.md" <<NOTES
**Komplettpaket** (BepInEx #$BEPINEX_BUILD + Plugin): Inhalt direkt in den Spielordner entpacken, neben fm.exe.
**Plugin**: nur das Plugin, wenn BepInEx schon installiert ist.

Anleitung: siehe \`FM View Exporter - LIESMICH.txt\` im Komplettpaket.

Pruefsummen (SHA-256):
\`\`\`
$(cat "$DIST/SHA256SUMS.txt")
winhttp.dll (BepInEx-Starter): $WINHTTP_SHA
\`\`\`
NOTES

rm -rf "$STAGE"
ls -la "$DIST"
