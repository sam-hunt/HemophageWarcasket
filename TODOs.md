# TODOs

## Pre-release polish

- **Full review of player-facing English text**
  - The three descriptions, `About.xml`, the README's features and the Workshop page are brought up to what is built. Final tweaks by hand, then lock for translation. No release, candidates included, before that.
  - Not yet read against `English.md`: the settings window's strings and the two abilities' and the hediff's descriptions.
- **Chinese localizations** How does hemogen sit in vanilla?
- **Workshop gallery** Add images, Medic/Overlord as precendent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Translation guidance** The `/translate` skill and `CONTRIBUTING.md`'s translation section predate the Keyed file, the settings window and every def beyond the three parts, and say none exists. Bring both up to what is built before the first translation pass.
- **Initial translation pass** of all 9 planned languages
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README

## Later

- `Docs/Research/WARCASKET_ABILITIES.md` is the Shipcracker repo's copy and still carries that set's scoping notes and design proposal, with pointers to a ledger that is not this one. Cut it down to the VFEP survey?
- The test suite is flaky. The host crashes on a failed load of UnityEngine.CoreModule in roughly one run in ten. Left alone since the test project is the family template's; should probably be chased.
