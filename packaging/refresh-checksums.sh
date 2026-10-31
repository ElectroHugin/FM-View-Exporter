#!/usr/bin/env bash
# Prueft die Pruefsummen nach dem Signieren neu (das signierte Setup hat eine andere SHA-256)
# und traegt sie in SHA256SUMS.txt und dist/release-notes.md ein.
set -euo pipefail
DIST="$(cd "$(dirname "$0")/.." && pwd)/dist"
(cd "$DIST" && sha256sum ./*.zip ./*.exe > SHA256SUMS.txt)
python3 - "$DIST" <<'PY'
import pathlib, sys
dist = pathlib.Path(sys.argv[1])
notes = dist / "release-notes.md"
sums = (dist / "SHA256SUMS.txt").read_text().splitlines()
lines = notes.read_text().splitlines()
start = lines.index("```") + 1
end = next(i for i in range(start, len(lines)) if lines[i].startswith("winhttp.dll"))
notes.write_text("\n".join(lines[:start] + sums + lines[end:]) + "\n")
PY
echo "Pruefsummen nach dem Signieren aktualisiert."
