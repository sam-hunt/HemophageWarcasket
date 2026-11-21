# SPEC: Thralls in lesser warcaskets

Status: PROPOSED, not built. A companion to the quest pilots (`Source/1.6/QuestPilot/`) and to
`Shuttle-Master-Points-Floor.md`, whose section 2 has the crashed shuttle quest's curves and
section 3 the price of a suited pawn; neither is restated here. Build the floor first: this spec
reuses its `points` plumbing and its price for the master. Vanilla facts were read on RimWorld
1.7.5014 and the frozen 1.6 assembly, VFEP on v4.03, VEF on the local checkout; each names where
it was read.

## 1. Goal

The crashed shuttle quest stops growing at 8000 threat points (one master, eleven thralls,
1200 combat power) and is under a third of a raid from 2000 up. Late in a game it is a formality.
Let a share of the gap be spent on dressing some of the thralls in VFEP's own lesser warcaskets,
so the quest keeps pace with the colony without changing vanilla's head count. Off by default.

## 2. Facts

- **The thralls.** `SanguophageThrall` (Biotech `PawnKinds_Special.xml`): `combatPower` 100,
  industrial and military apparel for 1250 to 2500 silver, `IndustrialGunAdvanced` or
  `MedievalMeleeDecent` weapons, `initialResistanceRange` 0~1, no xenotype set, so baseliners
  and not hemogenic. The hemophage set gives them nothing and is not offered.
- **Where they are after `RunInt`.** The shuttle's thralls are in the slate's `"shuttlePawns"`
  beside the master (`"sanguophage"`). The reinforcements are in no slate name: `RunInt` holds
  them in a local and hands them to `quest.DropPods`, which stores them in a
  `QuestPart_DropPods` (private `pawns`, public `Things`). A postfix reaches them through
  `QuestGen.quest.PartsListForReading`. That part exists by the time a postfix runs:
  `quest.Delay` runs its inner action during generation (`QuestGenUtility.RunInner`).
  Reinforcement weapons are biocoded (`quest.BiocodeWeapons`), which dressing does not touch.
- **An ordinary weapon works in a warcasket.** The restriction runs one way: VEF refuses a
  `HeavyWeapon` to a pawn without a warcasket
  (`VEF/Weapons/Harmony/EquipmentUtility_CanEquip_Patch.cs`), and nothing refuses a rifle to a
  pawn in one. A dressed thrall keeps the weapon vanilla gave it.
- **The two plain sets.** `VFEP_Warcasket_Warcasket` (research `VFEP_Warcaskets`) and
  `VFEP_Warcasket_Marine` (`VFEP_AdvancedWarcaskets`), each with `VFEP_WarcasketShoulders_<Set>`
  and `VFEP_WarcasketHelmet_<Set>`. Neither armor declares a comp or an ability, so nothing is
  lost on a pawn the AI drives; `Docs/Research/VFEP_WARCASKET_STATS.md` has their numbers.
- **VFEP's prices.** `VFEP_Footsoldier`, whose `apparelRequired` is the basic set, is
  `combatPower` 300; `VFEP_Veteran`, on the `WarcasketVeteran` tag the Marine set carries, 400.
  Both carry warcasket weapons, which a thrall will not, so against a thrall's 100 the upgrades
  cost at most **200** (basic) and **300** (Marine).
- **Any warcasket grants `VFEP_WarcasketTrait`**, so a dressed thrall feels no pain and cannot
  be downed by it. VFEP's own raiders are the same.
- **Dressing after generation is safe for VFEP's sets too.** `QuestPilot.TryDress`'s steps (fit
  check, pieces made first, strip, wear locked) name nothing of ours, and VFEP's slot fill has
  already run and found no warcasket.

## 3. Design

**Recommended: a budget, a share of the quest's points, spent on upgrades.**

```
budget = share * points
       - 100 * (1 + thralls + reinforcements)      what vanilla already fields
       - masterPrice, if the master was dressed    450, the floor spec's estimate
```

Then, while a tier is affordable, dress one more thrall in the dearest tier the budget still
covers, the shuttle's thralls first (the master's guard, on the map from the start), each
picked at random among those left. A thrall who cannot wear a tier's whole set keeps their
clothes and costs nothing. Deterministic but for the order: no per-thrall dice, so the result is
the same strength every time for the same quest.

