# Source/1.6/: C# guidance

Loaded when work touches the code. Each file's header carries its decompile-verified call paths
and the rationale for its numbers; this file holds only the rules that cross files. The csproj's
comments cover install-path detection, the compile-only VFEP/VEF references (`VFEP_PATH` and
`VEF_PATH` override the Workshop copy) and the deploy manifest.

The code so far is the armor's hemogen rush, its toggle (`Abilities/`), the hediff comp that
runs it (`Hediffs/`) and the glow node every piece draws while it runs (`Rendering/`, with its
patch), the pads' Dismember, its ability, part picker and job driver (`Abilities/`), then the
pilot's work trait (`WorkProfile.cs`), the startup that mirrors it (`Startup.cs`) and the
settings window (`Settings/`), with a section for each ability and one for the work. Several
rules below are the family's, written down before the code they govern so the next ability or
comp lands in the right shape; each names the sibling file to copy from rather than restating it.

- **Patch classes live in `Patches/`** under the `.Patches` namespace suffix, to avoid RimWorld
  type-name conflicts. Log with the `[Hemophage Warcasket]` prefix.
- **Patch timing is the load-bearing hazard.** `PatchAll()` runs from the
  `[StaticConstructorOnStartup]` in `ModInit.cs`, never from a `Mod` subclass constructor: a
  detour on a VFEP method applied before defs load runs that type's static ctor early and breaks
  it for good (the BetterTradersGuild v1.1.0 CWTL incident). `ModInit`'s header has the
  rationale; `HemophageWarcasketMod` owns the settings only.
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
  `HemophageWarcasketSettings.Generation` (`Ability_Dismember`).
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
- **A walk-up ability is ordered as a vanilla job and bound in its driver.** `Ability_Dismember`
  and `JobDriver_Dismember` are the model, after the Medic repo's: the job goes through
  `TryTakeOrderedJob` so it interrupts and queues as an order does, and the driver writes
  `CompAbilities.currentlyCasting` when the job starts. An apparel ability's `pawn` is null
  until its first gizmo draw or that binding, and VEF fires no apparel ability's autocast, for
  the player or for raiders.
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
