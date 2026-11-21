# TODOs

## Pre-release polish

- **Translation guidance** The `/translate` skill and `CONTRIBUTING.md`'s translation section predate the Keyed file, the settings window and every def beyond the three parts, and say none exists. Bring both up to what is built before the first translation pass.
- **Full review of player-facing English text** Final read pass against `l10n/languages/English.md`. Workshop description and About.xml are already green.
- **Chinese localizations** How does hemogen sit in vanilla?
- **Workshop gallery** Add images, Medic/Overlord as precendent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Quest pilots in game** Never run: generate both sanguophage quests with the chances at 100% (dev quest generation), check the set on arrival, the join offer and a capture, and the research slider's row in the settings window. With them the shuttle master's points floor: a shuttle quest generated under 2500 points and one at 2500, and whether the dressed quest at 2500 fights like the half raid its estimate of 450 combat power for the set says.
- **Initial translation pass** of all 9 planned languages
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README

## Later

- **AI use of the rush and Dismember** A pilot the player does not control casts neither (VEF fires no apparel ability's autocast), so a quest's sanguophage in the set fights with its plating alone.
- **Thralls in lesser warcaskets** `Docs/Specs/Thrall-Warcaskets.md`, proposed, with four decisions open in its section 5. Its budget subtracts the floor spec's 450 for a dressed master, so settle that number in play first.
- **AI sanguophages and their own abilities** Check whether a quest's sanguophage uses its gene abilities in a fight (piercing spine, longjump, coagulate), in the set and out of it.
- `Docs/Research/WARCASKET_ABILITIES.md` is the Shipcracker repo's copy and still carries that set's scoping notes and design proposal, with pointers to a ledger that is not this one. Cut it down to the VFEP survey?
- The test suite is flaky. The host crashes on a failed load of UnityEngine.CoreModule in roughly one run in ten. Left alone since the test project is the family template's; should probably be chased.
