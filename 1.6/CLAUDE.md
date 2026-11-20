# 1.6/: defs, patches, languages and compat roots

Loaded when work touches the game content. Each def's header carries its values and their
rationale; this file holds the rules that cross defs. The one `1.6/` tree serves 1.6 and 1.7
(`LoadFolders.xml` lists it under both and says why). Defs load recursively and the deploy
manifest globs, so new files and subfolders need no build change.

## Defs

- **Warcasket parts are `VFEPirates.WarcasketDef`s** on VFEP's abstract bases
  (`VFEP_WarcasketArmorBase`, `VFEP_WarcasketShoulderPadBase`, `VFEP_WarcasketHelmetBase`), so
  the foundry, entombing and removal surgery pick them up without C#. Check VFEP's XML before
  reimplementing anything a base provides. The three defs are the 10th generation's baseline,
  field by field the value most of VFEP's five spacer sets share; a field a def header does not
  discuss is that baseline's, so diff against VFEP's Sarcophagus before changing one.
- **Every `ApparelExtension` we declare carries `priority` 1.** The VFEP bases carry their own,
  and VEF merges duplicates at resolve time, keeping the highest priority and dropping fields its
  `Merge` does not copy; any new `ApparelExtension` field, from any load root, goes on our entry.
  The armor def's header has the mechanics.
- **The set's abilities are VEF abilities** (`VEF.Abilities.AbilityDef`, each granted by a
  `CompProperties_AbilitiesApparel` on the piece that carries it: the armor's hemogen rush
  and the shoulder pads' Dismember), never vanilla `AbilityDef`s: vanilla has no apparel
  grant, and the family's abilities are all VEF's. `Docs/Research/WARCASKET_ABILITIES.md`
  surveys VFEP's own.
- **Harm goes through `TakeDamage` with the pilot as instigator**, on a `DamageDef` with
  `externalViolence`, never by adding or removing hediffs directly: that one call is what makes
  the target's faction, guilt, kill blame and the damage records treat it as an attack.
  It writes no combat log line, so the caller adds one, on a rule pack of ours under vanilla's
  own log entry class. Dismember's strike is the model (`DamageDefs/Dismember.xml`,
  `RulePackDefs/Combat_Dismember.xml`, `Ability_Dismember`).
- **A state the pilot carries is a hediff.** The hemogen rush is on while the pilot has
  `HPWC_HemogenRush`: the toggle adds it and asks its comp to stop it, its stage is the buff,
  its comp spends the hemogen and ends it, and the game saves it. Anything that reacts to the
  rush reads the hediff off the pawn.
- **Worn graphics go through the pawn render tree** (`apparel.renderNodeProperties`), never draw
  hooks. The hemogen glow is the model: each piece's def declares a node for its own glow
  sprite, which draws while the pilot has the rush hediff or deathrests, whichever pieces they
  wear. A new
  node keeps to what `Source/1.6/Rendering/` records: portraits and the zoomed-out pawn cache
  bake the tree in child order and ignore per-frame property blocks, so an overlay sits after
  the sprite it covers and anything animated bakes as one fixed state.
- **Light on the glow grid is for a pilot who stays put.** A glower that moves has the grid
  rebuilt around it at every step, so the rush lights nothing and a deathrest does, through a
  thing spawned on the pilot's cell for as long as it lasts (`ThingDefs_Misc/DeathrestLight.xml`).
- **Art is `Textures/Things/Pawn/Warcasketlike/WarcasketHemophage/`**, VFEP's layout and file
  naming: `WarcasketHemophage`, `...Shoulders`, `...Helmet`, each with `_north`, `_east`,
  `_south`, a mask beside every one of those (`_northm`) and a bare-named copy of the south
  facing and its mask (`_m`) for the item icon. A new sprite keeps the facing suffix last
  (`<Name>_south.png`), or `Graphic_Multi` cannot resolve it. `Tests/1.6/SetShapeTests.cs`
  fails on a declared graphic with a missing file.
