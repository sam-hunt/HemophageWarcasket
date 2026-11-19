# CLAUDE.md

Guidance for Claude Code when working in this repository. This file holds only what applies
across the whole repo. Folder guides load on their own when work touches them: `1.6/CLAUDE.md`
(defs, patches, languages, gating), `Source/1.6/CLAUDE.md` (C#), `Scripts/CLAUDE.md` (l10n and
release tooling), `.steamworkshop/CLAUDE.md` (Workshop page).

## Project

**Hemophage Warcasket** is a RimWorld 1.6/1.7 mod adding one warcasket apparel set (armor,
shoulder pads, helmet) for Vanilla Factions Expanded - Pirates (VFEP), on VFEP's
10th-generation spacer chassis: a suit for a sanguophage, and more generally for any pilot with
Biotech's `Hemogenic` gene. VFEP, Biotech and Harmony are the hard dependencies (VEF comes with
VFEP); every other DLC or mod is optional. C# (.NET Framework 4.7.2), Harmony, XML defs. Def
prefix `HPWC_`.

**Design direction: a suit that runs on its pilot's hemogen.** Each piece carries one mechanic
built on Biotech's hemogen and the sanguophage's other genes: the armor a toggled ability that
spends hemogen quickly for a burst of speed and strength, with glowing parts on every worn piece
of the set while it runs; the shoulder pads an ability that destroys a target's limb and, for a
hemogenic pilot on a humanlike target, drains hemogen from it, on a small stock of strikes
reloaded with components; the helmet protection from sunlight for a pilot with a sunlight
sensitivity gene, a steadier nerve near fire for one with the pyrophobia gene, and the set's
heaviest plating. A pilot without the gene gets a plain spacer
warcasket. Any pilot keeps intellectual work, which VFEP's warcaskets block.

