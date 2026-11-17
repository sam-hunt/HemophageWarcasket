# German glossary, Hemophage Warcasket

Seeded before any translation pass; none has run here yet. Only mod-specific
coinages and grounding decisions. The VFEP warcasket vocabulary (warcasket,
the parts, the label pattern, foundry, entomb, VFEP's set names and research
labels, the spacer-tier openers and the 10th-generation paragraph) lives
upstream in `l10n/languages/German.md` under "Grounded common vocabulary, VFE
Pirates warcasket domain", with its grounding source and what is pending
native review; the settings and ability words the family settled sit in that
file's main table. The set's descriptions take the family's spacer-tier
opener from there; the generation paragraph is this mod's own, not the
upstream one, and with the series paragraph and what follows the opener in
each first line it is this mod's prose. Nothing here
restates the upstream rows; a VFEP term is changed there first.

## Set-specific rows

None yet. The first pass adds the set name in the upstream label pattern
(hemophage warcasket, shoulders and helmet), as prose uses it, and the
Workshop title. The abilities' names are not final, so they get rows when the
abilities land.

## Charge nouns

The set has no reloadable today and so no `chargeNoun` of its own. If an
ability ever adds one, its noun is shown only through `{CHARGENOUN_plural}` and `{lookup: ...}` slots (see the
skill), and the German worker delegates to the English one (lookup, else `+s`) and Core's German `plural.txt` is thin, so any coined noun ships its own `singular;plural` line in `1.6/Languages/German/WordInfo/plural.txt`. Shipcracker's "fuel unit" row in its glossary
is the worked example for this language.

## To ground in the first pass

- hemophage (the set's name), in the upstream label pattern for the three
  pieces and in prose. It is a coinage beside Biotech's official
  "sanguophage" and "hemogen": derive the rendering from that language's
  official Biotech terms, grounded against the Biotech tar, before coining
  anything.
- hemogen, sanguophage and the Hemogenic gene (the set is built around
  them), taken as the language's Biotech translation has them.
- the Workshop title, which names the mod: "Hemophage" plus the upstream
  warcasket noun.
- the generation paragraph's echoes of Biotech's sanguophage xenotype
  description (ageless, deathless, "Some believe ..."), in the words the
  language's Biotech translation uses for them.
- The abilities' names, once they are final.
- This mod's coinages, once the descriptions are final.

## Pending native review

Nothing of this mod's yet. What is open for the VFEP rows is tracked upstream.
