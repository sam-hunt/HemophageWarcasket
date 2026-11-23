# SPEC: The hemogen rush and Dismember for a pilot the player does not control

Status: PROPOSED, not built. Section 3 is written on the recommended answer to each decision in
section 6; each decision says what changes on another answer. Every engine fact below was read,
not run: VEF at checkout `fa069a72` (`../VanillaExpanded/VanillaExpandedFramework/Source/VEF/Abilities/`,
paths below relative to it), VFEP at `be4040e`, vanilla by decompile of 1.7.5014 and the frozen
1.6 assembly, the same on both for every method named. Each fact names where it was read so the
build session can re-check it instead of trusting this file.

## 1. Goal

A sanguophage who arrives in the set should fight as the set's pilot does: start the rush as it
closes to melee, and tear a limb off what it fights with the strikes loaded in the pads. Today
it casts neither and fights with its plating alone. The pilots this reaches are the quests'
sanguophages (`Quests/QuestPilot.cs`) and any pawn another mod dresses in the set.

## 2. What the engine does today

**VEF never offers an apparel ability to the AI.**

- VEF's one AI hook is a postfix on `Pawn.TryGetAttackVerb` that iterates
  `compAbilities.LearnedAbilities` (`Harmony/Pawn_TryGetAttackVerb.cs:19-41`). Apparel abilities
  are in `CompAbilitiesApparel.GivenAbilities`, which nothing outside that comp reads.
- That hook never self-casts: it needs an enemy the ability can hit, so a toggle is out of its
  reach even when learned.
- An apparel ability's `holder` is set when the comp initializes; its `pawn` is set only in
  `CompGetWornGizmosExtra`, on a gizmo draw (`Comps/CompAbilitiesApparel.cs:24-58`). On an NPC
  nobody has selected, `pawn` is null.
- VFEP changes nothing in a warcasket pilot's combat AI (its source has no think node, job giver
  or `TryGetAttackVerb` patch for pilots).

**VEF does tick an apparel ability, which is the way in.**

- `AbilityDef.needsTickingInterval` puts the ability in the comp's interval list
  (`Defs/AbilityDef.cs:14-15`, with a config error at `:130` for a class that declares
  `TickInterval` without the field), and `CompAbilitiesApparel.CompTickInterval` calls
  `Ability.TickInterval(int delta)`, a virtual with an empty body
  (`Comps/CompAbilitiesApparel.cs:96-101`, `Misc/Ability.cs:231`).
- A worn piece ticks through its wearer: `Thing.DoTick` ticks a `tickerType` Normal thing, then
  the directly held things of every holder under it, and `ThingWithComps.TickInterval` calls
  each comp's `CompTickInterval`. A piece whose def is `tickerType` Never gets nothing.
- VFEP's armor base is Normal and its part base is Never
  (`1.6/Defs/ThingDefs_Misc/Apparel_Various.xml:70` and `:29`), so our armor ticks and our pads
  do not. VFEP's own Barrage pads set Normal on the def (`:828`).

**How vanilla's fight job giver behaves, which the rules below read.**

- `JobGiver_AIFightEnemy.TryGiveJob` sets `pawn.mindState.enemyTarget` (`UpdateEnemyTarget`),
  tries the pawn's vanilla abilities for a non-colonist (`GetAbilityJob`), then gives a melee
  pawn an `AttackMelee` job on that target at any distance, with an expiry of 360 to 480 ticks
  that re-asks the think tree (`MeleeAttackJob`). An ability job expires after 30.
- `enemyTarget` is written only while that giver runs, so it goes stale once the pawn's duty
  stops reaching the giver. `JobGiver_ReactToCloseMeleeThreat` gives `AttackMelee` too, without
  touching it.
- `Pawn_JobTracker.StartJob` records the node that issued a job in `Job.jobGiver`.
- The duties the quests' sanguophages fight under all reach that giver: `Defend` and
  `AssaultColony` for the crashed shuttle, `ExitMapBestAndDefendSelf` for a betrayed meeting and
  `AssaultThing` for a meeting that turns on raiders.

**What a sanguophage NPC already does with its hemogen.**

