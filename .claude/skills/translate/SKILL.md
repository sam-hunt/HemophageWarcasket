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
generating translations is not, until the English text is final.

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
  language, skim otherwise. `l10n/process.md` § Register applies with full
  force here: the descriptions are lore paragraphs with no vanilla sentence
  to mirror, so the register read of every description, tooltip and
  Workshop sentence, by a fresh subagent that sees only the target text and
  the language file's "Generated-text tells" section, is part of every
  pass, and a pass that finds that section still a stub fills it from the
  tar and commits it upstream.
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

- **Two surfaces plus the Workshop page.** The defs' English is served by
  the def XML's own fields (DefInjected, below). Everything code prints is
  the one Keyed file, `1.6/Languages/English/Keyed/HemophageWarcasket.xml`,
  translated to `1.6/Languages/<Language>/Keyed/HemophageWarcasket.xml` with
  the same keys. Each group's comment in the English file says where its
  strings appear and what their placeholders hold; read it with the key.
  - Dismember's runtime strings. `HPWC_DismemberNoLimbs` and
    `HPWC_DismemberNotHostile` (rejection messages) take `{0}`, the
    target's capitalized short label, a name or a kind such as "Thrumbo".
    `HPWC_DismemberWillWound` and `HPWC_DismemberMayWound` are drawn beside
    the cursor where Core's `WillKill` goes: shape them like it.
    `HPWC_DismemberConfirmKill` and `HPWC_DismemberConfirmDrainKill` are
    the dialog before a lethal order and mirror Biotech's
    `WarningPawnWillDieFromBloodfeeding`, tokens and closing question
    alike, so each language builds them on its own rendering of that key.
    `HPWC_DismemberReload` (the ability tooltip's last line) takes `{0}`, a
    counted reload item (next bullet).
  - the mod settings window: `HPWC_SettingsCategory` (its header and the
    Workshop title), `HPWC_SettingsUnavailable`, the value words
    (`HPWC_DefaultSuffix`, which keeps its leading space, `HPWC_Enabled`
    and `HPWC_Disabled`, lowercase because they are a tooltip's `{0}`,
    `HPWC_Off`, the two `...None`), the section headers
    (`HPWC_SectionRush`, `HPWC_SectionDismember`, `HPWC_SectionHelmet`,
    `HPWC_SectionWork`, `HPWC_SectionQuestPilot`, the last three with a
    `...Desc` tooltip; the first two show the ability's own label and
    description as theirs, so `HPWC_SectionRush` and
    `HPWC_SectionDismember` are the two abilities' labels, capitalized as
    a header), and each row's label and `...Desc` tooltip. Labels carry no
    unit: the row's value shows it. Every tooltip ends on `Default: {0}.`,
    the default as the row shows it; `HPWC_DismemberReloadCostDesc` also
    takes `{1}`, the reload item's label pluralized by the language worker.
    The summaries are whole sentences, one key per combination of rows at
    zero, so no language joins two: the rush's four (`HPWC_RushSummary*`),
    Dismember's four (`HPWC_DismemberSummary*`, whose `{5}` and `{6}` are
    counted nouns, next bullet), the work section's three
    (`HPWC_WorkSummary` takes `{0}`, the open work types' short labels as
    the work tab's columns name them, lowercase, joined by the game's
    `ToCommaListAnd`). Times arrive with vanilla's unit letter attached
    (`LetterSecond`, `LetterHour`, `LetterDay`), percentages with their
    sign. `HPWC_QuestMeeting` and `HPWC_QuestShip` are subheadings that
    name Biotech's two sanguophage quests: ground them on the language's
    own text for those quests (`SanguophageMeetingHost`,
    `SanguophageShip`), and "master" and "thrall" on its
    `SanguophageThrall` kind and the quest's wording.
  Name the set's things with the same renderings as the DefInjected text
  (hemogen rush, dismember, strike, the injectors, the servos, warcasket).
