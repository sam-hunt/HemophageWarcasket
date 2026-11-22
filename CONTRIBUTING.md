# Contributing

Bug reports, fixes, and features are welcome. Open an issue or pull request.
Build instructions are in CLAUDE.md; the mod builds with
`dotnet build HemophageWarcasket.sln -c Release`.

If you work in Claude Code, the repo ships a Stop hook (`.claude/hooks/sync-mod.sh`,
wired by `.claude/settings.json`) that rebuilds and redeploys the mod into your
RimWorld Mods folder after any turn that changed mod files. It does nothing when no
RimWorld install is found. Like any script in a repo you clone, read it before you
let an agent run it.

## Localization

The mod targets the languages below, chosen by RimWorld's per-language
audience size. Contributions for any other language RimWorld supports are
welcome too.

| Language             | Status           | Credit    |
| -------------------- | ---------------- | --------- |
| English              | Source           | -         |
| Simplified Chinese   | Machine-assisted | -         |
| Russian              | Machine-assisted | -         |
| Korean               | Machine-assisted | -         |
| German               | Machine-assisted | -         |
| Spanish              | Machine-assisted | -         |
| French               | Machine-assisted | -         |
| Brazilian Portuguese | Planned          | -         |
| Japanese             | Planned          | -         |
| Traditional Chinese  | Planned          | -         |

Statuses: **Source** (the authoritative English strings), **Machine-assisted**
(generated with terminology grounded against the official RimWorld
localization; awaiting native review), **Native** (written or reviewed by a
native speaker), **Planned** (not started, contributions welcome).

Spanish here means Castilian (RimWorld's `Spanish` language folder). RimWorld
also ships a separate Latin American Spanish (`SpanishLatin`); a translation
for it is welcome as its own folder rather than as edits to this one.

Brazilian Portuguese likewise means RimWorld's `PortugueseBrazilian` folder.
European Portuguese (`Portuguese`) is a separate language folder in RimWorld,
so a translation for it is welcome in its own right rather than as edits to
this one.

### Contributing a translation

- The text has two parts. Everything the code prints is the one Keyed file,
  `1.6/Languages/English/Keyed/HemophageWarcasket.xml`, translated to
  `1.6/Languages/<Language>/Keyed/HemophageWarcasket.xml` with the same keys;
  the comments in the English file say where each string appears and what
  its placeholders hold. The defs' text is DefInjected: the key set comes
  from `Scripts/expected-injections.json` (every entry marked `required`),
  and each def type has its own folder under
  `1.6/Languages/<Language>/DefInjected/` (`ThingDef` for the three
  warcasket parts, which are `VFEPirates.WarcasketDef`s the game files under
  `ThingDef`, then `VEF.Abilities.AbilityDef`, `HediffDef`, `JobDef`,
  `DamageDef`, `RulePackDef` and `TraitDef`). An existing language's folder
  shows the layout.
- Every translated entry carries the current English source in a comment
  directly above it, e.g. `<!-- EN: hemophage warcasket -->`; this is how
  stale translations are detected when the English changes.
- The three part descriptions share their second and third paragraphs; keep
  them identical across the three entries, and keep each part's
  `shortDescription` equal to its description's first paragraph, as the
  English does.
- Hemogen, sanguophage and the other Biotech terms follow RimWorld's
  official translation for the language, in the same plain words. The mod's
  own vocabulary for each language (how "warcasket", the set's name, the two
  abilities and the foundry are rendered, and which community VFE Pirates
  translation it follows) is recorded in
  `.claude/skills/translate/glossary/<Language>.md`; match it, or change it
  there in the same PR.
- The game counts two nouns in the settings and in Dismember's tooltip, the
  strike and the component a strike is reloaded with ("2 strikes",
  "1 component"), and forms their plurals itself. Where a language needs
  help with that, the forms are in `1.6/Languages/<Language>/WordInfo/`;
  the glossary's "Charge nouns" section says which files a language ships
  and why.
- The Workshop page text lives in `.steamworkshop/Description/<Language>.txt`
  (see that folder's README for the format); corrections there are welcome
  too.
- Formatting: UTF-8 without BOM, LF line endings, 2-space indent.
- Validate before opening a PR:

  ```bash
  python3 Scripts/check-translations.py --strict
  ```

  It checks key coverage, placeholders, DefInjected paths, staleness, and
  file hygiene. The checker's engine lives in the `l10n/` git submodule, so
  clone with `git clone --recurse-submodules` (or run
  `git submodule update --init` in an existing clone) before validating.

- Improving a machine-assisted language? Corrections from native speakers
  are gladly merged, no matter how small.