- It arrives with a full tank: `Gene_Resource.Reset` sets the value to the maximum.
- It casts piercing spine on its own (`aiCanUse`, range 3.9, 20 hemogen, 60 tick cooldown), and
  the giver prefers it to the weapon. In melee that is about one spine each time the melee job
  expires, so roughly 20 hemogen every 7 to 9 seconds, about the rush's own drain.

**What VEF's cast job does that matters here.**

- `Ability.Cast` sets `cooldown` to now plus `GetCooldownForPawn()`, a virtual, and
  `IsEnabledForPawn` refuses while it runs; the field is saved (`Misc/Ability.cs:134`, `:537`,
  `:67`, `:766`).
- `JobDriver_CastAbilityOnce.MakeNewToils` sets `job.playerForced` for every cast, whoever the
  caster (`AI/JobDrivers/JobDriver_CastAbilityOnce.cs`).

## 3. Design

No Harmony patch. Each ability decides for its own wearer on its interval tick, through two
small static classes, and everything it then does is the path a player's click already takes.

### 3.1 Who is an AI pilot, and what it is fighting

`Abilities/AiPilot.cs`, the readers that turn a pawn into the rules' arguments:

- **`Is(Pawn)`**: spawned, not downed, awake, and `pawn.Faction != Faction.OfPlayer`. A
  player's pilot stays the player's to command, in a mental state too.
- **`FightTarget(Pawn)`**: the target of the current job when its def is `AttackMelee` or
  `HPWC_Dismember`; else `mindState.enemyTarget` when the current job's `jobGiver` is a
  `JobGiver_AIFightEnemy`; else null. The giver test is what keeps a stale `enemyTarget` out.
- **`InMeleeJob(Pawn)`**: the current job's def is `AttackMelee` or `HPWC_Dismember`.

The Dismember job def shares its defName with the damage def, which `HPWC_DefOf` already holds
under that name, so these readers need a handle of their own on the job def.

Each ability finds its wearer as `(holder as Apparel)?.Wearer`, never through `pawn`, and asks
at most once every 30 ticks (`wearer.IsHashIntervalTick(30, delta)`).

`Abilities/PilotTactics.cs` holds the decisions as plain arithmetic on numbers, booleans and our
own enums, naming no game type, as `Quests/ThrallBudget.cs` does, so its tests run anywhere.

### 3.2 The rush

**Engaged** (`PilotTactics.RushEngaged`): the pilot has a fight target, and either it is in a
melee job with the target within `aiStartDistance`, or the target is within `aiCloseDistance`
whatever the job.

**Start**, in `Ability_HemogenRush.TickInterval`: the wearer is an AI pilot, the comp's `aiUses`
is on, there is no rush hediff, the hemogen gene is active with `CanOffset` and
`Rush.Affords(value)`, and the pilot is engaged. It adds `HPWC_HemogenRush`, as the toggle does.

**Stop**, in `HediffComp_HemogenRush.CompPostTickInterval`: for an AI pilot, ticks not engaged
are counted and the rush ends at `aiStopAfterTicks`; an engaged tick resets the count. The count
is not saved, so a load restarts the grace. Every existing end still applies.

The melee clause is what makes a melee pilot sprint the last stretch. The close clause covers a
pilot with a gun that something has reached, and keeps the rush on between melee jobs while the
pilot casts a spine.

### 3.3 Dismember

A fifth file of the partial class, `Abilities/Dismember/Ability_Dismember.Ai.cs`, with the
`TickInterval` override. It starts the job when all of these hold, cheapest first:

1. The wearer is an AI pilot, the extension's `aiUses` is on, and its current job is not
   already `HPWC_Dismember`.
2. The fight target is a pawn, hostile to the pilot, not downed, within `aiStrikeDistance`, and
   reachable (`CanReach`, `PathEndMode.Touch`).
3. The ability is bound to the wearer (`pawn = wearer` and `Init()` when it differs), then
   `IsEnabledForPawn` and `ValidateTarget(target, showMessages: false)` pass: off cooldown, a
   strike loaded, capable of violence, a limb to take.
