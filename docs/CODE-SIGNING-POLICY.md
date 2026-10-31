# Code signing policy

Free code signing for the Windows setup is provided by [SignPath.io](https://about.signpath.io),
certificate by [SignPath Foundation](https://signpath.org).

- Status: the project is applying to SignPath. Releases are **not signed** yet; signing
  starts with the first release after SignPath has approved the project.
- Only files built by GitHub Actions from this public repository are signed. Every signing
  request is approved by hand.
- **Committers and reviewers:** [ElectroHugin](https://github.com/ElectroHugin)
- **Approvers:** [ElectroHugin](https://github.com/ElectroHugin)
- Bundled third-party files such as [BepInEx](https://github.com/BepInEx/BepInEx) are included
  as published by their authors and are not signed by this project.

**Privacy.** This program will not transfer any information to other networked systems unless
specifically requested by the user or the person installing or operating it. The plugin and
the setup make no network connections. Exports are written only to your own computer. The mod
loader BepInEx, which the setup installs, downloads Unity base libraries once from
`unity.bepinex.dev` on the first game start; see the
[BepInEx project](https://github.com/BepInEx/BepInEx) for its terms.

Back to the [README](../README.md).
