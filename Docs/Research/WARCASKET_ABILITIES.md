# Warcasket Abilities Research

> The survey of VFEP's sets from the sibling Shipcracker Warcasket repo's research doc of the same
> name, where it was written while scoping that set; the VFEP facts are set-independent and serve
> this set the same way. That set's own scoping notes and design proposal stay in its repo.

This doc covers *mechanics*: what each Vanilla Expanded warcasket piece does beyond its stats, how
the pieces of a set talk to each other, and which framework levers implement that. Baseline stat
tables (armor, insulation, costs, tags, inheritance) stay in `VFEP_WARCASKET_STATS.md`; our own
tuning rationale lives in the def headers under `1.6/Defs/`.

Sources, all read on 2026-09-15: Vanilla Factions Expanded - Pirates (VFEP) at `5a5618f`
(`../VanillaExpanded/VanillaFactionsExpanded-Pirates`, `1.6/Defs/ThingDefs_Misc/Apparel_Various.xml`,
`Apparel_Headgear.xml`, `1.6/Defs/AbilityDefs/Abilities.xml`, `1.6/Defs/Stats/Stats_Abilities.xml`,
`1.6/Defs/ThingDefs_Races/Races_Mechanoid.xml`, `1.6/Source/VFEPirates/`), Vanilla Expanded
Framework (VEF, `../VanillaExpanded/VanillaExpandedFramework`, `Source/VEF/Apparels/`,
`Source/VEF/Abilities/`, `Source/MVCF/`), and the game's `Assembly-CSharp.dll` via `ilspycmd`.
Every number is what ships in that XML or code. Informational only, not mod content.

## Set functionality, all thirteen VFEP sets

"Standard block" below means what every piece inherits or declares identically: `Forbiddable`,
`CompColorable`, `isUnifiedApparel` (helmets also `hideHead`), and on armor shells
`VEF_MassCarryCapacity +125`, the `VFEP_WarcasketTrait` on equip / `VFEP_Shellcasket` on unequip
trait swap, and `pawnCapacityMinLevels` Moving and Manipulation at 0.7. The 10th-generation
spacer sets additionally share `PsychicSensitivity -0.5`, `ToxicResistance +0.5` on armor and
helmet and `VacuumResistance` 0.2 / 0.1 / 0.7 (armor / shoulders / helmet, Odyssey only).

Shield energy shown as the in-game pool: `EnergyShieldEnergyMax` x100. Recharge
`EnergyShieldRechargeRate` r means the bubble regains r x max per second, so 0.01 is a full
refill in 100 s and 0.05 in 20 s (`CompShieldBubble.EnergyGainPerTick` = r / 60, applied x max).

### 3rd to 5th generation: stats only

| Set | Gen | Armor shell | Shoulder pads | Helmet | Identity |
| --- | --- | ----------- | ------------- | ------ | -------- |
| **Warcasket** | 3rd | Standard block. Sharp 1.06 / Blunt 0.55, Mass 50, `MoveSpeed -0.5`. | Standard. `MoveSpeed -0.2`. | Standard. | The baseline. |
| **Marine** | 4th | Standard. Sharp 1.20 / Blunt 0.65, Mass 50, `MoveSpeed -0.5`. | Standard. `MoveSpeed -0.2`. | Standard. | Straight armor upgrade at equal mass. |
| **Recon** | 5th | Standard. Sharp 1.02 / Blunt 0.6, Mass 40, `MoveSpeed -0.2`. | Standard. Mass 15, `MoveSpeed -0.1`, `Flammability` 0, `EquipDelay` 14 (every other piece is 1). | Standard. Mass 4, no `MoveSpeed` offset at all. | Lighter and faster. |
| **Cataphract** | 5th | Standard. Sharp 1.56 / Blunt 0.65, Mass 75, `MoveSpeed -0.85`. | Standard. Mass 35, `MoveSpeed -0.35`. | Standard. | Heaviest pre-spacer tank. Tags `WarcasketHeavy`, `WarcasketCata`. |

No comps, verbs or abilities on any of the twelve pieces.

### 7th generation: "designed to work as a set"

