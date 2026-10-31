#!/usr/bin/env bash
# Baut aus dem fertigen Plugin zwei Zips:
#   FMViewExporter-<version>-plugin.zip         nur das Plugin
#   FMViewExporter-<version>-komplettpaket.zip  BepInEx + Plugin, direkt in den Spielordner entpacken
# und das Setup-Programm mit eingebautem Komplettpaket:
#   FMViewExporter-<version>-setup.exe
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
cp "$ROOT/packaging/README.txt" "$PLUGIN/"
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
cp "$ROOT/packaging/README.txt" "$FULL/FM View Exporter - README.txt"
(cd "$FULL" && zip -qr "$DIST/FMViewExporter-$VERSION-komplettpaket.zip" .)

# Setup-Programm (.NET Framework 4.8) mit dem Komplettpaket als eingebetteter Ressource
dotnet build "$ROOT/src/FMViewExporter.Setup" -c Release \
  -p:PayloadZip="$DIST/FMViewExporter-$VERSION-komplettpaket.zip" \
  -p:InformationalVersion="$VERSION" -o "$STAGE/setup"
cp "$STAGE/setup/FMViewExporter-Setup.exe" "$DIST/FMViewExporter-$VERSION-setup.exe"

WINHTTP_SHA="$(sha256sum "$FULL/winhttp.dll" | cut -d' ' -f1)"
(cd "$DIST" && sha256sum ./*.zip ./*.exe > SHA256SUMS.txt)

cat > "$DIST/release-notes.md" <<NOTES
**FM View Exporter** exports the player list you are looking at in Football Manager 27 (squad, player database, shortlist) as CSV and HTML with one key press (F9). Free and open source (GPLv3), offline, never touches your save. More in the [README](https://github.com/ElectroHugin/FM-View-Exporter#readme).

| File | Use it when |
| --- | --- |
| \`FMViewExporter-$VERSION-setup.exe\` | **Recommended.** Finds FM27 on its own, installs BepInEx and the plugin, updates, uninstalls and edits the settings. |
| \`FMViewExporter-$VERSION-komplettpaket.zip\` | Manual install: BepInEx #$BEPINEX_BUILD + plugin, extract next to fm.exe. |
| \`FMViewExporter-$VERSION-plugin.zip\` | Only the plugin, if BepInEx is already installed. |

Changes: see [CHANGELOG.md](https://github.com/ElectroHugin/FM-View-Exporter/blob/main/CHANGELOG.md).
**Code signing policy:** https://github.com/ElectroHugin/FM-View-Exporter/blob/main/docs/CODE-SIGNING-POLICY.md

Deutsch: Das Setup ist der einfachste Weg (Installieren, Aktualisieren, Deinstallieren, Einstellungen). Anleitung auf Deutsch: [README.de.md](https://github.com/ElectroHugin/FM-View-Exporter/blob/main/README.de.md).

SHA-256 checksums:
\`\`\`
$(cat "$DIST/SHA256SUMS.txt")
winhttp.dll (BepInEx-Starter): $WINHTTP_SHA
\`\`\`
NOTES

rm -rf "$STAGE"
ls -la "$DIST"
