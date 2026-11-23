# Source/1.6/: C# guidance

Loaded when work touches the code. Each file's header carries its decompile-verified call paths
and the rationale for its numbers; this file holds only the rules that cross files. The csproj's
comments cover install-path detection, the compile-only VFEP/VEF references (`VFEP_PATH` and
`VEF_PATH` override the Workshop copy) and the deploy manifest.

Where each mechanic lives:

- **The armor's hemogen rush**: the toggle (`Abilities/Ability_HemogenRush.cs`), the hediff comp
  that runs it (`Hediffs/`) and the glow node every piece draws while it runs (`Rendering/`,
  with its patch).
- **The pads' Dismember**: `Abilities/Dismember/`, which has a guide of its own (which file
  answers what, the walk-up job, the strike pool), and the pool itself (`Comps/`).
- **Both abilities for a pilot the player does not control**: who that is and what it is
  fighting (`Abilities/AiPilot.cs`), the decisions (`Abilities/PilotTactics.cs`, pure) and
  the patch that keeps hemogen back for the rush.
- **The helmet**: sunlight protection (`Apparel/SunlightShield.cs`, with its three patches)
  and pyrophobia relief (`Apparel/PyrophobiaRelief.cs`, with its one).
- **The pilot's work**: the trait's rule (`Traits/WorkProfile.cs`) and the startup that mirrors
  it (`Startup.cs`).
- **The settings window** (`Settings/`): a section for each piece's mechanic, one for the work
  and one for the quests' sanguophages.
- **The mask that keeps the set off generated pawns**: `Utilities/ListMask.cs`, with its patch.
- **The quests' sanguophages who arrive in the set, and the crashed shuttle's thralls who
  arrive in a lesser one**: the rules (`Quests/`) and their two patches.

Several rules below are the family's, written down before the code they govern so
the next ability or comp lands in the right shape; each names the sibling file to copy from
rather than restating it.

- **A folder is named for the kind of thing in it, never for a feature**: the game type its
  classes extend (`Abilities/`, `Comps/`, `Hediffs/`, `Patches/`) or the part of the game they
  act on (`Apparel/`, `Quests/`, `Traits/`, `Rendering/`), with `Utilities/` for code that
  names no game type. A feature with files enough for a folder of its own nests inside one
  (`Abilities/Dismember/`), and a def extension or a helper sits beside the code that reads
  it. The root holds `ModInit.cs`, `Startup.cs` and `HPWC_DefOf.cs` alone.
- **Patch classes live in `Patches/`** under the `.Patches` namespace suffix, to avoid RimWorld
  type-name conflicts. Log with the `[Hemophage Warcasket]` prefix.
- **Patch timing is the load-bearing hazard.** The patches are applied from the
  `[StaticConstructorOnStartup]` in `ModInit.cs`, never from a `Mod` subclass constructor: a
  detour on a VFEP method applied before defs load runs that type's static ctor early and breaks
  it for good (the BetterTradersGuild v1.1.0 CWTL incident). `ModInit`'s header has the
  rationale; `HemophageWarcasketMod` owns the settings only.
- **`ModInit` patches class by class, never through `Harmony.PatchAll`**, which stops at the
  first class that throws: a failing class costs its own patch and an error in the log, and
  every other class and `Startup.Run` still run. A new patch needs only its `[HarmonyPatch]`
  attribute to be picked up.
- **A postfix on a check the game asks constantly does nothing until the original's answer
  needs changing.** The stat worker asks a gene's conditional affecter on every stat it computes
  for a carrier, so the sunlight postfix tests `__result` before it looks at the pawn
  (`Patches/ConditionalStatAffecter_InSunlight_Applies`), and never patches the stat worker
  itself. A patch's header says how often its target runs and for which pawns.
- **A patch on a corner of VEF or VFEP the set does not otherwise depend on finds its target by
  name and skips itself with a warning when it is missing**
  (`Patches/HediffComp_StageBySunlight_CompPostTickInterval`): a typed target that fails to
  resolve is an exception in a player's log, where a stand-down is one line.
- **Startup def-derived state must survive an in-process play-data reload** (a language switch
  reloads every def). Anything that reads or writes a def at startup goes in the idempotent
  `Startup.Run`, called once from `ModInit` and again from the
  `StaticConstructorOnStartupUtility.CallAll` postfix in `Patches/`; never a bare static ctor.
  Mod-shipped textures are destroyed on the same reload: resolve them through a getter that
  re-resolves on a Unity-null `== null` check, never `??` or `??=`
  (`~/dev/PersonaWeaponsUnbound/Source/1.6/Defs/PWU_Textures.cs` is the pattern). Vanilla-path
  textures are safe as plain statics.
- **Settings override def fields, never code paths.** `HemophageWarcasketSettings` is split
  into one `Settings_<Section>.cs` partial per window section, each owning its fields, scribe
  entries, defaults, apply, reset and draw, and `ApplyToDefs` writes them onto the defs at
  startup and on window close. A slider is a `Tunable` and a switch a `ToggleSetting`, the
  family's from the Medic repo's `Source/1.6/Settings/`; `Tunable` is byte-identical across the
  family but for the namespace and key prefix, and owns the row layout constants. A setting
  that needs a code path gets a def field first (`glowPulses`, `canTargetNonHostile`) and the
  code reads the field. What a write leaves stale on a live ability (a running cooldown) is
  caught up by the instance on its next gizmo request, keyed on
  `HemophageWarcasketSettings.Generation` (`Ability_Dismember`); what it leaves stale on a
  worn piece (strikes over a lowered capacity) the settings trim themselves.