- **Counted nouns reach the language worker.** `Ability_Dismember.Counted`
  prints a number, a space and a label, pluralized by
  `LanguageWorker.Pluralize(label, count)` from two up: the reload item
  (Core's `ComponentIndustrial` label, in the game's own translation) in
  `HPWC_DismemberReload` and as `{6}` of the Dismember summaries, and the
  charge noun "strike" as their `{5}`. `HPWC_DismemberReloadCostDesc`'s
  `{1}` is the same item's label pluralized with no count. So each
  language owes three things, all recorded in its glossary's "Charge
  nouns" section: a strike noun the worker pluralizes correctly, or a
  `1.6/Languages/<Language>/WordInfo/plural.txt` line for it (and, in
  Russian, `case.txt` rows for both numbers, which the worker reads to
  choose the form after a number); a `plural.txt` line for Core's
  component label where the worker's fallback mangles it (German's
  `Bauteil`, Brazilian Portuguese's `componente`, which the base worker
  returns unchanged); and sentences that take the counted noun in its bare
  citation form, after a colon or as a subject or direct object, never in
  a slot that would decline it. French's worker never reads the table, so
  the French strike noun is a single word with a regular plural. Japanese,
  Korean and Chinese get the label unchanged. The checker ignores these
  files (`l10n/lessons.md` has the decompile-verified worker map; the
  Medic and Toxin Warcasket glossaries are the worked examples). Vanilla
  also prints the charge noun through `{CHARGENOUN_plural}` and
  `{lookup: ...}` in five Keyed strings of its own, and German and
  Castilian switch on its gender there, so a feminine or neuter noun adds
  a `WordInfo/Gender/` line.
- **Enumerate the key set from `Scripts/expected-injections.json`, never
  from a Languages folder or by scanning `1.6/Defs/`.** The sidecar is a
  dump of what the live game walks; regenerate it (game closed) with
  `python3 Scripts/refresh-translation-expectations.py` whenever the checker
  reports it stale, and by hand after a change to the trait's text, which
  that check does not read. Take the English source text for each
  `<!-- EN: -->` comment from the sidecar's `english` field.
