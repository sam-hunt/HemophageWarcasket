# TODOs

## Pre-release polish

- **Workshop gallery** Add images, Medic/Overlord as precedent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Quest pilots in game** Generate both sanguophage quests at 100% chance (dev quest generation) and check:
  - the join offer and a capture
  - under Combat Extended, that a dressed pawn's autorifle fires and reloads from the three spare magazines
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README

## Later

- **AI use of the rush and Dismember** A pilot the player does not control casts neither (VEF fires no apparel ability's autocast), so a quest's sanguophage in the set fights with its plating alone.
- **AI sanguophages and their own abilities** Check whether a quest's sanguophage uses its gene abilities in a fight (piercing spine, longjump, coagulate), in the set and out of it.
- **Upstream l10n rows the Workshop pass found missing** `l10n/languages/<Language>.md` has no family row for VFEP's marine warcasket or for "lesser warcaskets", so each page says only "lesser warcaskets"; RimWorldKorea's pack also covers Vanilla Races Expanded - Sanguophage, which the Korean file does not yet name as a source. The siblings' pages split two and two on the FAQ's "weld it on" answer in several languages. Settle in `~/dev/rimworld-l10n`, then let each set follow.
- `Docs/Research/WARCASKET_ABILITIES.md` is the Shipcracker repo's copy and still carries that set's scoping notes and design proposal, with pointers to a ledger that is not this one. Cut it down to the VFEP survey?
- The test suite is flaky. The host crashes on a failed load of UnityEngine.CoreModule in roughly one run in ten. Left alone since the test project is the family template's; should probably be chased.
