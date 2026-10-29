#!/usr/bin/env bash
# Erzeugt die Referenz-Assemblies in lib/ref aus einer FM-Installation, in der BepInEx
# einmal gestartet wurde. Die Dateien enthalten nur Schnittstellen, keinen Programmcode.
#
#   ./tools/make-refs.sh "<Spielordner>" fm27 be755
#
# Benoetigt: dotnet SDK und das Tool Refasmer (dotnet tool install -g JetBrains.Refasmer.CliTool).
set -euo pipefail

GAME="$1"
GAME_TAG="${2:-fm27}"
BEPINEX_TAG="${3:-be755}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
GAME_OUT="$ROOT/lib/ref/$GAME_TAG"
BEPINEX_OUT="$ROOT/lib/ref/bepinex-$BEPINEX_TAG"
mkdir -p "$GAME_OUT" "$BEPINEX_OUT"

# Nur Unity- und .NET-Basisbibliotheken. Bibliotheken von Sports Interactive (SI.*, FM.*)
# werden bewusst nicht referenziert; das Plugin spricht sie zur Laufzeit ueber Namen an.
GAME_DLLS=(
  Il2Cppmscorlib Il2CppSystem Il2CppSystem.Core
  UnityEngine UnityEngine.CoreModule UnityEngine.UIElementsModule
  UnityEngine.TextCoreTextEngineModule UnityEngine.TextCoreFontEngineModule
  UnityEngine.HierarchyCoreModule UnityEngine.IMGUIModule UnityEngine.InputLegacyModule
  UnityEngine.InputForUIModule UnityEngine.PropertiesModule UnityEngine.SharedInternalsModule
  UnityEngine.TextRenderingModule UnityEngine.JSONSerializeModule
)
BEPINEX_DLLS=(
  BepInEx.Core BepInEx.Unity.IL2CPP BepInEx.Unity.Common BepInEx.Preloader.Core
  Il2CppInterop.Runtime Il2CppInterop.Common 0Harmony SemanticVersioning
)

for dll in "${GAME_DLLS[@]}"; do
  refasmer -c --omit-non-api-members=false -O "$GAME_OUT" "$GAME/BepInEx/interop/$dll.dll"
done
for dll in "${BEPINEX_DLLS[@]}"; do
  refasmer -c --omit-non-api-members=true -O "$BEPINEX_OUT" "$GAME/BepInEx/core/$dll.dll"
done

echo "Fertig: $GAME_OUT, $BEPINEX_OUT"
