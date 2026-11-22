# SPEC: A threat-point floor for the shuttle master's set

Status: BUILT as recommended in section 4, with the floor at 2500 and the slider; the code and
the armor def's "Quest pilots" header paragraph supersede this file. Vanilla facts below were
read on RimWorld 1.7.5014 and the frozen 1.6 assembly, the same on both unless a line says
otherwise; each names where it was read so a build session can re-check it instead of trusting
this file.

## 1. Goal

The shuttle master's chance to wear the set (`shipMasterChance`, default 0) should apply only to
a crashed shuttle quest big enough that the set does not turn it into more than the colony's
threat points pay for. The research gate answers "has this colony met warcaskets"; this answers
"is this colony strong enough", in the storyteller's own measure, wealth and head count.

## 2. What the quest spends its points on

`QuestNode_Root_SanguophageShip.RunInt` reads `slate.Get("points", 0f)` and uses it for two
curves only, each rounded with `Mathf.RoundToInt`:

- thralls in the shuttle: (0, 2) to (2000, 5);
- thralls dropped later as reinforcements: (2000, 0), (2500, 2), (5000, 4), (8000, 6).

The points are the incident's (`IncidentWorker_GiveQuest.GiveQuest` passes `parms.points` to
`QuestUtility.GenerateQuestAndMakeAvailable`), which the storyteller fills from
`StorytellerUtility.DefaultThreatPointsNow`, capped at 10000. The quest's own floor is its
`rootMinPoints`, 300.

The master (`Sanguophage`) and every thrall (`SanguophageThrall`) are priced at `combatPower`
100, and a raid spends about its points on combat power, so:

| Quest points | Master + thralls | Combat power | Share of a raid at those points |
|---|---|---|---|
| 300 | 1 + 2 | 300 | 100% |
| 1000 | 1 + 4 | 500 | 50% |
| 2000 | 1 + 5 | 600 | 30% |
| 2500 | 1 + 7 | 800 | 32% |
| 5000 | 1 + 9 | 1000 | 20% |
| 8000 to 10000 | 1 + 11 | 1200 | 15% to 12% |

The quest is a full-strength fight at its floor and a small one late.

## 3. What the set is worth

An estimate, not a measurement. VFEP prices its own warcasket pilots at `combatPower` 300
(`VFEP_Footsoldier`, the basic set), 400 (`VFEP_Veteran`) and 460 to 600 (the heavy
specialists), against 65 to 220 for its kinds without one
(`VFEP/1.6/Defs/PawnKindDefs_Humanlikes/`). Vanilla's meeting quest adds 200 raid points for
each sanguophage it expects to fight beside the colony
(`QuestNode_Root_SanguophageMeetingHost.RunInt`, `num2 + 200f * num3`). A sanguophage in a
spacer-tier set with no pain shock and zero psychic sensitivity sits at the top of VFEP's band:
call it 550, so the set adds about **450** to what vanilla counts.

With that price the quest's share of a raid, master dressed:

| Quest points | Combat power with the set | Share of a raid |
|---|---|---|
| 300 | 750 | 250% |
| 1000 | 950 | 95% |
| 1500 | 950 | 63% |
| 2000 | 1050 | 53% |
| 2500 | 1250 | 50% |
| 3000 | 1250 | 42% |
| 5000 | 1450 | 29% |

## 4. Design

**Recommended: a floor on the quest's points, default 2500.** Below it the master is never
dressed, whatever the chance. 2500 is where the dressed quest comes down to half a raid and
where the quest itself first brings its two reinforcements, so "a shuttle big enough to bring
reinforcements" is what the number means in play.

- **Def field.** `shipMasterMinPoints` on `QuestPilotExtension`, 2500 in the armor def, with a
  header bullet carrying the table's reasoning. 0 is no floor.
- **The rule stays pure.** `ChanceFor(source)` becomes `ChanceFor(source, points)` and answers 0
  for `ShipMaster` under the floor; the meeting's two sources ignore the points.
  `QuestPilot.TryDress` takes the points and passes them on.
- **The patch reads the points.** `Patches/QuestNode_Root_SanguophageShip_RunInt` passes
  `QuestGen.slate.Get("points", 0f)`, the same read the root makes. The meeting patch passes its
  own and nothing reads them. A quest generated with no points in its slate (a debug action that
  sets none) reads 0 and is under any floor, which is the safe side.
- **Both gates must pass.** The research gate first, then the floor, then the roll. Order only
  matters for Rand: nothing is drawn unless both pass.
- **Settings.** One more `Tunable` in `Settings_QuestPilot.cs` after the shuttle master's
  chance: 0 to 10000 in stored points, banded (250 up to 3000, 500 up to 5000, 1000 beyond), 0
  shown as off. Threat points are a number the game never shows the player outside dev mode, so
  the tooltip has to say what the number means in play. Draft, to be read against
  `l10n/languages/English.md` when built:
  - label: "Shuttle master threat points"
  - tooltip: "The threat points a crashed shuttle quest must be worth before its sanguophage
    master can be in a hemophage warcasket. The game raises threat points with your colony's
    wealth and number of colonists. Default: {0}."
- **The default chance stays 0.** The floor fixes the budget, not the capture: a master in the
  set still has no pain shock, no psychic opening and the capacity floors, at any points.

Files: `Quests/QuestPilotExtension.cs`, `Quests/QuestPilot.cs`, the two patches,
`Settings/Settings_QuestPilot.cs`, the armor def and its header, the Keyed file,
`Tests/1.6/QuestPilotTests.cs` (the `ChanceFor` theory gains points either side of the floor;
the floor parses within 0 to 10000), `Tests/1.6/SettingsTargetTests.cs` (the new field), the
README's settings line if it should name it. About fifty lines with the tests.

## 5. Looked at and left

- **Trading thralls for the set**, lowering the slate's points before `RunInt` so the curves
  field fewer thralls. The curves are too flat for it: 450 points off removes less than one
  thrall anywhere on them and nothing above 8450, while paying for the set in thralls would
  take four or five of them, the whole escort. It would also need the roll made in a prefix and
  carried to the postfix.
- **The meeting quest.** Its raid, when it draws one, is generated inside `RunInt` before the
  guests are dressed, so pricing dressed guests into it needs the rolls made ahead of the root.
  The guests are neutral: stronger ones make that raid easier and a betrayal harder, the second
  by the player's own choice.
- **A floor on the meeting's chances.** The same reasoning: neutral guests cost the colony
  nothing they did not choose.
- **Colony wealth after a join.** A set that joins with its pilot counts toward wealth as a
  colonist's worn apparel does (the Overlord armor def's header has the same for its parts), so
  later threat points already follow it.

## 6. Decisions for the author

1. The default floor: 2500 as recommended, or 2000 (53% of a raid) or 3000 (42%).
2. Slider or def field only. Recommended the slider: anyone who turns the master's chance up
   will want to see what else holds it back.
3. Whether the price estimate of 450 should be checked in play before the default is fixed
   (a dev-mode shuttle quest at 2500 points with the chance at 100%).
