---
name: translate
description: Generate, update, or audit mod localization (DefInjected plus one Keyed file) for a target language, grounded in Vanilla Factions Expanded - Pirates warcasket terminology plus vanilla Core and Biotech apparel and gene terminology for Hemophage Warcasket's single warcasket set. Use when asked to add a language, update translations, or check translation freshness.
argument-hint: "[language, e.g. German | update | check]"
---

# Translate

Produce or refresh localization files for Hemophage Warcasket. English is the
source of truth; every other language derives from it.

**Translation passes are deferred until shortly before release** and run
only on explicit request, one language at a time (see `l10n/process.md`'s
non-negotiables). Generating the key expectations sidecar is always fine;
generating translations is not, until the English text is final. Today it
is not: the full review of the English text is still open (`TODOs.md`).

**The family-wide process lives in the `l10n/` submodule, load these first,
and only these** (progressive disclosure; if `l10n/` is empty, run
`git submodule update --init`):

- `l10n/process.md`, non-negotiables, file/format conventions, terminology
  grounding method, and the generation / update / audit workflows. This is
  the workflow authority; follow it step by step.
- `l10n/languages/<Language>.md`, the target language's engine mechanics,
  style rules, and vanilla-grounded common vocabulary. Read ONLY the target
  language's file.
- `glossary/<Language>.md` (beside this file), this mod's own coined-term
  table for the target language. Read it in the same pass. Every file is
  seeded with the charge-noun constraint and the list of terms the first
  pass must still ground or coin; the pass fills that list in and adds the
  set-specific rows. The VFEP warcasket vocabulary (warcasket, parts,
  foundry, entomb, VFEP's set names and research labels, the spacer openers
  and the 10th-generation paragraph, the VEF ability strings) is the
  family's and lives in `l10n/languages/<Language>.md`, under its "VFE
  Pirates warcasket domain" section and in its main table, so every
  warcasket set reads as one family in-game. A pass never reopens one of
  those rows here: a change is made upstream first, then every set's
  shipped files follow.
- `l10n/lessons.md`, cross-language lessons; read when generating a new
  language, skim otherwise. Its free-prose register lesson applies with
  full force here: the descriptions are lore paragraphs with no vanilla
  sentence to mirror, so a separate register read of every description and
  Workshop sentence is part of every pass.
- `l10n/workshop.md`, the `.steamworkshop/` conventions, whenever the pass
  touches the Workshop description (every initial generation does).