4. The foresight says a limb can come off (`StrikeOutlook.Tears` or `MayTear`), and the strike
   would not take the head (`PilotTactics.WorthStriking`).

It then calls `wearer.jobs.StartJob(JobMaker.MakeJob(def.jobDef, target),
JobCondition.InterruptForced)`. `JobDriver_Dismember` binds the comp when the job starts, as it
does for an order.

Around the job:

- **Spacing.** `GetCooldownForPawn()` answers `aiStrikeSpacingTicks` for an AI pilot and the
  base's for anyone else, so VEF's own cooldown spaces the strikes and saves across a load.
- **Limit.** The pads' loaded strikes. An NPC carries no components and never reloads.
- **The walk gives up.** For an AI pilot, a target farther than twice `aiStrikeDistance` ends
  the job in the walk toil, with nothing spent.
- **The drain spares a life.** For an AI pilot, the drain toil is skipped when the target's
  blood loss plus a full drain's would kill (`DismemberDrain.Outlook` answers `Kills`), asked
  once as the drain would start.
- **Unchanged.** The strike's damage, its combat log line, the stun, the faction consequences
  and the wider drain sources are the cast's own and need nothing. `MeleeAim` already reads
  another mod's aim height for a player's pilot only.

### 3.4 Def fields and settings

| Where | Field | Value | Setting |
|---|---|---|---|
| `HediffCompProperties_HemogenRush` | `aiUses` | true | switch |
| | `aiStartDistance` | 25 | |
| | `aiCloseDistance` | 5 | |
| | `aiStopAfterTicks` | 300 | |
| `DismemberExtension` | `aiUses` | true | switch |
| | `aiStrikeDistance` | 5 | |
| | `aiStrikeSpacingTicks` | 600 | |

- `HPWC_HemogenRush` and `HPWC_Dismember` (the ability defs) gain `needsTickingInterval`.
- `WarcasketShoulders_Hemophage.xml` gains `tickerType` Normal.
- Each switch is a `ToggleSetting` in its section's partial (`rushAiUse`, `dismemberAiUse`),
  written onto `aiUses`. Draft strings, to be held to `l10n/languages/English.md` before they
  go in:
  - Label for both: "Used by pilots you do not control".
  - Rush: "When enabled, a pilot you do not control will start a hemogen rush when closing in
    to melee. When disabled, only your own pilots will use it. Default: {0}."
  - Dismember: "When enabled, a pilot you do not control will dismember enemies within reach,
    with the strikes loaded in the shoulder pads. When disabled, only your own pilots will use
    it. Default: {0}."
- The numbers' rationale goes in the two def headers. 25 cells is about a rifle's reach, so a
  melee pilot crosses the colony's firing line in the rush for about 12 hemogen (5 to start,
  then 2 a second for 3.5 seconds at 7.2 cells a second). 5 cells is one more than the spine's
  reach. 300 ticks of grace costs 10 hemogen, twice a restart. 600 ticks between strikes is
  time for the player to pull a colonist back.

### 3.5 Tests

- **`PilotTacticsTests`** (new, no game type): `RushEngaged` across the two clauses and their
  boundaries, `WorthStriking` across the four outlooks with and without the head.
- **`SettingsTargetTests`**: `aiUses` on the rush comp and on the extension.
- `CompatPatchTests` already holds a patch on the extension to the fields the C# declares.

### 3.6 To check in game

Dev mode, a crashed shuttle quest generated at 2500 points or more with the master's chance set
to 1 in the settings, and a meeting betrayed with a dressed leader.

1. The armor's `TickInterval` runs on a pilot nobody has selected, and the pads' does once
   their def ticks.
2. A melee master starts the rush about 25 cells out and the glow shows on every piece.
3. The rush ends about five seconds after the fight does, and when the tank runs dry.
4. The master tears a limb off a colonist it reaches, drains, and waits ten seconds before the
   second strike; the pads then read 0 / 2 and it strikes no more.
5. A target the drain would kill is torn and not drained; a limbless target is left alone.
6. `playerForced` on the Dismember job: the pilot still follows its lord when the duty changes
   mid-job (the shuttle's switch from `Defend` to `AssaultColony`, a raid's retreat).