| Set | Armor shell | Shoulder pads | Helmet | Role and synergy model |
| --- | ----------- | ------------- | ------ | ---------------------- |
| **Aerial** | No comps. Feeds the jump through `equippedStatOffsets` on custom stats: `VFEP_PowerJumpRange +25`, `VFEP_PowerJumpDetonationRadius +1`; `equippedStatFactors` `VFEP_FlightSpeed` x1.5. Sharp 1.65 / Blunt 0.72. | Grants `VFEP_PowerJump` (`CompAbilitiesApparel`) fueled by `CompApparelReloadable`: 100 charges, 1 Chemfuel per charge, 60-tick reload. `Ability_PowerJump` burns 20 charges per jump (5 per tank), 60-tick cast, `targetMode` Location, target must be a walkable cell within the wearer's `VFEP_PowerJumpRange`; no line-of-sight test. Bomb explosion, 15 damage, radius = wearer's `VFEP_PowerJumpDetonationRadius` (base 3), at takeoff and again at landing, wearer excluded. Adds `VFEP_PowerJumpRange +5`, `VFEP_FlightSpeed` x1.5. | `VFEP_PowerJumpRange +10` only. | Mobility skirmisher. Shoulders are the engine; armor and helmet feed it through custom StatDefs the ability reads off the pawn. Full set jumps 40 tiles with a radius-4 blast; shoulders alone 5 tiles, radius 3. |
| **Barrage** | Grants `VFEP_SiegeMode` (`CompAbilitiesApparel`): a self-target toggle (`Ability_SiegeMode`, `CommandAbilityToggle`) applying hediff `VFEP_SiegeMode`, `VEF_VerbRangeFactor +0.25` and `RangedCooldownFactor -0.25`. `Hediff_SiegeMode` removes itself and flips the toggle off the moment the pawn starts moving. `cooldownTime` 600 ticks scaled by stat `VFEP_SiegeModeCooldown`. `MoveSpeed -1.0`. | `tickerType` Normal. `CompApparelReloadable`: 4 charges, 25 Steel per charge, "grenade". Verb `Verb_LaunchProjectileStaticMultiple` with `VerbProps_MultipleProjectiles`: "grenade barrage", `projectileCount` 4 of `VFEP_Proj_GrenadierGrenade` (vanilla frag grenade stats), `range` 12.9, `warmupTime` 1.5, `forcedMissRadius` 1.9, `ai_IsBuildingDestroyer`; one activation loops `TryCastShot` four times. `equippedStatFactors` `ShootingAccuracyPawn` x1.1. `MoveSpeed -0.4`. | Standard. `MoveSpeed -0.2`. | Planted fire support. Each piece is independent: armor buffs any ranged weapon while stationary, shoulders add an area burst, helmet is inert. No cross-piece reads. |
| **Hazard** | `statBases` `Flammability` 0 and `equippedStatFactors` `Flammability` x0. No comps. | `equippedStatFactors` `Flammability` x0.5. `MoveSpeed -0.4`. | `equippedStatFactors` `PsychicSensitivity` x0; `ToxicResistance +1`. | Environmental immunity, pure XML. The `WarcasketFlamer` tag it shares with the heavy flamer only pairs them at pawn generation (`VFEP_Pyro` kind); no runtime fire interaction. |
| **Shock** | Grants `VFEP_BlastOff` (`CompAbilitiesApparel`) fueled by a 100-charge Chemfuel `CompApparelReloadable`; `Ability_BlastOff` needs and burns the whole tank. `worldTargeting`: drop-pods the pawn to any map within `maxLaunchDistance` 30 world tiles (`CanHitTargetTile` requires a `MapParent`), 60-tick cast. On landing `PawnIncoming.Impact` fires a radius-15 Bomb explosion for 15 damage that ignores the wearer. Mass 65, `MoveSpeed -0.75`, Blunt 1.24. | `equippedStatFactors` `MeleeHitChance` x1.2, `MeleeDodgeChance` x1.2. `MoveSpeed -0.2`. | `equippedStatFactors` `MeleeDodgeChance` x1.2. | Orbital-drop melee. Armor delivers the pilot, shoulders and helmet make them a better brawler once there. "Benefits from enemy friendly fire" is flavor; nothing implements it. |

### 10th generation: spacer sets

