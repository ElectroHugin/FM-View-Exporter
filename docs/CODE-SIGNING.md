# Signieren über SignPath (Anleitung für den Maintainer)

Ziel: Die `FMViewExporter-<version>-setup.exe` wird bei jedem Release signiert. Windows Defender
und SmartScreen warnen dann deutlich seltener. Das kostet nichts: Die
[SignPath Foundation](https://signpath.org) stellt Open-Source-Projekten ein Zertifikat
kostenlos bereit, und [SignPath.io](https://about.signpath.io) übernimmt das Signieren.

Die öffentliche Code signing policy steht in [CODE-SIGNING-POLICY.md](CODE-SIGNING-POLICY.md).
Das README verlinkt sie nur noch klein am Ende (SignPath verlangt den Begriff „Code signing
policy“ als Link oder Überschrift auf der Startseite und auf den Release-Seiten).

## Was im Repository schon vorbereitet ist

- `docs/CODE-SIGNING-POLICY.md` mit Rollen, Datenschutz und Hinweis auf SignPath. README
  (englisch und deutsch) und Release-Notes verlinken darauf.
- `.github/workflows/build.yml` signiert das Setup bei einem Release (Tag `v…`), aber nur,
  wenn die SignPath-Variablen gesetzt sind. Vorher läuft alles wie bisher, unsigniert.
- `packaging/refresh-checksums.sh` trägt nach dem Signieren die neuen Prüfsummen in
  `SHA256SUMS.txt` und die Release-Notes ein.
- Produktname in allen Dateien: `FM View Exporter` (aus `Directory.Build.props`).

## Bedingungen der SignPath Foundation (Stand 31.10.2026)

- OSI-Lizenz für alle Teile: GPLv3 (Plugin, Setup), BepInEx LGPL. Erfüllt.
- Keine proprietären Teile und keine Schadfunktionen. Erfüllt.
- Deinstallation muss möglich sein: Setup → „Uninstall“, Anleitung im README. Erfüllt.
- Code signing policy auf der Startseite: Link im README auf `docs/CODE-SIGNING-POLICY.md`. Erfüllt.
- **Die Software muss schon in der Form veröffentlicht sein, die signiert werden soll.** Für
  das Setup heißt das: Es braucht ein öffentliches Release mit der setup.exe. Bisher gibt es
  nur Testversionen (0.0.x) ohne Setup. Eine Möglichkeit: v1.0.0 unsigniert veröffentlichen,
  dann bewerben und ab 1.0.1 signieren.
- Alle Beteiligten nutzen **Zwei-Faktor-Anmeldung** bei GitHub und SignPath.
- Jede Signierung wird von Hand freigegeben (Approver).

## Schritte

1. **Zwei-Faktor-Anmeldung bei GitHub** einschalten, falls noch nicht aktiv:
   GitHub → Settings → Password and authentication.
2. **Bewerben** unter <https://signpath.org/apply>. Angaben: Repository
   `https://github.com/ElectroHugin/FM-View-Exporter`, Lizenz GPLv3, Beschreibung („Plugin und
   Setup, das Spielerlisten aus Football Manager 27 als CSV/HTML exportiert“), Link zur Code
   signing policy (`docs/CODE-SIGNING-POLICY.md`). Die Foundation entscheidet selbst und kann ablehnen.
3. **Nach der Zusage** in SignPath.io (mit Zwei-Faktor-Anmeldung):
   - Projekt anlegen, z. B. Slug `fm-view-exporter`, und GitHub.com als vertrauenswürdiges
     Build-System verbinden (Anleitung von SignPath).
   - Artefakt-Konfiguration (die CI lädt ein Zip mit der setup.exe hoch):

     ```xml
     <artifact-configuration xmlns="http://signpath.io/artifact-configuration/v1">
       <zip-file>
         <pe-file path="FMViewExporter-*-setup.exe" product-name="FM View Exporter">
           <authenticode-sign/>
         </pe-file>
       </zip-file>
     </artifact-configuration>
     ```

   - Signierrichtlinie, z. B. `release-signing`, mit ElectroHugin als Approver.
   - Einen CI-Benutzer mit API-Token anlegen, der Anträge stellen darf.
4. **Im GitHub-Repository** (Settings → Secrets and variables → Actions):
   - Secret `SIGNPATH_API_TOKEN` = API-Token des CI-Benutzers
   - Variable `SIGNPATH_ORGANIZATION_ID` = Organisations-ID aus SignPath
   - Variable `SIGNPATH_PROJECT_SLUG` = z. B. `fm-view-exporter`
   - Variable `SIGNPATH_SIGNING_POLICY_SLUG` = z. B. `release-signing`

## Ablauf bei einem Release

1. Tag `v…` pushen oder den Build mit `release_tag` von Hand starten.
2. Die CI baut alles und schickt das Setup an SignPath.
3. In SignPath den Antrag prüfen und **freigeben** (bis zu einer Stunde wartet die CI).
4. Die CI übernimmt die signierte setup.exe, rechnet die Prüfsummen neu und legt das Release an.

Wird nicht freigegeben, schlägt der Build fehl und es entsteht kein Release. Das ist Absicht.
