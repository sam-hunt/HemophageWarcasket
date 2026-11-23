# SPEC: The hemogen rush and Dismember for a pilot the player does not control

Status: BUILT, not run in game (section 3.7). Section 6 has the author's answers. Every engine fact below was read,
not run: VEF at checkout `fa069a72` (`../VanillaExpanded/VanillaExpandedFramework/Source/VEF/Abilities/`,
paths below relative to it), VFEP at `be4040e`, vanilla by decompile of 1.7.5014 and the frozen
1.6 assembly, the same on both for every method named. Each fact names where it was read so the
build session can re-check it instead of trusting this file.

## 1. Goal

Any pilot of the set that the player does not control should fight as the set's pilot does:
start the rush as it closes to melee, keep hemogen back for it, and tear a limb off what it
fights with the strikes loaded in the pads. Today it casts neither and fights with its plating
alone. The pilots this reaches are the quests' sanguophages (`Quests/QuestPilot.cs`), a captured
pilot in a prison break and any pawn another mod dresses in the set.

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
- Every humanlike race def gets VEF's `CompAbilities` at startup, whoever the pawn belongs to
  (`Misc/InitializeAbilityComp.cs`), so the comp `JobDriver_Dismember` binds is there on an NPC.
- VFEP changes nothing in a warcasket pilot's combat AI (its source has no think node, job giver
  or `TryGetAttackVerb` patch for pilots).

**VEF does tick an apparel ability, which is the way in.**

- `AbilityDef.needsTickingInterval` puts the ability in the comp's interval list
  (`Defs/AbilityDef.cs:14-15`, with a config error at `:130` for a class that declares
  `TickInterval` without the field), and `CompAbilitiesApparel.CompTickInterval` calls
  `Ability.TickInterval(int delta)`, a virtual with an empty body
  (`Comps/CompAbilitiesApparel.cs:96-101`, `Misc/Ability.cs:231`). The list is rebuilt from the
  def on load (`:81`), so a saved piece picks the field up.
- A worn piece ticks through its wearer: `Thing.DoTick` ticks a `tickerType` Normal thing, then
  the directly held things of every holder under it, and `ThingWithComps.TickInterval` calls
  each comp's `CompTickInterval`. A piece whose def is `tickerType` Never gets nothing.
- A worn piece's interval comes at the piece's own rate (`GenTicks.GetCameraUpdateRate`, asked
  of the piece), every 15 ticks at the slowest and for a piece that was never spawned, with the
  ticks since the last as its delta. The wearer's own rate, 1 to 5 on screen, is what a hediff
  comp's interval gets.
- VFEP's armor base is Normal and its part base is Never
  (`1.6/Defs/ThingDefs_Misc/Apparel_Various.xml:70` and `:29`), so our armor ticks and our pads
  do not. VFEP's own Barrage pads set Normal on the def (`:828`).

**How vanilla's fight job giver behaves, which the rules below read.**

- `JobGiver_AIFightEnemy.TryGiveJob` sets `pawn.mindState.enemyTarget` (`UpdateEnemyTarget`),
  tries the pawn's vanilla abilities for a non-colonist (`GetAbilityJob`), then gives a melee
  pawn an `AttackMelee` job on that target at any distance, with an expiry of 360 to 480 ticks
  that re-asks the think tree (`MeleeAttackJob`). An ability's job has no expiry and ends with
  the cast.
- `enemyTarget` is not cleared when the fight ends, so it goes stale once the pawn's duty stops
  reaching the giver. `JobGiver_ReactToCloseMeleeThreat` gives `AttackMelee` too, without
  touching it.
- `Pawn_JobTracker.StartJob` records the node that issued a job in `Job.jobGiver`, null for a
  job started without one.
- The duties the quests' sanguophages fight under all reach that giver through a subclass:
  `Defend` (`JobGiver_AIDefendPoint`) and `AssaultColony` (`JobGiver_AIFightEnemies`) for the
  crashed shuttle, `ExitMapBestAndDefendSelf` (`JobGiver_AIDefendSelf`, which takes a target
  within 35 cells) for a betrayed meeting and `AssaultThing` for a meeting that turns on raiders.
- `AttackMelee` is not only for enemies. Under `AssaultColony` and `AssaultThing` a raider with
  nobody to reach gets it on a building from the trash givers (`TrashUtility.TrashJob`), doors
  and walls among them.
- The engine's own test for something that fights back is `IAttackTarget.ThreatDisabled`: a
  pawn and a turret are attack targets, a door and a wall are not, and a downed pawn's threat is
  disabled (`Pawn.ThreatDisabled`).