7. Save and load mid-rush and mid-Dismember.
8. A captured, dressed pilot in a prison break: it is an AI pilot and should behave as one.
9. With Combat Extended active, the same pass. Its guide (`1.6/Mods/CombatExtended/CLAUDE.md`)
   has what CE changes around the strike.

## 4. The fight this should make

An estimate from the numbers, to be measured in step 2 to 4 above. A melee master with a full
tank spends about 12 hemogen on the sprint, tears a limb on contact and takes back up to 25,
then loses about 2 a second to the rush and about 20 every 7 to 9 seconds to its own spines.
That is a rush of roughly 20 to 30 seconds, one more tear ten seconds in, and a pilot who ends
the fight dry.

## 5. Looked at and left

- **A postfix on `JobGiver_AIFightEnemy.TryGiveJob`.** It is where vanilla picks abilities, but
  it runs only when the think tree asks for a job: a melee job issued 60 cells out is not asked
  about again for 6 to 8 seconds, by which time the pilot has arrived, so the sprint would
  start late or not at all. It also costs a patch on a protected method that three subclasses
  override.
- **A learned copy of each ability** (`CompAbilities.GiveAbility`), which VEF's own hook would
  fire. A learned instance's `holder` is the pawn, so the strike pool, which is on the pads,
  would not limit it, and the toggle would still never fire.
- **A hidden vanilla `AbilityDef` with `aiCanUse`**, as ghoul frenzy is cast. It would ride
  vanilla's own path, at the price of a second ability beside each of ours and an exception to
  the rule that the set's abilities are VEF's.
- **Governing the spine.** Holding a spine back while the rush runs would stretch the rush, and
  it is the gene's business, not the set's (decision 2).
- **Starting the rush for a strike.** A gun-carrying pilot's tear lands without the rush's
  damage unless something is already within `aiCloseDistance`, which a strike's target is.

## 6. Decisions for the author

1. **Who is the boss.** `shipMasterChance` is 0, so in default play the only hostile pilot is a
   betrayed meeting's, who fights while leaving. Recommended: raise the default once this is
   built and measured, and re-price `shipMasterPrice`, whose 450 counts plating only. Left at 0,
   this spec still stands and the feature waits on the setting, or on another mod's pilots. The
   chance and the price are the armor def's and outside this spec's build.
2. **Hemogen between the spine and the rush.** Recommended: leave the spine alone and measure
   section 4. If the rush runs too short, the fallback is an `aiHemogenFactor` on the rush comp
   that scales an AI pilot's drain.
3. **Strike pacing.** Recommended: the two loaded strikes, 600 ticks apart. Another answer
   changes `aiStrikeSpacingTicks` alone.
4. **Whether an AI strike may kill outright.** Recommended: no, as section 3.3 has it, the head
   left alone and a lethal drain skipped. On "yes", drop the head test from `WorthStriking` and
   the drain toil's check. A torn limb's own bleeding can kill either way.
5. **Settings.** Recommended: one switch per ability, on by default, and no sliders for the
   distances.
6. **Which pilots.** Recommended: every pawn not of the player's faction, as section 3.1 has
   it. That takes in a meeting's sanguophages fighting raiders beside the colony and a captured
   pilot in a prison break. Vanilla's own line for abilities is wider by a hair
   (`!IsColonist`, which also lets a colony's unsecured slave cast).

## 7. Text to bring in step when built

- `Source/1.6/CLAUDE.md`: the line that VEF fires no apparel ability's autocast stays true and
  gains where the AI's use comes from.
- `Source/1.6/Abilities/Dismember/CLAUDE.md`: the class is in five files.
- `Warcasket_Hemophage.xml`'s header: the paragraph on apparel tags says a raider casts neither
  ability; the set stays off generated pawns for the gene alone.
- `Ability_HemogenRush.cs` and `Ability_Dismember.cs`: the headers' words on autocast.
- `CLAUDE.md`'s test paragraph: seven files touch no game type, and what `PilotTacticsTests`
  holds.
- `TODOs.md`: the first item closes.
