# German glossary, Hemophage Warcasket

First machine-assisted pass 2026-11-22. Only mod-specific coinages and
grounding decisions. The VFEP warcasket vocabulary (warcasket, the parts, the
label pattern, foundry, entomb, VFEP's set names and research labels, the
spacer-tier openers) lives upstream in `l10n/languages/German.md` under
"Grounded common vocabulary, VFE Pirates warcasket domain", with its
grounding source (no community VFEP German pack) and what is pending native
review; the settings and ability words the family settled (spacer-tech,
Off, When enabled, casting time, cooldown, the VEF `jobReportString` frame
and the `cooldownGerund`) sit in that file's main table. The descriptions
take the family's 10th-generation opener up to its stat clause; the stat
clauses, the generation paragraph and the series paragraph are this mod's
own prose. Nothing here restates the upstream rows; a VFEP term is changed
there first.

## Set-specific rows

| English | German | Grounding |
|---|---|---|
| hemophage warcasket / shoulders / helmet | Hämophagen-Kriegssarg / Hämophagen-Kriegssarg-Schulterpanzer / Hämophagen-Kriegssarg-Helm | Upstream label pattern. Hämophage is coined from Biotech's two official words: `Hemogen` = Hämogen (the Häm- of Biotech's Hämogen, Hämogenbeutel, `Hemopump` = Hämopumpe) and `Sanguophage` = Sanguophage (the -phage). The Fugen-n follows Biotech's own Sanguophage compounds (Sanguophagenmeister, Sanguophagenjäger, Sanguophagen-Kollegen in the SanguophageShip and meeting quests). Rejected: Hämophage-Kriegssarg (no Fugen-n, against Biotech's compounds), Sanguophagen-Kriegssarg (names the xenotype and loses the coinage), Hämogen-Kriegssarg (reads as an item of hemogen, loses the -phage kinship), Blutsauger-Kriegssarg (Biotech's word for bloodfeeders), the English loan Hemophage. |
| Hemophage series (prose) | die Hämophagen-Baureihe | Baureihe per the upstream 10th-generation row ("armour series" = Baureihen) and Toxin's `die Toxin-Baureihe`. |
| Workshop title / `HPWC_SettingsCategory` | Hämophagen-Kriegssarg | Contains Kriegssarg; the two are coupled. |
| warcasket (the trait label) | Kriegssarg | Bare upstream noun, as Medic and Overlord ship it. |
| hemogen / hemogenic / non-hemogenic | Hämogen / hämogenisch / nicht-hämogenisch | Biotech Keyed `Hemogen`, `AbilityDisabledNoHemogenGene` "ist nicht hämogenisch", `Bloodfeed.description` "nicht-hämogenischen Menschen". |
| sanguophage / sanguophages | Sanguophage / Sanguophagen | Biotech `Sanguophage.label`, `labelPlural`. |
| the rumor frame (Some think ... Some believe ...) | Manche vermuten, ... Andere glauben, ... | Biotech `Sanguophage.description` (XenotypeDef): "Manche halten sie für Legenden ... Andere glauben, dass Sanguophagen heimlich ganze menschliche Zivilisationen lenken." Both rumors take the subjunctive (`Manche vermuten, Sanguophagen hätten die Entwürfe gestohlen`, `Andere glauben, Sanguophagen hätten das Programm ... gelenkt`) so neither reads as fact; the shipped second rumor drops Biotech's `dass`. |
| hemogen rush (ability, hediff, section) | Hämogenschub (m.) | Coined. Schub per Medic's adrenaline rush stage (`Schub`, from Adrenalinschub); closed compound like Biotech's Hämogenbeutel, Hämogenvorrat. Rejected: Hämogenrausch (Rausch reads as intoxication). |
| injectors / hemogen injectors | Injektoren / Hämogeninjektoren | Coined, after Medic's Adrenalininjektor. |
| blood pumping (what the injectors replace) | Blutproduktion; in the series paragraph `halten seinen Blutkreislauf auch ohne Herz in Gang` | Core `BloodPumping.label` = Blutproduktion (the health tab's capacity label, kept in the stat clause); Blutkreislauf per Biotech `Sanguophage.descriptionShort` "Architen im Blutkreislauf" for the prose image. |
| dismember (ability, section header, verb) | Zerstückeln; zerstückeln | Coined. Single-verb ability label like Biotech `Resurrect` = Erwecken. Rejected: Gliedmaße abreißen (the head is a possible target too), Verstümmeln (mutilate, loses the limb sense), Zergliedern (anatomical register). |
| dismemberment (DamageDef label) | Zerstückelung | Noun of the ability; Core damage labels are nouns (`Crush` Quetschung, `AcidBurn` Verätzung). |
| {0} has been torn apart. (death message) | {0} wurde in Stücke gerissen. | Core deathMessage frame `{0} wurde ...` (`Scratch` "zu Tode gerissen", `Crush` "zu Tode gequetscht"). |
| tear off / torn off | abreißen / abgerissen | Core `Scratch.injuryProps.destroyedLabel` = Abgerissen (the label a part torn off by this damage shows, since its hediff is Scratch). |
| limb / limbs; body part | Gliedmaße (f.) / Gliedmaßen; Körperteil | Core uses Gliedmaßen; Körperteil per Core `MissingBodyPart.label` fehlender Körperteil. |
| strike (charge noun, settings) | Schlag (m.), pl. Schläge | See "Charge nouns". |
| drain hemogen (from the wound, from a target) | Hämogen entziehen (der Wunde, einem Ziel); noun Hämogenentzug, Entzug | Biotech renders drain as Entzug (`HemogenDrain.label` = Entzug von Hämogen), and Anomaly's prose uses entziehen for drawing a resource out of a body ("entziehe deinen Feinden Erfahrung und Gesundheit"). Never Blut saugen / Blutsaugen / sich nähren (Biotech's `Bloodfeed` ability, warning and prisoner mode) or extrahieren (the bloodfeed description and hemogen-pack surgery). The settings label is `Dauer des Hämogenentzugs`, not Entzugsdauer, because Core's bare Entzug is drug withdrawal. |
| a full drain | ein vollständiger Entzug | Same. |
| base hemogen gain | Basis-Hämogengewinn | Biotech Keyed `AbilityHemogenGain` verbatim. |
| hemogen cost to start | Hämogenkosten zum Start | Biotech Keyed `AbilityHemogenCost` Hämogenkosten. |
| hemogen reserve / hemogen level | Hämogenreserve / Hämogenstand | Biotech `HemogenCraving.description` "Hämogenreserven"; Hämogenstand coined. |
| hemogen craving | Hämogen-Verlangen | Biotech `HemogenCraving.label`. |
| target's body size | Körpergröße des Ziels | Biotech `Bloodfeed.description` "abhängig von der Körpergröße des Ziels". |
| servos | Servos (m. pl.) | Core Miniservos, Mikroservomotoren. |
| burn out (a servo) | durchbrennen | Plain German for a motor failing. |
| blood loss / bleeding / blood spatter | Blutverlust / Blutung / Blutspritzer | Core `BloodLoss.label`; Filth_Blood = Blut, so the spatters are Blutspritzer. |
| move speed / melee damage factor | Laufgeschwindigkeit / Nahkampfschaden-Faktor; row label Nahkampfschaden | Core `MoveSpeed.label`, `MeleeDamageFactor.label`. |
| Will kill / Will only wound / May only wound | Wird töten / Wird nur verletzen / Könnte nur verletzen | Core Keyed `WillKill`. |
| hostile / not hostile | feindlich / nicht feindlich | Core Keyed `HostileLower`; Medic's `{0} ist feindlich.` |
| the confirm dialogs | `Das wird {PAWN_objective} töten.\n\nBist du sicher?`; `..., wird das {PAWN_objective} töten.\n\nBist du sicher?` | Biotech Keyed `WarningPawnWillDieFromBloodfeeding` "Blutsaugen wird {PAWN_nameDef} töten.\n\nBist du sicher?". German's warning has no `{PAWN_objective}`; ours keeps it as the accusative object of `töten` (`wird das {PAWN_objective} töten`) because the checker locks Keyed placeholders to the English. `HPWC_DismemberConfirmKill` puts `{PAWN_nameDef}` in the nominative because the target can be an animal, whose nameDef carries a nominative article; `HPWC_DismemberConfirmDrainKill` may use a dative slot because a drain target is always a named human. |
| sunlight / UV sensitivity gene | Sonnenlicht / ein Gen für UV-Anfälligkeit | Biotech `UVSensitivity_Mild.label` leichte UV-Anfälligkeit, `UVSensitivity_Intense.label` hohe UV-Anfälligkeit, descriptions "empfindlich gegenüber Sonnenlicht", `Sanguophage.description` "durch UV-Licht verlangsamt". |
| pyrophobia gene / fire terror / flee from fire | das Gen 'Pyrophobie' / Feuerterror / panisch vor Feuer fliehen | Biotech `FireTerror` GeneDef label Pyrophobie, MentalBreakDef label Feuerterror, `PanicFleeFire` "flieht panisch vor dem Feuer"; the gene cited in quotes per Toxin's `dem Gen 'Giftgasabsorption'`. |
| heat protection / impact dispersion | Hitzeschutz / Stoßverteilung | Stoßverteilung per the upstream Shipcracker opener variant; Hitzeschutz coined in the same frame (`mit verbessertem Hitzeschutz`). |
| visor | Visier | Medic's Diagnosevisier, Odyssey `ImprovedSights`. |
| sanguophage meeting (quest subheading) / leader / guests | Sanguophagentreffen / Anführer / Gäste | Biotech `SanguophageMeetingHost` description "sich heimlich ... zu treffen", "das Treffen", "ist der Anführer", "Wenn du zusagst". |
| crashed shuttle (quest subheading) / master / thralls | Abgestürzte Raumfähre / Sanguophagenmeister, Meister / Diener | Biotech `SanguophageShip` letters and description ("Eine Raumfähre mit einem Sanguophagenmeister und [thrallCount] Dienern ist in der Nähe abgestürzt", "aus dem Wrack klettern"), `SanguophageThrall.label` Diener. |
| lesser warcaskets (VFEP's plain sets) | einfachere Kriegssärge | Coined; no upstream row yet. |
| threat points | Bedrohungspunkte | Coined: no vanilla German term (Core's `ThreatPoints` stat label is a joke string); after Core Keyed `Difficulty_ThreatScale_Label` Bedrohungsausmaß, `Bedrohungsstufe`. |
| colony wealth / colonist count / storyteller / raid | Koloniereichtum / Kolonistenzahl / Erzähler / Überfall | Core Keyed `ThisMapColonyWealthTotal`, `Difficulty_FixedWealthTimeFactor_Info` "Kolonistenzahl", `Storyteller`, `Raid`. |
| research (required) / None (slider stop) | Forschung, `Erforderliche Forschung` / Nichts | Core `Research.label`; Core Keyed `None` = Nichts, per the upstream table. |
| intellectual work (trait's last clause) | `aber weiterhin intellektuell arbeiten` | Core Keyed `WorkTagIntellectual` = Intellekt and `AllowBookIntellectual.description` "intellektuelle Kenntnisse"; shaped like Medic's `aber weiterhin als Arzt arbeiten`. |
| down (a pawn) / capture | zu Boden bringen / einfangen | Core `PainShockThreshold.description` "zu Boden geht"; Biotech `SanguophageShip` "einzufangen". |

## Combat log (`DefInjected/RulePackDef/Combat_Dismember.xml`)

Built the way Core's German combat packs build a sentence around a part:
the attacker as `[INITIATOR_nameDefNom]`, the target as
`[RECIPIENT_nameDefDat]` (dative of possession: `riss dem Thrumbo den linken
Arm ab`) or `[RECIPIENT_nameDefAcc]`, all three from Core's GlobalUtility
pack, which every resolution includes (`GrammarResolver` adds
`RulePackDefOf.GlobalUtility`, decompile-verified on 1.7). The part takes
its article from `recipient_part0_article_acc` and the possessive from
`recipient_part0_possessive_acc`, both copied verbatim from Core's
`RulePacks_CombatIncludes` into the pack because it has no include, and the
label is declined through `{lookup: [recipient_part0_label]; decline; 7}`
after the article or `; 3` after the possessive, as Core does (decline.txt
lists `linker Arm`, `linkes Bein`, `Kopf`). The "held" line names no
pronoun for the part (`doch der Körperteil hielt stand`), which would need
the part's gender. A modded body part missing from decline.txt prints its
nominative label after a correct article.

## Ported from the siblings

| English | German | From |
|---|---|---|
| (default) / enabled / disabled / Off / Default: {0}. | ` (Standard)` / aktiviert / deaktiviert / Aus / `Standard: {0}.` | Medic, Overlord, Shipcracker. |
| When enabled, ... When disabled, ... | Wenn aktiviert, ... Wenn deaktiviert, ... | Both siblings. |
| Casting time / Cooldown | Wirkzeit / Abklingzeit | Medic, Overlord. |
| The number of X the shoulders hold when full | Wie viele X die Schulterpanzer höchstens fassen. | Medic `MDWC_ShotsDesc`. |
| The number of {1} it takes to reload one ... | Wie viele {1} das Nachladen eines ... kostet. | Overlord `OLWC_ReloadCostDesc`. |
| The time the pilot must wait between strikes | Wie lange der Pilot zwischen zwei Schlägen warten muss. | Medic `MDWC_ShotCooldownDesc`. |
| The next ... can follow at once. / The cooldown after ... is {1}. | Der nächste Schlag kann sofort folgen. / Die Abklingzeit nach einem Schlag beträgt {1}. | Medic summaries. |
| Enabled work types / work desc / work summary | Aktivierte Arbeitstypen / `Wenn aktiviert, können ... diesen Arbeitstyp übernehmen, du musst ihn ihnen aber im Tab 'Arbeit' zuteilen. Wenn deaktiviert, können sie ihn nicht übernehmen.` / `Der Pilot kann diese Arbeitstypen übernehmen: {0}.` | Overlord. |
| settings unavailable | `... sind nicht verfügbar. Eine andere Mod hat den Hämophagen-Kriegssarg verändert, daher behalten Fähigkeiten und Helm ihre eingebauten Werte.` | Overlord, Medic. |
| Using ability: {0} / on cooldown | setzt Fähigkeit '{0}' ein. / klingt gerade ab | Upstream main table, Toxin, Medic. |
| Toggle again to stop ... | Ein erneuter Klick schaltet ... ab. | Toxin `TXWC_VentTank.description`. |
| trait description | Medic's rendering verbatim up to `aber weiterhin`, ending `intellektuell arbeiten.` | Medic, Overlord. |
| combat log file format | the EN comment as `<li>` lines above the translated list | UniqueMeleeWeapons `RulePacks.xml`. |

## Charge nouns

| English | German | Forms | Lookup files |
|---|---|---|---|
| strike (the shoulders' `CompStrikePool.chargeNoun`, the settings' word for one use) | Schlag (m.) | pl. Schläge | `1.6/Languages/German/WordInfo/plural.txt` (`Schlag;Schläge`); masculine, so no Gender line (a table miss resolves masculine) |
| component (Core `ComponentIndustrial.label`, the reload item) | Bauteil (n.) | pl. Bauteile | `plural.txt` (`Bauteil;Bauteile`); Core's `WordInfo/Gender/Neuter.txt` already lists `bauteil`, and Core's `plural.txt` has no line, so the worker would print `Bauteils` |

The German worker delegates to the English one (lookup, else `+s`). Every
mod string takes the counted noun as a direct object or after a colon, in
the citation form: `Ein neuer Servo kostet beim Nachladen {0}.`, `Die
Schulterpanzer fassen {5}, und jeder Schlag kostet beim Nachladen {6}.`,
`Wie viele {1} das Nachladen eines Schlags kostet.` (never `mit {0}`, whose
dative plural `Bauteilen` the worker cannot produce). As printed at the
defaults (2 strikes, 1 component): `Die Schulterpanzer fassen 2 Schläge, und
jeder Schlag kostet beim Nachladen 1 Bauteil.`; at 1 strike and 3
components: `... fassen 1 Schlag, und jeder Schlag kostet beim Nachladen 3
Bauteile.`; the tooltip `Wie viele Bauteile das Nachladen eines Schlags
kostet.` Core's own slots read `Schläge verbleibend`, `Die maximale Anzahl der
Schläge` (the `plural_decline` lookup misses harmlessly), `Schlag-Nachladekosten`,
`Nachladekosten pro Schlag`, `einen Schlag nachzuladen`, `dessen letzter
Schlag`, `Keine Schläge mehr.`

The two count formats, `HPWC_CountedStrikes` and `HPWC_CountedReloadItem`, are `{0} {1}` as in English, so the printed examples in this section are unchanged.

## Workshop page

| English | German | Grounding |
|---|---|---|
| off-record | `den es offiziell nicht gibt` | Coined phrasing. |
| rumor ... some say ... others ... | `Was man sich darüber erzählt, läuft immer auf die Sanguophagen hinaus: Manche vermuten, sie hätten ..., andere glauben, das Ganze sei ...` | The rumor frame above, subjunctive kept. |
| an inside job all the way to the top | `das Ganze sei heimlich von ganz oben abgesegnet worden` | Coined: the second rumor, sanctioned in secret from the top. |
| heat damage protection (beyond the spacer-tier baseline) | `mehr Hitzeschutz, als bei Kriegssärgen der Techstufe Raumfahrt üblich ist` | Hitzeschutz as in the shipped descriptions (Core `ArmorRating_Heat.label` = Rüstung - Hitze); Techstufe Raumfahrt per the siblings' intro line. |
| hemogen metabolism | Hämogenstoffwechsel | Coined closed compound on Biotech's Stoffwechsel (`DeathrestExhaustion.description`). |
| heavy burnout servos | `Schwere Servos arbeiten dabei bis zum Durchbrennen` | durchbrennen per the set-specific rows. |
| even from unwilling targets | `entzieht ... dem Ziel Hämogen, ob es will oder nicht` | Coined. |
| sunlight sensitivity / plating / filters | `bei UV-Anfälligkeit vor Sonnenlicht` / Panzerung / Filter | Biotech `UVSensitivity_*` labels; Panzerung per the upstream VFEP table. |
| brains blown out (the joke) | `eine Kugel ins Hirn` | Coined idiom; Core `Brain.label` is Gehirn, Hirn is the colloquial form. |
| sanguophage quests (scaling) | `Je stärker die Sanguophagen-Quests aus Biotech ausfallen, desto eher ...` | Coined. |
| lesser marine/warcasket sets on accompanying thralls | `begleitende Diener ... in einfacheren Kriegssärgen` | The lesser warcaskets row; marine left out, as the in-game tooltip does (Core `Apparel_PowerArmor.label` Marinerüstung would give a coined Marine-Kriegssarg). |
| mod settings | Mod-Einstellungen | Overlord, Medic, Toxin, Shipcracker pages. |
| dangerous UV sensitivity / animalfeeder / sanguofeeder (VRE Sanguophage genes) | 'gefährliche UV-Anfälligkeit' / 'Tiersauger' / 'Sanguosauger' | **Coined**: the mod ships English only. From Biotech `UVSensitivity_Mild.label` leichte UV-Anfälligkeit and `Bloodfeeder.label` Blutsauger; the gene labels only, never the drain verb. |
| aim height (CE) | Zielhöhe | **Coined**: CE ships no German (its German Keyed is a stray Achtung! file). |
| the hemogenic gene | das Gen 'hämogenisch' | Biotech `Hemogenic.label`; quoted per Toxin's `das Gen 'Giftgasabsorption'`. |
| How do I get it? answer | `Erforsche das Projekt 'Raumfahrt-Kriegssärge' und baue das Set dann wie jedes andere in der Kriegssarg-Gießerei.` | Overlord's page (Toxin and Shipcracker render "weld it on" as `schweiße ... einen Kolonisten ... ein`). |

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The term concerns it left open are still pending native review. `Blutproduktion` in the armor's first paragraph is Core's own label for the blood pumping capacity, and a native reader takes it for "blood production" beside the third paragraph's `Blutkreislauf`.

- The coinage Hämophage and its compounds (`Hämophagen-Kriegssarg`,
  `Hämophagen-Baureihe`).
- `Hämogenschub`, `Zerstückeln` / `Zerstückelung`, `Schlag` as the charge
  noun, `entziehen` / `Hämogenentzug` for drain and the label `Dauer des
  Hämogenentzugs`, `Hämogenstand`, `Hämogeninjektoren`, `Hitzeschutz`,
  `einfachere Kriegssärge`, `Bedrohungspunkte`, `Sanguophagentreffen`, and the
  row label `Kriegssarg-Punkte des Meisters`.
- The six combat log lines, which rest on Core's helper rules and decline
  tables and whose helper rules the reader has not seen in game; a strike on
  a human and on an animal confirms them.
- The two confirm dialogs' clause order, and `HPWC_DismemberReload`'s
  rewording (`Ein neuer Servo kostet beim Nachladen {0}.`).
- Workshop page coinages: `Hämogenstoffwechsel`, `von ganz oben abgesegnet`, `ob es will oder nicht`, `eine Kugel ins Hirn`, the VRE Sanguophage gene labels `gefährliche UV-Anfälligkeit`, `Tiersauger`, `Sanguosauger`, and CE's `Zielhöhe`.
- All free prose: the three first paragraphs and the shared paragraphs 2
  and 3, both ability descriptions, the hediff and trait descriptions, every
  settings tooltip and summary.