- **Def type folders.** Never translate or place a non-`required` sidecar
  entry (texture and icon paths, the render nodes' `debugLabel`) in any
  language file.
  - `DefInjected/ThingDef/` for the three `VFEPirates.WarcasketDef`s
    (`HPWC_Warcasket_Hemophage`, `HPWC_WarcasketShoulders_Hemophage`,
    `HPWC_WarcasketHelmet_Hemophage`): `label`, `description`,
    `shortDescription` (a VFEP field shown in the foundry's part picker;
    translate it like any other), and on the shoulders the strike pool's
    `comps.CompStrikePool.chargeNoun` ("strike") and
    `comps.CompStrikePool.cooldownGerund` (vanilla's default "on
    cooldown", which the def does not set but the probe dumps; the
    language file's main table has the family rendering). The walker keys
    a comp by its comp class. The game rolls a def type without its own
    database into its base and the checker maps the element tag via
    `DEF_TYPE_ALIASES` in `Scripts/check-translations.py`; a
    `WarcasketDef` folder would never load.
  - `DefInjected/VEF.Abilities.AbilityDef/` (the full type name; it is its
    own def database, so no alias) for `HPWC_HemogenRush` and
    `HPWC_Dismember`: `label`, `description`, `jobReportString` and
    `verbProperties.label` (VEF copies the label into the verb it
    generates for every ability; render it identically to `label`). The
    rush's `jobReportString` is VEF's own `Using ability: {0}`, whose
    family rendering is in the language file's main table.
  - `DefInjected/HediffDef/` for `HPWC_HemogenRush`, the running rush:
    `label` (the ability's, identically) and `description`, which follows
    the language's hediff descriptions ("This person's ...").
  - `DefInjected/JobDef/` for `HPWC_Dismember`: `reportString`, shown in
    the pilot's inspect pane while walking up. `TargetA` is replaced by
    the target's label and must survive verbatim; lowercase and
    punctuated as the language's own job reports are.
  - `DefInjected/DamageDef/` for `HPWC_Dismember`: `label` (the noun, as
    the health tab names what destroyed a part) and `deathMessage`, whose
    `{0}` is the victim. Shape both on the language's Core damage defs.
  - `DefInjected/RulePackDef/` for `HPWC_Combat_Dismember`:
    `rulePack.rulesStrings`, the combat log line, a `<li>` list with the
    English list in its `<!-- EN: -->` comment as `<li>` lines
    (`../UniqueMeleeWeapons/1.6/Languages/<Language>/DefInjected/RulePackDef/`
    is the family's format precedent). Every line keeps its
    `r_logentry(...)->` head with its constraint and weight untouched and
    is a whole sentence, so it may be reordered freely, and the list may
    change length (`fullListAllowed`) as long as both values of
    `recipient_part0_destroyed` keep at least one line. The symbols the
    entry supplies are the `INITIATOR_` and `RECIPIENT_` pawn symbols and
    the part's three forms, `recipient_part0_label`,
    `recipient_part0_definite` and `recipient_part0_indefinite`: a body
    part label in the game's own translation, bare or behind the article
    the language worker gives it, which never inflects further. Ground the
    grammar around it, and the possessive, on the language's Core combat
    packs (`l10n/lessons.md` on the possessive symbol), and copy into the
    list any helper rule those packs lean on that lives in an include this
    pack does not have, as the German list does. The part is a limb or a
    head, so no line may assume which.
  - `DefInjected/TraitDef/` for `HPWC_WarcasketTrait_Hemophage`:
    `degreeDatas.warcasket.label` and `degreeDatas.warcasket.description`
    (the walker keys the degree by its label's slug). The label is VFEP's
    own trait label, `warcasket`, so it renders exactly as VFEP's does in
    the language, with no set name. The description is VFEP's trait text
    with the work clause changed, which Medic and Overlord Warcasket
    already ship in every language: take their rendering verbatim up to
    the last clause and end it on the language's Core `Intellectual` work
    type. Nothing else on the trait is text.
- **The descriptions share paragraphs verbatim.** All three share their
  second paragraph (the set's own telling of the 10th generation, which
  replaces VFEP's; the armor def's header says why) and their third (the
  series paragraph); only the first differs per piece. Keep the shared
  paragraphs byte-identical across the three defs in every language, and
  keep each def's `shortDescription` identical to its description's first
  paragraph, as the English does (`Tests/1.6/SetShapeTests.cs` holds the
  English to this). The first paragraph opens on the family's spacer-tier
  opener up to its stat clause, which is this set's own. The second
  paragraph tells the set's origin as two rumors ("Some think ...", "Some
  believe ..."), the way Biotech's sanguophage xenotype description tells
  its own ("Some believe sanguophages secretly direct ..."): render the
  rumor frame with the words that language's official Biotech translation
  uses there, and never as settled fact. Paragraph breaks are the literal
  two-character `\n` sequences the def XML uses.
- **What the text may say of hemogen is fixed by the English.** A
  hemogenic's hemogen is in stores in the body, never "in the blood";
  blood is what a target loses. The pilot "drains hemogen from the wound",
  words that say where and not how, and no surface names Bloodfeed, fangs,
  biting or feeding (they are the bloodfeeder gene's, and any hemogenic
  pilot drains). A translation keeps each of these and does not sharpen
  "drain" into the language's word for Biotech's bloodfeeding.
- **The compat roots carry no strings.** `1.6/Mods/VanillaGravshipExpanded/`,
  `1.6/Mods/CombatExtended/` and
  `1.6/Mods/VanillaRacesExpandedSanguophage/` ship only patches with no
  translatable fields (`1.6/Patches/SOS2Patch.xml` is a main-tree patch).
  Everything lands in the main `1.6/Languages/<Language>/` tree today. If a
  gated root ever gains a labelled def, its DefInjected must move into that
  root's own `Languages/` with a gate-suffixed filename (see
  `1.6/CLAUDE.md`'s gating section), never the main tree.
- **Workshop page:** `.steamworkshop/Description/<Language>.txt`, per
  `l10n/workshop.md` and the folder's own `README.md` and `CLAUDE.md`. The
  title's anchor term is "warcasket"; every localized title must contain
  the rendering of "warcasket" recorded in that language's glossary. The
  title is coupled to the Keyed `HPWC_SettingsCategory` (the settings
  window's header, `WORKSHOP_TITLE_KEY`), so translate the two together. A
  language whose description is still owed is a checker warning, an error
  under `--strict`.

## This mod's grounding domain

Domain mod: **Vanilla Factions Expanded - Pirates (VFEP)**, plus vanilla
Core and Biotech. **VFEP ships English only**, so its warcasket vocabulary
is not available from VFEP itself for any other language. For each target
language, check first whether a community "Vanilla Expanded" translation
covers VFEP in that language and ground terms against it; where none does,
coin the term and record it in `glossary/<Language>.md` rather than
inventing silently at translation time. `l10n/languages/<Language>.md`'s
VFE Pirates section already records that search per language, with the
pack it found or the absence of one; start there (see `glossary/README.md`).

Terms that MUST be grounded before use:

- from VFEP's own vocabulary: "warcasket" itself, "warcasket foundry",
  "shoulders" / "pauldrons", "helmet", "shell", "pilot", "entomb", and
  "lesser warcaskets" for VFEP's plain sets;
- from vanilla Core (ground against the Core tar per `l10n/process.md`):
  apparel terms (armor, helmet), spacer tech, component, servo, the body
  part words (limb, arm, leg, head, heart) and "blood pumping" (the
  capacity's label), blood loss, the blood filth, wound, melee damage and
  move speed (the stats' labels), casting time, cooldown, reload, hostile,
  faction, the work tab and work types (`Intellectual`), research, threat
  points, colony wealth, storyteller, raid, quest, pain shock, and the
  combat log's verbs for a destroyed part;
- from vanilla Biotech (ground against the Biotech tar): hemogen, the
  `Hemogenic` gene and "hemogenic" as the description of a pawn (Biotech's
  `AbilityDisabledNoHemogenGene` has it), hemogen craving, sanguophage,
  thrall, the sanguophage meeting and crashed shuttle quests, the UV
  sensitivity genes (`UVSensitivity_Mild`, `UVSensitivity_Intense`), the
  pyrophobia gene (`FireTerror`) and the mental state it causes, sunlight,
  deathrest, xenotype. Where the language gives the sanguophage xenotype
  and sanguophages in general two words (Simplified Chinese does), take
  each as the Biotech text at the same slot has it;
- this mod's own vocabulary, coined once and recorded in the glossary:
  "hemophage", the set's name, a coinage beside Biotech's official
  "sanguophage" and "hemogen" and so derived from that language's official
  Biotech terms; hemogen rush (the ability, its hediff and the settings
  section); dismember (the ability, verb and section) and dismemberment
  (the damage); strike (the charge noun and the settings' word for one use
  of Dismember); drain (hemogen, from a wound or a target); the injectors
  and the servos; hemogen reserve; "Hemophage series".

**Blood is written plainly in every language, Chinese included.** Vanilla's
own Simplified and Traditional Chinese name blood, blood loss, bloodfeeding
and hemogen in plain words (血原质, 失血, 汲血; 血原, 失血, 吸血), and so
does this mod: never euphemize, soften or swap out a blood term the
official translation uses. The rule in mainland China that has publishers
recolor or remove blood is a licensing review of games sold there, which
reaches neither the game's international release nor a Workshop mod's text.

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