- **A switch an XML patch reads is a restart-time setting**, read through a
  `PatchOperationSetting` (the Overlord repo's `Source/1.6/PatchOperations/`) while the patches
  apply; its row carries the restart suffix and the settings code writes nothing for it.
- **Hemogen goes through Biotech's own entry points.** Gain with `GeneUtility.OffsetHemogen`,
  which applies `HemogenGainFactor`. Spend with
  `GeneResourceDrainUtility.OffsetResource(hemogenGene, -amount)`: `OffsetHemogen` sets the
  value directly for a pawn without a hemogen drain gene and so skips the check that starts
  hemogen craving at zero. Test for the gene with
  `pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>()`, which skips an inactive one, never by
  xenotype (`CLAUDE.md`, "Cross-cutting rules"). `Gene_Hemogen.CanOffset` is false for a
  deathresting pawn; an ability that spends hemogen respects it. All verified on 1.6.
- **The pilot's work trait is mirrored from VFEP's at startup.** A child `ApparelExtension`
  can only add to what a granted trait blocks, so the armor grants a trait of its own, written
  from `VFEP_WarcasketTrait` by `WorkProfile.Mirror` in `Startup.Run` so the mods that reshape
  VFEP's work profile reshape ours. Nothing else writes the trait's work fields, and nothing
  tests a pawn for `VFEP_WarcasketTrait`: a hemophage pilot does not carry it.
- **The quest pilots are one rule and two postfixes.** `Quests/QuestPilot.TryDress` puts
  the whole set on a living, hemogenic pawn by the chances, the research and, for the shuttle
  master, the floor on the quest's threat points, all on the armor's
  `QuestPilotExtension`, and is called only from the postfixes on the two vanilla quest roots
  that make Biotech's sanguophages (`Patches/QuestNode_Root_SanguophageShip_RunInt`,
  `Patches/QuestNode_Root_SanguophageMeetingHost_RunInt`), both bound by name with a
  stand-down. Never dress a pawn from a `PawnGenerator` patch: the pawn must have left
  generation, past VFEP's slot fill, which the mask answers only for a pawn wearing no part of
  ours. `QuestPilot`'s header has the order of events and what the pawn is left as; the armor
  def's has the numbers. The Overlord repo's `Quests/AncientPilot.cs` is the sibling, for the dead.
  The shuttle postfix also hands the quest's thralls to `QuestPilot.DressThralls`, which
  dresses as many as the pure `ThrallBudget` answers in the VFEP sets the extension's
  `thrallTiers` name. Every dressed pawn goes through the one `Dress`, which also paints the
  set in one color drawn through vanilla's `PawnApparelGenerator.PostProcessApparel`, so the
  color is the pawn kind's own and neither the code nor a def names one, and exchanges the
  weapon, for a sanguophage by the higher of Shooting and Melee and for a thrall by the kind of
  weapon held; VFEP's sets and weapons are named in the armor def's XML and nowhere in the C#,
  and Combat Extended's magazine is reached through `WeaponAmmo` by the names its root's patch
  fills in.
- **An apparel ability's `pawn` is null until its first gizmo draw** or until something binds
  it, and VEF fires no apparel ability's autocast, for the player or for raiders.
  `Abilities/Dismember/CLAUDE.md` has how a job binds it.
- **An ability serves a pilot the player does not control from its own `TickInterval`**, which
  VEF calls only for a def with `needsTickingInterval` on a piece whose def ticks. It finds the
  wearer through `holder`, asks `AiPilot` who and what, takes the decision from `PilotTactics`
  and then goes the way a player's click does. A new decision is arithmetic in `PilotTactics`,
  where the tests reach it, on numbers the def holds; `AiPilot.Is` is the one test of whose
  pilot it is.
- **A type of another mod's is named by a def field, never in the code**, as a def of another
  mod's is: the main tree leaves the field empty, that mod's compat root fills it in, and the
  code resolves the name by reflection, once, with a warning and a fallback when it is not there
  (`Abilities/Dismember/MeleeAim.cs`, for Combat Extended's aim height).
- **Mod-active checks from C#** go through a `ModDetection.IsActive(packageId)` helper
  (`../RimworldModTemplate/Source/1.7/ModDetection.cs` on its `1.7` branch, the Toxin repo's
  `PatchOperationModActive` for the XML side), never `ModsConfig.IsActive(string)`, which misses
  a Workshop copy stored with the `_steam` postfix.
- **Keyed strings a DLC owns are available only when the DLC is a hard dependency.** Biotech's
  are; Odyssey's, Ideology's and Royalty's are not. Before using a vanilla key from code, grep
  `Data/*/Languages/English/Keyed/`; if neither Core nor Biotech owns it, add an `HPWC_` key to
  the one Keyed file instead.
- **Verification notes name their game version.** The local install's assembly is the 1.7 one,
  while the project compiles against the 1.6 reference package and the DLL runs on both; a
  header call path says which build it was verified on, and a member that differs between the
  two versions is called out there. The reference package has no method bodies: decompile the
  frozen 1.6 copy (`~/.local/share/RimWorld-1.6/Managed`) or the install, as the `rimworld-logs`
  skill describes.