**Fiction: nobody's official program.** The siblings' sets come from pirates welding themselves
in or from a government program entombing its veterans. This one is quieter: sanguophages who
had worked their way into that program had the pattern built in secret, for themselves, to make
the nearly deathless harder still to destroy. Player-facing text tells it as rumor ("Some
believe ..."), the way Biotech tells its own sanguophage lore, and never as settled history.

**State: all three mechanics in, not yet released.** The three pieces are in on the tier's
baseline stats and costs with their commissioned art, but for the heat plating and the
devilstrand that pays for it, the pads' cost, which pays for Dismember, and the helmet's blunt
plating and cost, and the SOS2, VGE and CE patches mirror them. The armor's hemogen rush is
built, with its glow on all three pieces, and so are the pads' Dismember, the helmet's sunlight
protection and pyrophobia relief, the pilot's own work trait and the settings
window, with a section for each piece's mechanic and one for the pilot's work. The descriptions
and the player-facing pages are first cuts still owed a full review. `TODOs.md` is the ledger.
The sibling repos (same author, same VFEP base) are the worked precedents for everything not
here yet: `../ToxinWarcasket/` for a toggled ability, a gauge and the Biotech dependency,
`../ShipcrackerWarcasket/` for render-tree graphics and a gated art root, `../MedicWarcasket/`
and `../OverlordWarcasket/` for the settings' sliders and switches, `../LegionWarcasket/` for a
multi-world sidecar.

## Documentation rules

- **The file's header is the record.** Per-item values, tuning numbers and decompile-verified
  call paths live in the header comment of the file they describe, with the _why_. No guide
  restates a def value or call path; a line joins a guide only when it constrains other files.
- **Comments describe the present, git describes the past.** No dates, previous values, dropped
  attempts or "seen in game" notes in comments; that history goes in the commit message.
  Comments never ship: `StageMod` strips every comment node from the staged XML, so a def header
  can carry its full rationale at no download cost.
- **Design specs go in `Docs/Specs/`**, one file per feature, when a feature is designed before
  it is built (most of the siblings' abilities have one). Never read a spec whole unprompted: specs
  run long and the code supersedes them. Grep for the section you need, or delegate a spec-wide
  question to a subagent and keep its answer.
- **Dependency sources are read-only.** `../VanillaExpanded/VanillaFactionsExpanded-Pirates/`
  (its `CLAUDE.md` maps the warcasket subsystem) and
  `../VanillaExpanded/VanillaExpandedFramework/` are upstream checkouts; never commit into them.

## Build

```bash
dotnet build HemophageWarcasket.sln -c Release   # builds 1.6/Assemblies/ and redeploys to Mods/HemophageWarcasket
```

Compiles against the pinned `Krafs.Rimworld.Ref` 1.6 package; the local install (auto-detected,
or `RIMWORLD_PATH`, in WSL the Windows path) only decides deployment and serves as the decompile
target. Release builds deploy atomically through the csproj's `StageMod` target, the one file
manifest, which CI calls too; Debug builds never deploy. The csproj's comments cover path
detection, the compile-only VFEP/VEF references and the manifest's globbing. The manifest globs
the working tree, not the git index: an untracked file under a content folder deploys locally
and is absent from a CI release. The Stop hook `.claude/hooks/sync-mod.sh` (self-documenting,
byte-identical across the mod family) rebuilds after any turn that changed mod files and feeds a
failing build back to the agent. **Warnings are build errors** (`TreatWarningsAsErrors`,
severities pinned in `.editorconfig`): fix the code, not the severity.

`Tests/1.6/` is a headless xUnit (net472) suite, never shipped: `dotnet test
Tests/1.6/HemophageWarcasket.Tests.csproj` (vstest hosts it through mono on Linux/WSL). Four
files read only the repo's XML and art, so they run anywhere; `WorkProfileTests`,
`DismemberTargetTests` and `TunableTests` run game code and so need the game's real 1.6 runtime
DLLs (`RIMWORLD_MANAGED_1_6`, a 1.6 install, or the frozen copy at
`~/.local/share/RimWorld-1.6/Managed`; the csproj documents the lookup), failing without them
while the build stays green. The first holds the work trait's rule: which work types the
settings may open, their defaults, and the tags and types a choice writes. The second holds
which part a Dismember strike destroys, the third the settings sliders' arithmetic.
`CompatPatchTests` resolves every xpath in `1.6/Mods/*/Patches/` and the SOS2 patch against the
main-tree defs, the only check a gated root gets short of a boot with that mod active, and
fails on two load roots sharing a patch or def path;
`SetShapeTests` holds each piece's declared graphics to files that exist under `Textures/` (a
missing one is a pink box in game and an error nowhere), the descriptions to their shared
paragraphs and `About.xml` to no `modIconPath` (the helmet's settings header draws the mod icon,
which only the `About/ModIcon.png` load keeps alive across a play-data reload); `RulePackTests` holds each combat log rule pack to the symbols its log entry
supplies (an unknown one is an error only when the line is first written in game);
`SettingsTargetTests` holds the ability defs, the pads' strike pool and the helmet's two
extensions to the fields the settings write (a missing one drops those sections from the window
with one warning).

## Layout

```
About/            Mod metadata, ModIcon.png, Preview.png
1.6/              Defs, Patches, Mods/<compat roots>; serves 1.6 and 1.7
Source/1.6/       C# (net472)
Textures/         Art, version-independent: pawn art under VFEP's Warcasketlike/ layout
Tests/1.6/        xUnit net472 suite                        (not shipped)
Scripts/          l10n shims, the sidecar, the smoke test and the preview GIF build (not shipped)
l10n/             rimworld-l10n toolkit, pinned submodule   (not shipped)
Docs/Research/    VFEP warcasket stat tables and ability mechanics (copied from the siblings),
                  and the preview GIF transcode's measurements
.steamworkshop/   Workshop page text and images             (not shipped)
LoadFolders.xml   Load roots per game version, compat roots gated by package id
```

Not here yet, with the place each takes when it lands: `Docs/Specs/`, and a
`1.6/Mods/CombatExtended/CLAUDE.md` once Combat Extended has been scanned for what it changes
about hemogen (the Overlord repo's is the model).

## Cross-cutting rules

- **One def per file**, named after the def with the `HPWC_` prefix stripped.
- **Log with the `[Hemophage Warcasket]` prefix.** Root namespace `HemophageWarcasket`.
- **Biotech is a hard dependency, by decision.** The set exists for the hemogenic gene, and its
  mechanics are built from Biotech's defs. Biotech content goes in the main tree like everything
  else; there is no Biotech load-folder gate and references to Biotech defs need no
  `MayRequire`.
- **A mechanic keys on the gene, never on the xenotype.** "Hemogenic" means the pawn has an
  active `Hemogenic` gene (`pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>()`, as Biotech's own
  `GeneUtility.OffsetHemogen` finds it; verified on 1.6); a custom xenotype or a pawn with an implanted xenogerm qualifies exactly as a sanguophage
  does, and nothing may test for `XenotypeDefOf.Sanguophage`. The fiction says sanguophage; the
  rules say hemogenic.
- **Vanilla Races Expanded - Sanguophage is optional content, gated by load root.** Anything
  that reads its genes, hediffs or research goes in a compat root of its own
  (`1.6/Mods/<Mod>/`, `IfModActive` on its package id), never in the main tree behind
  `MayRequire`; `1.6/CLAUDE.md` has why.
- **Player-facing text follows `l10n/languages/English.md`** (def labels and descriptions,
  `Keyed/`, `About.xml`, `.steamworkshop/`): read it before writing or changing any. Its
  mechanical rules (no dashes, semicolons, "Whether" openers or parenthetical asides) are
  checked by `Scripts/check-translations.py` as `[player text]` warnings, which `--strict` turns
  into release errors. Code comments, def comments and these guides are unaffected. American
  spelling, as vanilla uses. Biotech's own words for its mechanics are the vocabulary: hemogen,
  hemogenic, sanguophage, deathrest, all lowercase in running text as Biotech writes them.
- **Settings override def fields, never code paths**, written onto the defs at startup and on
  window close, so defaults come from the XML and the def header stays the record.
  `Source/1.6/CLAUDE.md` has the shape and the sibling files the sliders come from.
- **English is the source of truth for text.** Translations come from the `/translate` skill,
  one language at a time, shortly before a release and never earlier.
- **`.claude/` is partly tracked** (`skills/`, `hooks/`, `settings.json`; the rest is personal).
  Editing a skill is a team-visible change that must keep in step with what it automates.
- **Shared infrastructure is mirrored, never forked.** The csproj's build and deploy logic, the
  Stop hook, the release workflow and the skills' family-wide steps are the template's and the
  siblings'; change them there first (`../RimworldModTemplate/`, or the most exercised sibling)
  and copy the change here.

## Game versions

The mod is a dual-version ("Phase A") mod like its warcasket siblings: one `1.6/` tree and one
net472 assembly, loaded by both 1.6 and 1.7, with `LoadFolders.xml` naming the tree under each.
`../RIMWORLD_1.7_MIGRATION.md` is the family's guide to what 1.7 is and when a mod splits or
goes 1.7-only; do not add a `1.7/` folder or retarget the project outside that plan.

## Releases and debugging

Run the `/release` skill. It encodes the CHANGELOG layout, the version scheme (`X.Y.Z-rc.N`
candidates are CHANGELOG-less and Workshop-less prereleases) and the pre-release checks;
`.github/workflows/release.yml` fails a stable release whose CHANGELOG section is missing.

The `rimworld-logs` skill finds `Player.log` on 1.6 and 1.7 and disassembles the game, VFEP and
VEF assemblies; the local install's assembly, now the 1.7 one, is the source of truth over the
Krafs package. In game, VFEP and Vanilla Expanded Framework must load before this mod, which
appears as `HemophageWarcasket` in the list.