**What a sanguophage NPC already does with its hemogen.**

- It arrives with a full tank: `Gene_Resource.Reset` sets the value to the maximum.
- It casts piercing spine on its own (`aiCanUse`, with `ai_IsOffensive` true by default, range
  3.9, 20 hemogen, 60 tick cooldown), and the giver prefers it to the weapon. In melee that is
  about one spine each time the melee job expires, so roughly 20 hemogen every 7 to 9 seconds,
  about the rush's own drain. Longjump and coagulate are not `aiCanUse`.
- The AI's one test of the cost is `CompAbilityEffect_HemogenCost.AICanTargetNow`: the value is
  at least the cost, with nothing held back. `Ability.AICanTargetNow` asks it of every effect
  comp before the giver may pick the ability.

**What VEF's cast job does that matters here.**

- `Ability.Cast` sets `cooldown` to now plus `GetCooldownForPawn()`, a virtual, and
  `IsEnabledForPawn` refuses while it runs; the field is saved (`Misc/Ability.cs:134`, `:537`,
  `:67`, `:766`). Nothing sets it before the cast.
- `JobDriver_CastAbilityOnce.MakeNewToils` sets `job.playerForced` for every cast, whoever the
  caster (`AI/JobDrivers/JobDriver_CastAbilityOnce.cs`). On an NPC that means damage does not
  interrupt the job (`Pawn_JobTracker.Notify_DamageTaken`), the close melee threat reaction
  stands down (`PawnUtility.PlayerForcedJobNowOrSoon`) and the path accepts deadly danger.
  Nothing in `Lord`, `LordToil` or `LordJob` reads it, so a duty change replaces the job as it
  would any other.

## 3. Design

Each ability decides for its own wearer on its interval tick, through two small static classes,
and everything it then does is the path a player's click already takes. One Harmony postfix
keeps hemogen back for the rush, on the one method the AI asks whether it can pay for an
ability.

### 3.1 Who is an AI pilot, and what it is fighting

`Abilities/AiPilot.cs`, the readers that turn a pawn into the rules' arguments:

- **`Is(Pawn)`**: `pawn.Faction != Faction.OfPlayer`. A player's pilot stays the player's to
  command, in a mental state too. A prisoner keeps their own faction and so is an AI pilot. A
  slave, a berserk colonist and a quest lodger are of the player's faction and are not.
- **`Acts(Pawn)`**: an AI pilot that is spawned, not downed and awake.
- **`FightTarget(Pawn)`**: the target of the current job when it is a melee job; else
  `mindState.enemyTarget` when the current job's `jobGiver` is a `JobGiver_AIFightEnemy`; else
  null. The giver test is what keeps a stale `enemyTarget` out.
- **`InMeleeJob(Pawn)`**: the current job's def is `AttackMelee`, or its driver is a
  `JobDriver_Dismember`.
- **`Threat(Pawn pilot, Thing)`**: the thing is spawned on the pilot's map and is an
  `IAttackTarget` whose `ThreatDisabled(pilot)` is false. A door being broken down and a downed
  pawn being finished off are fight targets and not threats.

