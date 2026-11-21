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
- **The helmet**: sunlight protection (`Sunlight/`, with its three patches) and pyrophobia
  relief (`Fire/`, with its one).
- **The pilot's work**: the trait's rule (`WorkProfile.cs`) and the startup that mirrors it
  (`Startup.cs`).
- **The settings window** (`Settings/`): a section for each piece's mechanic, one for the work
  and a last one for Vanilla Races Expanded - Sanguophage, shown only while that mod is active.
- **The mask that keeps the set off generated pawns**: `ListMask.cs`, with its patch.

Several rules below are the family's, written down before the code they govern so
the next ability or comp lands in the right shape; each names the sibling file to copy from
rather than restating it.

- **Patch classes live in `Patches/`** under the `.Patches` namespace suffix, to avoid RimWorld
  type-name conflicts. Log with the `[Hemophage Warcasket]` prefix.
- **Patch timing is the load-bearing hazard.** `PatchAll()` runs from the
  `[StaticConstructorOnStartup]` in `ModInit.cs`, never from a `Mod` subclass constructor: a
  detour on a VFEP method applied before defs load runs that type's static ctor early and breaks
  it for good (the BetterTradersGuild v1.1.0 CWTL incident). `ModInit`'s header has the
  rationale; `HemophageWarcasketMod` owns the settings only.
- **A postfix on a check the game asks constantly does nothing until the original's answer
  needs changing.** The stat worker asks a gene's conditional affecter on every stat it computes
  for a carrier, so the sunlight postfix tests `__result` before it looks at the pawn
  (`Patches/ConditionalStatAffecter_InSunlight_Applies`), and never patches the stat worker
  itself. A patch's header says how often its target runs and for which pawns.
- **A patch on a corner of VEF or VFEP the set does not otherwise depend on finds its target by
  name and skips itself with a warning when it is missing**
  (`Patches/HediffComp_StageBySunlight_CompPostTickInterval`): one target that fails to resolve
  throws out of `PatchAll` and takes every later patch and `Startup.Run` with it.
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
- **An apparel ability's `pawn` is null until its first gizmo draw** or until something binds
  it, and VEF fires no apparel ability's autocast, for the player or for raiders.
  `Abilities/Dismember/CLAUDE.md` has how a job binds it.
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