At a share of one third, with the master dressed:

| Quest points | Budget after vanilla and the master | Thralls dressed |
|---|---|---|
| 3000 | under 0 | none |
| 5000 | 217 | 1 basic |
| 8000 | 1017 | 3 Marine |
| 10000 | 1683 | 5 Marine |

and with the master in vanilla's clothes: 1 basic at 3000, 2 Marine at 5000, 4 Marine and 1
basic at 8000, 7 Marine at 10000, out of eleven thralls.

- **Tiers are data.** A list on `QuestPilotExtension`, each entry the three defs of one VFEP set
  and its price, cheapest first: the basic set at 200 and Marine at 300. VFEP is a hard
  dependency, so its defs are named in the main tree's XML and nowhere in the C#. Adding a
  third tier is an XML edit.
- **Each tier is gated by its own set's research**, read off the armor def's
  `researchPrerequisites`, so the colony meets only warcaskets it could build, with no gate
  field of its own. The quest pilots' `requiredResearch` is not consulted: that gate is about
  the hemophage set.
- **One setting**, the share, as a percent slider in the quest section, 0 to 100 in steps of 5,
  default **0** (off). A def field `thrallBudgetShare`; the price of the master's set a second
  def field (`shipMasterPrice`), not a setting.
- **The rule is pure and tested.** A static function from (points, pawn counts, master dressed,
  share, tiers available) to a list of tier picks, with the table above as its theory rows.
- **The patch grows by one call.** After the master's `TryDress`, the ship postfix gathers the
  thralls (slate, then the drop pod part), asks the rule and dresses each pick through the same
  steps the master takes. `TryDress` splits into the hemophage-specific checks (gene, chance,
  research) and a shared `Dress(pawn, set)`.

Files: `QuestPilot/` (extension fields, the budget rule, the shared dress), the ship patch,
`Settings/Settings_QuestPilot.cs`, the armor def and header, the Keyed file,
`Tests/1.6/QuestPilotTests.cs` and `SettingsTargetTests.cs`, the guides' one-line mentions.
Roughly the size of the quest pilots themselves.

## 4. Looked at and left

- **A chance per thrall on a curve of points.** Simpler, but the quest's strength then varies
  by the dice, and nothing ties it to whether the master is dressed.
- **The specialised and spacer sets.** Their modules are abilities the AI does not cast, and a
  spacer thrall beside a hemophage master blurs who the master is.
- **Warcasket weapons for dressed thralls.** VFEP's prices assume them; without them the
  upgrade is cheaper than priced, which errs on the easy side.
- **More thralls instead.** Raising the head count means rewriting vanilla's curves or adding
  pawns to a quest already generated, where the shuttle, the letter and the lord all hold the
  list.

## 5. Decisions for the author

1. **Does this belong in this mod?** It dresses pawns who are not hemogenic in sets that are
   not ours. It is the same quest and the same patch, and off by default, so recommended yes;
   the alternative is to leave the shuttle quest as vanilla scales it.
2. **The share's ceiling in play.** One third keeps the quest a capture opportunity. Half makes
   it a real raid at high points (all eleven thralls in Marine sets at 10000). The slider
   allows either; the question is only what the tooltip recommends.
3. **Cheap suited recruits.** A thrall's resistance is 0 to 1, rolled from the pawn kind's
   `initialResistanceRange` (`Pawn_GuestTracker`), where VFEP's own warcasket raiders roll 7 to
   12. A dressed
   thrall taken alive is a nearly free warcasket pilot. Recommended: accept it, since a pawn
   with no pain shock is rarely taken alive; raising it would take a patch on capture.
4. **Whether the meeting's guests' escorts matter.** The meeting has no thralls, so this spec
   touches the shuttle quest only.

## 6. To check in game when built

- A dressed thrall leaves the shuttle, defends and assaults as an undressed one does (the
  lords hold pawns, not gear).
- Reinforcements dressed while still in the quest arrive by drop pod wearing the set.
- The letter's and the quest text's thrall counts are unchanged.