Each ability finds its wearer as `(holder as Apparel)?.Wearer`, never through `pawn`, and asks
once every 30 ticks (`wearer.IsHashIntervalTick(30, delta)`, which passes once in 30 ticks
whatever the piece's rate).

`Abilities/PilotTactics.cs` holds the decisions as plain arithmetic on numbers, booleans and our
own enums, naming no game type, as `Quests/ThrallBudget.cs` does, so its tests run anywhere.

### 3.2 The rush

**Engaged** (`PilotTactics.RushEngaged`): the pilot's fight target is a threat, and either the
pilot is in a melee job with the target within `aiStartDistance`, or the target is within
`aiCloseDistance` whatever the job.

**Start**, in `Ability_HemogenRush.TickInterval`: the wearer acts as an AI pilot, the comp's
`aiUses` is on, there is no rush hediff, the hemogen gene is active with `CanOffset` and
`Rush.Affords(value)`, and the pilot is engaged. It adds `HPWC_HemogenRush`, as the toggle does.

**Stop**, in `HediffComp_HemogenRush.CompPostTickInterval`: for an AI pilot, ticks not engaged
are counted and the rush ends at `aiStopAfterTicks`; an engaged tick resets the count. With
`aiUses` off no tick is engaged. The count is not saved, so a load restarts the grace. Every
existing end still applies.

The melee clause is what makes a melee pilot sprint the last stretch. The close clause covers a
pilot with a gun that something has reached, and keeps the rush on between melee jobs while the
pilot casts a spine.

### 3.3 Hemogen kept for the rush

`Patches/CompAbilityEffect_HemogenCost_AICanTargetNow.cs`, a postfix that turns a yes into a no
when the caster is an AI pilot wearing the rush's armor, `aiUses` is on and the cost would take
its hemogen under `aiHemogenKept` (`PilotTactics.AffordsAbility`: the value less the kept level
is at least the cost). The method is asked by the AI alone, so a player's order is untouched,
and it covers every ability with a hemogen cost, another mod's too. The AI simply sees an
ability it cannot pay for. At 0 the rule is off and at 1 the pilot casts none.

The rush and the pads' drain are not abilities with a hemogen cost and are not governed: the
rush runs the kept half down, and a drain fills the tank back up.

### 3.4 Dismember

A fifth file of the partial class, `Abilities/Dismember/Ability_Dismember.Ai.cs`, with the
`TickInterval` override. It starts the job when all of these hold, cheapest first:

1. The wearer acts as an AI pilot, the extension's `aiUses` is on, and its current job is not
   already a Dismember.
2. The fight target is a humanlike pawn, a threat, hostile to the pilot, within
   `aiStrikeDistance`, and reachable (`CanReach`, `PathEndMode.Touch`).
3. The ability is bound to the wearer (`pawn = wearer` and `Init()` when it differs), then
   `IsEnabledForPawn` and `ValidateTarget(target, showMessages: false)` pass: off cooldown, a
   strike loaded, capable of violence, a limb to take.
4. The foresight says a limb can come off (`StrikeOutlook.Tears` or `MayTear`), and the strike
   would not take the head (`PilotTactics.WorthStriking`). The head is taken only from a target
   with no limb left, so this leaves the limbless alone.

It then calls `wearer.jobs.StartJob(JobMaker.MakeJob(def.jobDef, target),
JobCondition.InterruptForced)`. `JobDriver_Dismember` binds the comp when the job starts, as it
does for an order.

Around the job:

- **Humanlikes only.** The two strikes are not spent on an animal or a mechanoid sent to take
  them, and only a humanlike pays the pilot back in hemogen.
- **Spacing.** `GetCooldownForPawn()` answers `aiStrikeSpacingTicks` for an AI pilot and the
  base's for anyone else, so VEF's own cooldown spaces the strikes and saves across a load. A
  job that ends before the cast has set no cooldown and may start again at the next ask.
- **Limit.** The pads' loaded strikes. An NPC carries no components and never reloads.
- **The walk gives up.** For an AI pilot, a target farther than twice `aiStrikeDistance` ends
  the job in the walk toil, with nothing spent.
- **The drain spares a life.** For an AI pilot, the drain toil is skipped when the target's
  blood loss plus a full drain's would kill (`DismemberDrain.Outlook` answers `Kills`), asked
  once as the drain would start.
- **Unchanged.** The strike's damage, its combat log line, the stun, the faction consequences
  and the wider drain sources are the cast's own and need nothing. `MeleeAim` already reads
  another mod's aim height for a player's pilot only.

### 3.5 Def fields and settings

| Where | Field | Value | Setting |
|---|---|---|---|
| `HediffCompProperties_HemogenRush` | `aiUses` | true | switch |
| | `aiStartDistance` | 25 | |
| | `aiCloseDistance` | 5 | |
| | `aiStopAfterTicks` | 300 | |
| | `aiHemogenKept` | 0.5 | |
| `DismemberExtension` | `aiUses` | true | switch |
| | `aiStrikeDistance` | 5 | |
| | `aiStrikeSpacingTicks` | 600 | |

- `HPWC_HemogenRush` and `HPWC_Dismember` (the ability defs) gain `needsTickingInterval`.
- `WarcasketShoulders_Hemophage.xml` gains `tickerType` Normal.
- Each switch is a `ToggleSetting` in its section's partial (`rushAiUse`, `dismemberAiUse`),
  written onto `aiUses`. The rush's switch carries the kept hemogen with it. Their strings are
  held to `l10n/languages/English.md`.
- The numbers' rationale goes in the two def headers. 25 cells is about a rifle's reach, so a
  melee pilot crosses the colony's firing line in the rush for about 12 hemogen (5 to start,
  then 2 a second for 3.5 seconds at 7.2 cells a second). 5 cells is one more than the spine's
  reach. 300 ticks of grace costs 10 hemogen, twice a restart. Half the tank kept is 25 seconds
  of rush, and leaves a full tank two spines. 600 ticks between strikes is time for the player
  to pull a colonist back.