| Set | Armor shell | Shoulder pads | Helmet | Role and synergy model |
| --- | ----------- | ------------- | ------ | ---------------------- |
| **Siegebreaker** | Ranged-only shield bubble (`CompShieldBubble`, `blockRangedAttack`, melee passes, wearer shoots through): 250 energy, recharge 0.01 (100 s to full). Breaks on depletion, hardcoded 3200-tick reset, returns at 20%. EMP zeroes it and passes. Sharp 2.0 / Blunt 1.0. | No comps. `EnergyShieldRechargeRate +1` as an `equippedStatOffsets` entry, meant to speed the armor's recharge. **Dead XML** (discrepancy 1). | Standard. | Ranged fire support. Armor carries the mechanic, shoulders were meant to amplify it, helmet is inert. |
| **Guardian** | Same bubble at 150 energy, 0.01. Plus `CompShieldField`: `manualActivation`, `workingTimeTicks` 600 (10 s), `cooldownTicks` 5400 (90 s), radius from `VEF_EnergyShieldRadiusApparel` 5 read off the armor Thing. No energy stats set, so the field is `Indestructible`: it cannot be drained or EMP'd, only time-gated. `EnergyShieldTick` intercepts every projectile inside the radius whose launcher is outside it, protecting everyone inside; shots from inside pass; `flyOverhead` projectiles past midpoint are exempt. No melee blocking. | `thingClass` `VFEPirates.Apparel_GuardianShoulders`: `CheckPreAbsorbDamage` fully negates any damage instance with a flat 25% chance. `MoveSpeed -0.5` (shoulders elsewhere are -0.1 to -0.4). | Standard. | Squad protector. Armor projects a 10-second area dome for allies, shoulders give the wearer a personal 25% negate. Independent pieces. |
| **Controller** | Bubble at 150 energy, 0.01. `CompApparelReloadable` 1 charge, 1 ComponentSpacer, "drone deployment". Verb `Verb_DroneDeployment` (`targetable` false, non-violent): spawns 3 `VFEP_Mech_Wardrone` at the wearer's feet, **4 if `VFEP_WarcasketHelmet_Controller` is worn** (explicit def check in C#). Wardrone: `MoveSpeed` 4.7, `baseHealthScale` 0.22, autonomous `VFEP_WarDrone` think tree, miniturret gun range 28.9, `CompProperties_DestroyAfterDelay` 3600 ticks, small explosion on death. | `CompApparelReloadable` 1 charge, 1 ComponentSpacer, "spider mine deployment". Verb `Verb_Spidermine`: `range` 14.9, `warmupTime` 0.5, `VFEP_SpidermineProjectile` spawns one `VFEP_Mech_Spidermine` at impact; it walks to the nearest enemy and explodes (flame, radius 4.9), self-destructs if nothing to hunt. | Standard. Its only function is the +1 drone the armor's verb grants when it is worn. | Summoner. The one set where a piece checks for a sibling piece by def in code. |
| **Sarcophagus** | Bubble at 150 energy, 0.01. `pawnCapacityMinLevels` at 1.0: Moving, Manipulation, BloodFiltration, BloodPumping, Metabolism. `preventDowning`, `preventBleeding`, `preventKilling` gated by `preventKillingUntilHealthHPPercentage` 0.6 and `preventKillingUntilBrainMissing`: the pilot dies only once remaining body-part max-HP falls to 60% or the brain is gone. | `pawnCapacityMinLevels` at 1.0: BloodFiltration, BloodPumping, Metabolism (duplicates the armor). Description promises Manipulation; the XML does not floor it. | `pawnCapacityMinLevels` at 1.0: Consciousness, Sight, Hearing, Talking, Breathing. | Unkillable pilot. Each piece floors the capacities of the body region it covers. All death suppression lives on the armor. |
| **Brute** | Bubble at **500 energy, recharge 0.05** (20 s to full), `blockRangedAttack`, `blockMeleeAttack` false, **`dontAllowRangedAttack`**: `CompAllowVerbCast` refuses every non-melee verb, so the wearer is melee-only while shielded. Sharp 2.0 / Blunt 1.5. Tags `WarcasketVeteran`, `WarcasketMelee`, no `WarcasketAll`. | `equippedStatOffsets` `VEF_EnergyShieldEnergyMaxFactor +0.5`: a pawn-level stat `CompShieldBubble.EnergyMax` does read, so the bubble becomes 750. Sharp 2.0 / Blunt 1.5. No melee stats despite the "melee assistor" flavor. | `MVCF.Comps.CompProperties_VerbGiver` exposes verb `Verb_ShieldDetonation` ("shield detonation", self-cast, `IsMeleeAttack`): sums `Energy` over every worn `CompShieldBubble`, Bomb explosion centered on the wearer with radius energy / 50 and damage energy / 20 (750 energy: radius 15, 37 damage), wearer excluded, then zeroes every shield without calling `Break`, so no reset timer. Available only while some shield has energy. No cooldown of its own. | Melee bomb. Armor is the battery, shoulders enlarge it, helmet spends it. The tightest three-piece loop in VFEP, and the pawn-level shield stat is the mechanism that makes the shoulders work. |

### Synergy models VFEP actually uses

1. **Custom StatDefs read off the pawn** (Aerial): the ability calls `pawn.GetStatValue` on
   `VFEP_PowerJumpRange` etc., so any worn piece can contribute an offset. Cleanest and fully XML
   on the contributing pieces.
