# Referenz-Assemblies

Diese DLLs enthalten nur Schnittstellen (Klassen, Methoden, Eigenschaften), aber keinen
Programmcode. Sie werden nur zum Kompilieren gebraucht. Zur Laufzeit verwendet das Plugin die
echten Dateien, die BepInEx auf dem Rechner des Spielers erzeugt bzw. mitbringt.

| Ordner | Herkunft |
| --- | --- |
| `fm27/` | Von BepInEx erzeugte Interop-Assemblies aus Football Manager 27 (Unity 6000.3.18f1), nur Unity- und .NET-Basisbibliotheken. Keine Bibliotheken von Sports Interactive. |
| `bepinex-be755/` | BepInEx 6.0.0-be.755 (LGPL-2.1) und Il2CppInterop (LGPL-3.0) aus demselben Build. |

Neu erzeugen, z. B. fuer ein neues FM oder einen neuen BepInEx-Build:

```
./tools/make-refs.sh "<Spielordner>" fm27 be755
```

Voraussetzung: BepInEx wurde im Spiel einmal gestartet, sodass `BepInEx/interop` existiert.