### 3.6 Tests

- **`PilotTacticsTests`** (new, no game type): `RushEngaged` across the two clauses and their
  boundaries, `WorthStriking` across the four outlooks with and without the head,
  `AffordsAbility` at the kept level's edges.
- **`SettingsTargetTests`**: `aiUses` on the rush comp and on the extension.
- `CompatPatchTests` already holds a patch on the extension to the fields the C# declares.

### 3.7 To check in game

Dev mode, a crashed shuttle quest generated at 2500 points or more with the master's chance set
to 1 in the settings, and a meeting betrayed with a dressed leader.

1. The armor's `TickInterval` runs on a pilot nobody has selected, and the pads' does once
   their def ticks.
2. A melee master starts the rush about 25 cells out and the glow shows on every piece.
3. The rush ends about five seconds after the fight does, and when the tank runs dry. A master
   breaking down a door with nobody near does not rush.
4. The master casts a spine or two from a full tank and none under half.
5. The master tears a limb off a colonist it reaches, drains, and waits ten seconds before the
   second strike; the pads then read 0 / 2 and it strikes no more. An animal or a mechanoid in
   its way is fought and not torn.
6. A target the drain would kill is torn and not drained; a limbless target is left alone.
7. The pilot still follows its lord when the duty changes mid-job (the shuttle's switch from
   `Defend` to `AssaultColony`, a raid's retreat).
8. Save and load mid-rush and mid-Dismember.
9. A captured, dressed pilot in a prison break: it is an AI pilot and should behave as one.
10. With Combat Extended active, the same pass. Its guide (`1.6/Mods/CombatExtended/CLAUDE.md`)
    has what CE changes around the strike.

## 4. The fight this should make

An estimate from the numbers, to be measured in steps 2 to 5 above. A melee master with a full
tank spends about 12 hemogen on the sprint, tears a limb on contact and takes back up to 25,
casts one or two spines while its tank is over 70, and from then on loses only the rush's 2 a
second, with a second tear ten seconds in for up to 25 more. That is a rush of roughly 50
seconds, where the spine left alone would end it in 20 to 30.

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
- **No spine at all in the armor.** The simplest way to keep hemogen for the rush, at the price
  of the pilot's only ranged attack. A kept level of 1 is that rule.
- **Putting the spine on cooldown while the rush runs** (`Ability.StartCooldown`), which needs
  no patch. It writes the set's state onto the gene's ability, and a spine could still slip in
  between a drain's end and the rush's restart.
- **Starting the rush for a strike.** A gun-carrying pilot's tear lands without the rush's
  damage unless something is already within `aiCloseDistance`, which a strike's target is.

## 6. Decided

1. **Which pilots.** Every pawn not of the player's faction that wears the set and is in a
   fight, as section 3.1 has it. The feature is not the shuttle master's alone:
   `shipMasterChance` stays at 0 and is the armor def's business. In default play the pilot it
   reaches is a meeting's leader, dressed 7 times in 10, who on a betrayal takes targets within
   35 cells while leaving and can cost the colony two limbs.
2. **Hemogen between the spine and the rush.** Half the tank is kept for the rush
   (section 3.3).
3. **Strike pacing.** The two loaded strikes, 600 ticks apart.
4. **Whether an AI strike may kill outright.** No: the head is left alone and a lethal drain
   skipped. A torn limb's own bleeding can kill either way.
5. **Settings.** One switch per ability, on by default, and no sliders.
6. **What an AI pilot strikes.** Humanlikes only.

## 7. Text to bring in step when built

- `Source/1.6/CLAUDE.md`: the line that VEF fires no apparel ability's autocast stays true and
  gains where the AI's use comes from.
- `Source/1.6/Abilities/Dismember/CLAUDE.md`: the class is in five files.
- `Warcasket_Hemophage.xml`'s header: the paragraph on apparel tags says a raider casts neither
  ability; the set stays off generated pawns for the gene alone.
- `Ability_HemogenRush.cs` and `Ability_Dismember.cs`: the headers' words on autocast.
- `CLAUDE.md`'s test paragraph: seven files touch no game type, and what `PilotTacticsTests`
  holds.
- `TODOs.md`: the first item becomes the in game pass of section 3.7.
- The CHANGELOG entry says what a betrayed meeting's leader now does.