2. **Pawn-level VEF shield stats** (Brute shoulders): `VEF_EnergyShieldEnergyMaxFactor` and
   `VEF_EnergyShieldEnergyMaxOffset` are read off the pawn by `CompShieldBubble.EnergyMax`. There
   is no recharge or radius equivalent; those are read off the apparel Thing (discrepancy 1).
3. **Explicit sibling check in C#** (Controller helmet): `Verb_DroneDeployment` looks for the
   helmet def in `WornApparel` and adds a drone. Simple, but hard-codes the def.
4. **`equippedStatFactors` via `ApparelExtension`** (Shock, Barrage, Hazard): multiplicative stat
   tweaks applied by VEF's stat patch; independent of other pieces.
5. **`pawnCapacityMinLevels` per body region** (Sarcophagus): each piece floors the capacities it
   plausibly covers.
6. **`thingClass` override** (Guardian shoulders): `Apparel.CheckPreAbsorbDamage` for a per-hit
   effect with no comp at all.
7. **Verb-on-apparel** via vanilla `<verbs>` plus `CompApparelReloadable` (Barrage shoulders,
   Controller) or MVCF `Comp_VerbGiver` (Brute helmet), versus **ability-on-apparel** via VEF
   `CompAbilitiesApparel` (Aerial, Barrage armor, Shock). Verbs get reload/ammo for free;
   abilities get cooldowns, cast times, toggles and world targeting.

### Flavor text that the code does not back

1. **Siegebreaker shoulders' recharge bonus never reaches the shield.** `CompShieldBubble` reads
   `EnergyShieldRechargeRate` and `EnergyShieldEnergyMax` via `parent.GetStatValue(...)` on the
   apparel Thing (statBases and quality only), and vanilla `StatWorker.GetValueUnfinalized`
   applies `equippedStatOffsets` only when the stat request is for a Pawn. The wearer's stat goes
   up; nothing reads it. `CompShieldField` reads its radius stat the same way.
2. Brute's description says the shield blocks melee as well; `blockMeleeAttack` is false. It also
   calls the recharge "particularly slow"; at 0.05 it is five times Siegebreaker's.
3. Brute shoulders' "melee assistor modules" have no stat behind them.
4. Sarcophagus shoulders promise Manipulation replacement and do not floor it.
5. Shock "benefits from enemy friendly fire": nothing in VFEP code or patches.
6. Controller helmet's "deployment module but no controller" is exactly the +1 drone check; fine,
   but the helmet has no comp of its own.
7. Hazard's flamer link is a pawn-kind loadout pairing only.

### Tag gating worth knowing

`StaticStartup` collects every tag used only by `WarcasketDef`s; `PawnGenerator_GeneratePawn_Patch`
fills a warcasket wearer's missing slots with pieces whose tags intersect the pawn kind's
`apparelTags`. Sets carrying `WarcasketAll` are in every warcasket kind's pool; Brute
(`WarcasketVeteran`, `WarcasketMelee` only) appears only on kinds that list those tags. A set's
tags decide which raiders and mercenaries can spawn in it.

## Framework levers

- **VEF `AbilityDef`** (`Source/VEF/Abilities/Defs/AbilityDef.cs`): `range` plus
  `rangeStatOffsets`/`rangeStatFactors` and `maxRange`, `cooldownTime`, `castTime`,
  `requireLineOfSight` (default true; VFEP's Power Jump overrides `CanHitTarget` and skips it),
  `worldTargeting`, `showUndrafted`, `requiredTrait`, `requiredHediff`, `verbProperties`.
- **VEF `Ability`** virtuals (`Source/VEF/Abilities/Misc/Ability.cs`): `ShowGizmoOnPawn`,
  `IsEnabledForPawn(out reason)`, `GetRangeForPawn`, `CanHitTarget(target, sightCheck)`,
  `ValidateTarget`, `Cast`, `PostCast`. `CompAbilitiesApparel` builds its list once at
  `Initialize`.
- **Vanilla `CompProperties_ApparelReloadable`**: single `ammoDef`, `ammoCountPerCharge`,
  `ammoCountToRefill`, `baseReloadTicks`, `replenishAfterCooldown`.
- **VFEP Power Jump internals** (`Ability_PowerJump.cs`): `CanHitTarget` = walkable cell within
  `VFEP_PowerJumpRange` (base 0), blast radius `VFEP_PowerJumpDetonationRadius` (base 3, min 3, so
  Aerial's +1 gives 4), flight speed `VFEP_FlightSpeed` (base 12). Both detonations are
  `GenExplosion.DoExplosion(..., DamageDefOf.Bomb, damage 15)` ignoring the wearer.
