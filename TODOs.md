# TODOs

## Pre-release polish

- **Translation guidance** The `/translate` skill and `CONTRIBUTING.md`'s translation section predate the Keyed file, the settings window and every def beyond the three parts, and say none exists. Bring both up to what is built before the first translation pass.
- **Full review of player-facing English text** Final read pass against `l10n/languages/English.md`. Workshop description and About.xml are already green.
- **Chinese localizations** How does hemogen sit in vanilla?
- **Workshop gallery** Add images, Medic/Overlord as precedent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Quest pilots in game** Generate both sanguophage quests at 100% chance (dev quest generation) and check:
  - the set on arrival, each piece in the kind's dark grey or dark red on its own draw, and how the two read side by side on the masked art and under the rush's glow
  - the join offer and a capture
  - the autorifle or broadsword in hand: by vanilla's weapon kind on a thrall, by the higher of Shooting and Melee on the master and a meeting's sanguophages, broadsword on a tie
  - under Combat Extended, that a dressed pawn's autorifle fires and reloads from the three spare magazines
- **Initial translation pass** of all 9 planned languages
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README

## Later

- **AI use of the rush and Dismember** A pilot the player does not control casts neither (VEF fires no apparel ability's autocast), so a quest's sanguophage in the set fights with its plating alone.
- **Workshop page and the thralls** The page says nothing of the crashed shuttle's thralls arriving in warcaskets, which is on by default. The author's to word.
- **AI sanguophages and their own abilities** Check whether a quest's sanguophage uses its gene abilities in a fight (piercing spine, longjump, coagulate), in the set and out of it.
- `Docs/Research/WARCASKET_ABILITIES.md` is the Shipcracker repo's copy and still carries that set's scoping notes and design proposal, with pointers to a ledger that is not this one. Cut it down to the VFEP survey?
- The test suite is flaky. The host crashes on a failed load of UnityEngine.CoreModule in roughly one run in ten. Left alone since the test project is the family template's; should probably be chased.
