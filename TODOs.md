# TODOs

## Pre-release polish

- **Translation guidance** The `/translate` skill and `CONTRIBUTING.md`'s translation section predate the Keyed file, the settings window and every def beyond the three parts, and say none exists. Bring both up to what is built before the first translation pass.
- **Full review of player-facing English text** Final read pass against `l10n/languages/English.md`. Workshop description and About.xml are already green.
- **Chinese localizations** How does hemogen sit in vanilla?
- **Workshop gallery** Add images, Medic/Overlord as precedent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Quest pilots in game** Never run: generate both sanguophage quests with the chances at 100% (dev quest generation), check the set on arrival, each of its three pieces in the sanguophage kind's dark grey or dark red on a draw of its own and how the two read side by side on the masked art and under the rush's glow, the join offer and a capture, and the quest section of the settings window: the research slider's row, with its value column as wide as the widest name it can show, and the two quests' subheadings, their tooltips and their indented rows, whose sliders should start and end with every other row's. With them the shuttle master's points floor: a shuttle quest generated under 2500 points and one at 2500, and whether the dressed quest at 2500 fights like the half raid its estimate of 450 combat power for the set says. And the shuttle's thralls: quests at 2500, 5000 and 10000 points with the Marine research done and without, for the count dressed against the armor def's header, the reinforcements dressed first and arriving by drop pod in the set, the autorifle or broadsword in hand, on a thrall by the kind of weapon vanilla gave and on the master and a meeting's sanguophages by the higher of Shooting and Melee, the broadsword on a tie, the letter's and quest text's thrall counts unchanged, and whether the prices of 200 and 250 make the quest at 10000 fight like a quarter of a raid. Under Combat Extended, that a dressed pawn's autorifle fires and reloads from the three spare magazines.
- **Initial translation pass** of all 9 planned languages
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README

## Later

- **AI use of the rush and Dismember** A pilot the player does not control casts neither (VEF fires no apparel ability's autocast), so a quest's sanguophage in the set fights with its plating alone.
- **Workshop page and the thralls** The page says nothing of the crashed shuttle's thralls arriving in warcaskets, which is on by default. The author's to word.
- **AI sanguophages and their own abilities** Check whether a quest's sanguophage uses its gene abilities in a fight (piercing spine, longjump, coagulate), in the set and out of it.
- `Docs/Research/WARCASKET_ABILITIES.md` is the Shipcracker repo's copy and still carries that set's scoping notes and design proposal, with pointers to a ledger that is not this one. Cut it down to the VFEP survey?
- The test suite is flaky. The host crashes on a failed load of UnityEngine.CoreModule in roughly one run in ten. Left alone since the test project is the family template's; should probably be chased.
