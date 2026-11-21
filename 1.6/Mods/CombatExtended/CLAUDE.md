# 1.6/Mods/CombatExtended/: the Combat Extended root

Loaded when work touches this root. Gated on `CETeam.CombatExtended` in `LoadFolders.xml`, never
shipped beyond its `Patches/` (the deploy manifest takes XML only). Each patch file's header
carries its numbers and the CE file each one mirrors; this file maps what CE takes over from
vanilla in the set's domain to where the answer lives, and records what was looked at and left.

Scanned against CE 16.7.4 (Workshop 2890901044, `Assemblies/CombatExtended.dll` and its XML).
CE ships no source: `ilspycmd -p -o <scratch dir> CombatExtended.dll` gives a tree to grep, and
`grep -rhoE 'HarmonyPatch\(typeof\([A-Za-z_.]+\)(, *"[A-Za-z_.]+")?'` over it lists every method
CE patches, the quickest check after a CE update or before patching a new vanilla method. Its
XML is three trees: `Patches/Core/`, a folder per DLC (`Biotech/Patches/`) and a folder per mod
(`ModPatches/Vanilla Factions Expanded - Pirates/`, `ModPatches/Vanilla Races Expanded -
Sanguophage/`).

## What CE takes over, and the answer

- **Armor, bulk, carry, shield, smoke, costs**: `Patches/ApparelStats.xml`. CE reaches only the
  defNames it patches, so every new stat on a piece needs its CE value there.
- **Part health and how much damage a pawn takes**: CE keeps vanilla's scale of damage against
  part health but moves the numbers under it (a leg's hit points, a centipede's rings, Tough).
  `Patches/Dismember.xml` moves the strike's damage with them. A number in the main tree that
  is argued from a part's hit points needs its CE value in that file.
- **Melee aim height**: a comp CE adds to every humanlike, set by the player from a gizmo.
  `Patches/Dismember.xml` names the comp and its field for the strike to follow; the code that
  reads it (`Source/1.6/Abilities/Dismember/MeleeAim.cs`) names nothing of CE's. A renamed class
  or field is one warning in the log and a random limb, so check both names after a CE update.

## Looked at and left alone

- **The strike's damage path.** CE's armor replaces vanilla's inside the branch a hit that
  ignores armor skips, and returns at once for a damage def without an `armorCategory`; its
  other patches on `DamageWorker_AddInjury` are on the propagation the strike turns off (the
  header of `Ability_Dismember.Strike.cs`). `DamageDefs/Dismember.xml` must keep no
  `armorCategory`.
- **The reload.** CE prefixes `JobGiver_Reload.TryGiveJob`: a pilot carrying the pool's
  components has as many as the pads lack dropped at their feet and loaded from there, and
  otherwise vanilla fetches from the map as without CE. CE's "Reload worn armor" button on every
  worn reloadable runs the same prefix. Both read the pool through vanilla's own
  `CompApparelReloadable` members, so the strike count and the cost per strike hold. They ask
  `MaxAmmoNeeded` for the count to drop, which is negative for a pool over its capacity, so
  `CompStrikePool`'s trims are what keep a lowered capacity from throwing under CE too.
- **Blood loss and bleeding.** CE rewrites the `BloodLoss` stages (severe, from 0.45, is life
  threatening), gives injuries and missing parts a stabilize comp that scales their bleeding
  down, at best to nothing, and counts a missing part fresh by a rule of its own. The drain
  adds blood loss by severity and asks only whether the target bleeds at all, so a target
  stabilized that far is not drained, as one whose wounds are tended is not without CE. The
  drain's default blood loss stays a hair under CE's severe stage, as under vanilla's.
- **Downed targets do not die of being downed.** CE turns off the roll that kills a pawn as it
  goes down (`Pawn_HealthTracker.CheckForStateChange`), so more of the pilot's targets are
  still alive to drain under CE.
- **Suppression interrupts any job**, an ordered Dismember included, with a run for cover or a
  hunker. The armor's CE row makes its pilot immune, as CE does every spacer warcasket's.
- **The hemogen rush.** CE patches neither `MeleeDamageFactor` nor a hediff's stat factors. Its
  move speed worker multiplies the finished stat by the pilot's carried weight, after the rush's
  factor, and its melee verb takes its damage through vanilla's
  `VerbProperties.AdjustedMeleeDamageAmount`, the rush's factor included.
- **Untouched by CE**: hemogen, the hemogenic and bloodfeeder genes, Bloodfeed, deathrest,
  the sunlight and pyrophobia genes, mental breaks through `MentalBreakWorker`, VEF abilities
  and their apparel grant, VEF's shield bubble, VFEP's warcasket trait and abilities, and every
  def the Vanilla Races Expanded - Sanguophage root names. CE's Biotech folder moves a few
  genes' numbers (melee damage, fire damage, piercing spine); its folder for that mod patches a
  hediff, a tool, a single-use ability and the hemogen packs' bulk.
- **CE swaps `GenRadial`'s pattern for one of 119 cells**; the strike's spatter asks for a
  radius of 2.
- **Generated pawns.** CE's apparel autopatcher is off by default and matches no warcasket,
  and nothing in its assembly names VFEP, whose slot fill is the one path `ListMask` answers.