**Where learnings land:** mod-independent findings (engine mechanics, a
language's grammar rule, corpus style facts) go in the `l10n/` submodule,
edit the canonical checkout at `~/dev/rimworld-l10n`, commit and tag there.
Mod-specific findings (coined terms, phrasing decisions) go in
`glossary/<Language>.md`.

**Before any pass, bump the pin:** run `l10n/tools/bump-consumer.sh` (fetches
upstream's release tags, checks out the latest, commits the pointer as `chore:
Bump l10n submodule vOLD -> vNEW`; no-op when already current). This is one of
the three moments a pin moves (release, pass start, new upstream major), never
per upstream commit. If it reports a MAJOR bump, read the upstream release
notes for the shim or flow edit this repo owes before continuing.

## This mod's translation surface

- **One surface today, plus the Workshop page.** The defs' English is
  served by the def XML's own fields (DefInjected, below). Everything code
  prints will be one Keyed file,
  `1.6/Languages/English/Keyed/HemophageWarcasket.xml`, translated to
  `1.6/Languages/<Language>/Keyed/HemophageWarcasket.xml` with the same keys.
  It does not exist yet: the first code-printed string creates it, and
  `HPWC_SettingsCategory` becomes the Workshop title key then. Each key's
  comment in the English file should say where the string appears and what
  its placeholders hold. There is no settings window yet.
- **Enumerate the key set from `Scripts/expected-injections.json`, never
  from a Languages folder or by scanning `1.6/Defs/`.** The sidecar is a
  dump of what the live game walks; regenerate it (game closed) with
  `python3 Scripts/refresh-translation-expectations.py` whenever the checker
  reports it stale. Take the English source text for each `<!-- EN: -->`
  comment from the sidecar's `english` field.
- **Def type folder.** `DefInjected/ThingDef/` for the three
  `VFEPirates.WarcasketDef`s (`HPWC_Warcasket_Hemophage`,
  `HPWC_WarcasketShoulders_Hemophage`, `HPWC_WarcasketHelmet_Hemophage`):
  `label`, `description`, `shortDescription` (a VFEP field shown in the
  foundry's part picker; translate it like any other), in
  `1.6/Languages/<Language>/DefInjected/ThingDef/Apparel_Hemophage.xml`. The
  game rolls a def type without its own database into its base and the
  checker maps the element tag via `DEF_TYPE_ALIASES` in
  `Scripts/check-translations.py`; a `WarcasketDef` folder would never
  load. Never translate or place a non-`required` sidecar entry (texture
  paths such as `shieldTexPath`) in any language file. The defs are
  parented directly on VFEP's abstract bases; there are no abstract bases of
  our own.
- **One world.** The sidecar is single-world: the refresh shim's
  `engine.CANONICAL_ACTIVE_MODS` pins Biotech and Odyssey alongside VFEP, so
  every key is in the main tree.
- **The descriptions share paragraphs verbatim.** All three share their
  second paragraph (the set's own telling of the 10th generation, which
  replaces VFEP's; the armor def's header says why) and their third (the
  series paragraph); only the first differs per piece. Keep the shared
  paragraphs byte-identical across the three defs in every language, and
  keep each def's `shortDescription` identical to its description's first
  paragraph, as the English does (`Tests/1.6/SetShapeTests.cs` holds the
  English to this). The second paragraph echoes Biotech's sanguophage
  xenotype description ("don't age or die naturally", "Some believe
  sanguophages secretly direct ..."): render those echoes with the words
  that language's official Biotech translation uses there. Paragraph
  breaks are the literal two-character `\n` sequences the def XML uses.
- **The compat roots carry no strings.**
  `1.6/Mods/VanillaGravshipExpanded/` and `1.6/Mods/CombatExtended/` ship
  only patches with no translatable fields (`1.6/Patches/SOS2Patch.xml` is a
  main-tree patch). Everything lands in the main `1.6/Languages/<Language>/`
  tree today. If a gated root ever gains a labelled def, its DefInjected must
  move into that root's own `Languages/` with a gate-suffixed filename (see
  `1.6/CLAUDE.md`'s gating section), never the main tree.
- **Workshop page:** `.steamworkshop/Description/<Language>.txt`, per
  `l10n/workshop.md` and the folder's own `README.md`. The title's anchor
  term is "warcasket"; every localized title must contain the rendering of
  "warcasket" recorded in that language's glossary. The title is not
  coupled to a Keyed key yet (`WORKSHOP_TITLE_KEY` is `None`); once the
  settings window's `HPWC_SettingsCategory` exists, translate the two
  together.

## This mod's grounding domain

Domain mod: **Vanilla Factions Expanded - Pirates (VFEP)**, plus vanilla
Core and Biotech. **VFEP ships English only**, so its warcasket vocabulary
is not available from VFEP itself for any other language. For each target
language, check first whether a community "Vanilla Expanded" translation
covers VFEP in that language and ground terms against it; where none does,
coin the term and record it in `glossary/<Language>.md` rather than
inventing silently at translation time. That search is already recorded per
language upstream, in `l10n/languages/<Language>.md`'s "VFE Pirates warcasket
domain" section; start there (see `glossary/README.md`).

Terms that MUST be grounded before use:

- from VFEP's own vocabulary: "warcasket" itself, "warcasket foundry",
  "shoulders" / "pauldrons", "helmet", "shell" and "pilot";
- from vanilla Core (ground against the Core tar per `l10n/process.md`):
  apparel terms (armor, helmet, shoulder pads);
- from vanilla Biotech (ground against the Biotech tar): hemogen,
  sanguophage, the Hemogenic gene, xenotype;
- this mod's own vocabulary, coined once and recorded in the glossary:
  "hemophage", the set's name. It is a coinage beside Biotech's official
  "sanguophage" and "hemogen", so each language's rendering must be derived
  from that language's official Biotech terms. The abilities' names are not
  final; their terms are grounded when the abilities land.

Grep the tars for just this handful of terms; never extract or read a
whole tar. The vanilla-grounded answers for common words live in
`l10n/languages/<Language>.md`; this mod's own coined terms and VFEP-term
decisions live in `glossary/<Language>.md`.

## Workflows

Follow `l10n/process.md`'s Initial generation / Update pass / Audit-only
workflows verbatim. This mod's specifics on top:

- The checker: `python3 Scripts/check-translations.py` (`--strict` for new
  languages). Sidecar regen: `python3
  Scripts/refresh-translation-expectations.py` (game must be closed; drives
  the deployed L10nProbe, which must have this mod ticked in its settings).
- There is no compat-root routing to do today (see above); everything,
  Keyed included, lands in the main tree.
- The public roster is CONTRIBUTING.md's localization table, update it in
  the same commit as any language addition or native review.
- Machine-assisted passes are run as one Opus subagent per language with a
  bounded brief (the key list, the grounded VFEP and Biotech terms, the
  language file and glossary, the register gate); the lead reviews every
  diff and owns the commit.