- **The parts declare no `apparel/tags`**, which keeps them out of every pawnkind's gear roll;
  VFEP's slot fill, which reads no tags, is answered in
  `Source/1.6/Patches/PawnGenerator_GeneratePawn_Patch_Postfix.cs`. The armor def's header has
  why; a patch that adds a tag (Save Our Ship 2's `EVA`) adds the node with it.
- **A protection a piece gives its wearer is a `DefModExtension` flag on the piece**, read off
  the worn apparel by the patches that answer the game's own checks. The helmet's
  `SunlightShieldExtension` is the model, with `PyrophobiaReliefExtension` beside it: it names
  no gene, so another mod's sunlight gene built on the game's classes is covered without a
  compat root.
- **Tunables are def fields** the mod settings write at startup (`Source/1.6/CLAUDE.md`); the
  XML value is the default and the header stays the record. The pilot's trait
  (`TraitDefs/WarcasketTrait_Hemophage.xml`) is the exception its header explains: its work
  fields are written from VFEP's trait at startup, so its XML is a readable copy, not a dial.

## Languages

- `Languages/English/Keyed/HemophageWarcasket.xml` is the one home for every string printed
  from code, under the `HPWC_` prefix; def text reaches translators as DefInjected. Biotech's
  own Keyed strings may be printed directly: the DLC is a hard dependency, so its keys always
  exist.
- `WarcasketDef` has no def database of its own: the game injects it under `ThingDef`. A
  `DefInjected/WarcasketDef/` folder would never load.
- Translation passes, the checker and the sidecar are described in `Scripts/CLAUDE.md`.

## Optional content and gating

- **Gate by load root, not by attribute.** `MayRequire` works on def roots and list items, but the
  DefInjected loader ignores attributes, so a gated def's injection in the main tree errors
  ("found no def named ...") whenever the gate is closed. Gated content ships from a compat root
  listed with `IfModActive` in `LoadFolders.xml` (`Mods/VanillaGravshipExpanded` is the model):
  `1.6/Mods/<Mod>/` for version-specific content, root-level `Mods/<Mod>/` for art. The def's own
  `MayRequire` is then redundant and dropped. `MayRequire` stays right for a stat leaf whose
  stat a DLC defines (`SlaveSuppressionOffset`, `VacuumResistance`): it drops a number, never a
  def or a key. Biotech needs neither: it is a hard dependency.
- **A patch in a gated root, or behind a `PatchOperationFindMod` gate, fails silently for
  everyone but that mod's players**, so `Tests/1.6/CompatPatchTests.cs` resolves every patch's
  xpaths against the main tree; keep every xpath to one node per `defName` it names, or zero
  for a `PatchOperationConditional` probe. A root that patches another mod's or a DLC's def
  needs that def's XML merged into the test (the Overlord repo's `DlcRoots`).
- **A patch that branches on a mod setting** wraps its operations in
  a `PatchOperationSetting` (the Overlord and Toxin repos' `Source/1.6/PatchOperations/`); a
  change then takes a restart, and the smoke test, which boots on default settings, never runs
  its `match` branch.
- **Compat roots sit beside the well-known folders, never inside them.** Anything under
  `1.6/Defs/**` or `1.6/Languages/**` loads unconditionally at any depth.
- **An injection is gated by the root that contains it, never by the def it targets.** A main-tree
  def's translation placed in a compat root silently vanishes when the gate closes.
- **A compat root's language files never reuse a main-tree language-relative path**
  (`DefInjected/<Type>/<File>.xml`, `Keyed/<File>.xml`): the game dedups per mod by that path and
  silently skips one whole file. Suffix the filename with the gate's name
  (`Apparel_VanillaGravshipExpanded.xml`).
- **No two load roots share a root-relative path under `Patches/` or `Defs/`**
  (`Patches/ApparelStats.xml` in two compat roots): the game loads one XML file per such path
  per mod, the later-listed root's, and silently drops the other (`DirectXmlLoader.
  XmlAssetsInModFolder`, 1.6 and 1.7). `Tests/1.6/CompatPatchTests.cs` fails on a shared path.
- **A compat art variant goes at the main tree's exact texture path** inside the gated root, not
  under a new name behind a patch. A later-listed load folder wins a shared path, so the gated
  root stays below `/` in `LoadFolders.xml`.
- **Gate on the package id** (`vanillaexpanded.gravship`), never `PatchOperationFindMod` (display
  names), unless the patch mirrors an upstream patch and copies its gate so both fire together
  (`Patches/SOS2Patch.xml`).
- **A compat root that adds a def or a translatable field gets a world in the sidecar**
  (`Scripts/CLAUDE.md`). The three roots today add neither: they move numbers and fill def
  fields the main tree leaves empty, so the sidecar is one boot.
