# TODOs

## Pre-release polish

- **Finalize English workshop description** determine whether we could/should fit/add any text to our workshop description regarding the scaling settings we offer for biotech's sanguophage quests. once I've approved it, we'll move to translation.
- **Workshop descriptions in the nine languages** Owed once the English page is approved; the checker warns on each until then, and `--strict` fails on the warnings.
- **Workshop gallery** Add images, Medic/Overlord as precedent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Quest pilots in game** Generate both sanguophage quests at 100% chance (dev quest generation) and check:
  - the join offer and a capture
  - under Combat Extended, that a dressed pawn's autorifle fires and reloads from the three spare magazines
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README

## Later

- **AI use of the rush and Dismember** A pilot the player does not control casts neither (VEF fires no apparel ability's autocast), so a quest's sanguophage in the set fights with its plating alone.
- **Workshop page and the thralls** The page says nothing of the crashed shuttle's thralls arriving in warcaskets, which is on by default. The author's to word.
- **AI sanguophages and their own abilities** Check whether a quest's sanguophage uses its gene abilities in a fight (piercing spine, longjump, coagulate), in the set and out of it.
- `Docs/Research/WARCASKET_ABILITIES.md` is the Shipcracker repo's copy and still carries that set's scoping notes and design proposal, with pointers to a ledger that is not this one. Cut it down to the VFEP survey?
- The test suite is flaky. The host crashes on a failed load of UnityEngine.CoreModule in roughly one run in ten. Left alone since the test project is the family template's; should probably be chased.
