# French glossary, Hemophage Warcasket

First pass: machine-assisted, 2026-11-22, no native review yet. Only
mod-specific coinages and grounding decisions; family-wide mechanics and style
(the elision worker, hand-written contractions in DefInjected, spaced `:` `?`
`!`, guillemets for clicked UI) live in `l10n/languages/French.md`. Folder
`French`, grounded against the Core and Biotech `French (Français).tar`s. The
VFEP warcasket vocabulary (warcasket, kept untranslated, the label pattern,
the parts, foundry, entomb, VFEP's set names and research labels, the spacer
opener) lives upstream under "Grounded common vocabulary, VFE Pirates
warcasket domain"; the settings and ability words the family settled sit in
that file's main table. This set's descriptions carry their own paragraphs 2
and 3 (the generation paragraph told as two rumors, and the series
paragraph), so VFEP's shared 10th-generation lore paragraph is not used; only
the spacer opener is the family's, with Shipcracker's finite relative clause
(`qui disperse mieux les impacts`) as the shape for this set's "improved"
stats. A VFEP term is changed upstream first.

## Set-specific rows

| English | French | Grounding |
|---|---|---|
| hemophage (set name) | hémophage (`warcasket hémophage`, `épaulières de warcasket hémophage`, `casque de warcasket hémophage`) | **Coined** from Biotech's `hémogène` (`Hemogenic.resourceLabel`) and `sanguophage` (`XenotypeDef Sanguophage.label`): the `hémo-` of the one and the `-phage` of the other, so a player sees both. Invariable in gender (ends in `-e`), in apposition after the noun as the upstream label pattern has `warcasket médecin`. Rejected: `hématophage` (the real zoological word for blood-feeding animals, which names feeding, the bloodfeeder gene's ground the fiction avoids), `hémogénophage` (unwieldy), `sanguophage` (Biotech's own xenotype). |
| Hemophage series (prose) | la série Hémophage | Capitalized as a series name, as the English. |
| Workshop title / `HPWC_SettingsCategory` | Warcasket hémophage | Title coupling; contains `warcasket`. |
| hemogen | hémogène (m.), `{0} hémogène` after a number | Biotech `Hemogenic.resourceLabel`, Keyed `Hemogen`; the bare number before it as Keyed `CannotWakeNotEnoughHemogen` (`Besoin d'au moins {0} hémogène`). Masculine as `HemogenDesc` (`le produisent`) and the sanguophage description (`de l'hémogène dérivé`); every string avoids an agreement on it. |
| hemogenic / non-hemogenic | hémogénique / non hémogénique | Biotech `Hemogenic.label`, Keyed `AbilityDisabledNoHemogenGene` (`n'est pas hémogénique`), `AlertLowHemogenDesc` (`non hémogénique`). |
| sanguophage | sanguophage | Biotech `XenotypeDef Sanguophage.label`. |
| the rumor frame (Some think ... Some believe ...) | Certains pensent que ... D'autres croient que ... | Biotech `Sanguophage.description` (`Certains pensent qu'il s'agit de légendes ... Certains croient que les sanguophages dirigent secrètement ...`); the second rumor opens `D'autres croient que` so the two frames do not repeat `Certains`. |
| hemogen craving | besoin d'hémogène | Biotech `HediffDef HemogenCraving.label`. |
| hemogen reserve | Réserve d'hémogène | Biotech `Hemogenic.customEffectDescriptions` (`réserve hémogénique`), as a noun phrase for the row label. |
| base hemogen gain | Gain d'hémogène de base | Biotech Keyed `AbilityHemogenGain`, verbatim. |
| body size | taille corporelle | Biotech `Bloodfeed.description`. |
| hemogen rush (ability, hediff, section) | poussée d'hémogène | **Coined.** Core backstory prose `poussée d'adrénaline`; a noun phrase, as Biotech ability labels are. |
| hemogen injectors | injecteurs d'hémogène | **Coined**, plain French. |
| dismember (ability, section) / dismember (verb) | démembrement / démembrer | **Coined.** Noun phrase for the ability label, as Biotech ability labels are; the verb in the non-hostile tooltip. |
| dismemberment (DamageDef) | démembrement | Same word as the ability; shaped on Core damage labels (`tabassage`, `pourrissement`). |
| torn apart (death message) | `{0} a été {PAWN_gender ? mis : mise : mis(e)} en pièces.` | Core `DamageDef` death messages (`a été tu{PAWN_gender ? é : ée : é(e)}`). |
| tear off a limb (job, ability) | arracher un membre à X (`arrache un membre à TargetA.`, `arrache un membre.`) | Dative of the person, as French says `arracher le bras à quelqu'un`; it also keeps `de` off a definite symbol. |
| strike (charge noun, settings) | coup (m.) | See "Charge nouns". |
| drain hemogen (from the wound, from a target) / a drain | extraire de l'hémogène (de la plaie, d'une cible) / une extraction | **Chosen** neutral verb for drawing a resource out: Biotech's own hemogen extraction words (`extraire un pack d'hémogène`, Keyed `MessageCannotStartHemogenExtraction`). Never `se nourrir` / `alimentation sanguine` (Biotech `Bloodfeed.label`, `BloodfeedOn`), `mordre` or `sucer`. Rejected: `prélever` (Core's organ-harvest verb, `organe prélevé sur {0}`), `drainer` (medical wound drainage, evacuating fluid). |
| drain time | Durée d'extraction | Follows the row above. |
| wound (drained from) | plaie | Biotech `Bloodfeed.description` (`scellera la plaie`). |
| servos | servomoteurs | **Coined**, ordinary French; no servo in the Core or Biotech tar. |
| blood pumping | pompage du sang | Core `PawnCapacityDef BloodPumping.label`. |
| blood loss / blood spatter | perte de sang / éclaboussures de sang | Core `HediffDef BloodLoss.label`; `éclaboussures` **coined** (Core `Filth_Blood.label` is just `sang`). |
| heat protection (improved) | qui protège mieux de la chaleur | Shipcracker's `qui disperse mieux les impacts` shape for an improved stat. |
| sunlight / UV sensitivity gene | soleil, lumière du soleil / gène de sensibilité aux UV | Biotech `UVSensitivity_Mild.label` `sensibilité légère aux UV`, `.description` `lumière du soleil`. |
| pyrophobia gene / fire terror | gène de pyrophobie / panique face au feu (`Chance de panique face au feu`) | Biotech `GeneDef FireTerror.label` `pyrophobie`, `MentalStateDef PanicFleeFire.label` `panique face au feu`. |
| hold their nerve | garder son sang-froid | Plain French idiom. |
| melee damage / melee damage factor / move speed | dégâts de mêlée / facteur de dégâts de mêlée / vitesse de déplacement | Core `StatDef MeleeDamageFactor.label`, `MoveSpeed.label`. |
| health (of a limb) | points de vie | Core `MaxHitPoints.description`. |
| intellectual work (trait) | travaux intellectuels (`peut toujours accomplir des travaux intellectuels`) | Core Keyed `WorkTagIntellectual`. |
| work summary | `Le pilote peut travailler comme {0}, en plus des tâches permises à tous les pilotes de warcasket.` | Medic and Overlord `Le pilote peut travailler comme {0}.`; `{0}` is the work types' `labelShort` (`chercheur` by default). |
| sanguophage meeting (quest subheading) / leader / guests | Réunion de sanguophages / chef / invités | Biotech `QuestScriptDef SanguophageMeetingHost` (`meetNoun` `réunion`, `Leur chef`, `héberger la réunion`, `passeront plusieurs heures à discuter`). |
| crashed shuttle (quest subheading) / master / thrall | Navette écrasée / maître sanguophage / serviteur | Biotech `SanguophageShip` (`sanguophageShuttleCrashedLetterLabel` `Navette écrasée`, `un maître sanguophage et [thrallCount] serviteurs`), `PawnKindDef SanguophageThrall.label` `serviteur`. |
| threat points / storyteller / colony wealth | points de menace / narrateur / richesse de la colonie | `points de menace` **coined** (no French vanilla string names them; Keyed `Difficulty_ThreatScale_Label` `Échelle de menace`); Core Keyed `Storyteller` `Narrateur`, `QuestRequiredPlayerWealth` `une richesse de colonie`. |
| lesser warcaskets (VFEP's plain sets) | warcaskets plus modestes | **Coined**; the upstream VFEP section has no row for it. |
| Will only wound / May only wound | Ne fera que blesser / Pourrait ne faire que blesser | Sits beside Core Keyed `WillKill` `Va tuer` in the same tooltip. |
| confirm dialogs | `Le coup arrachera la tête à {PAWN_nameDef}, ce qui sera fatal pour {PAWN_objective}.\n\nÊtes-vous sûr ?`; `Extraire de l'hémogène de {PAWN_nameDef} après le coup sera fatal pour {PAWN_objective}.\n\nÊtes-vous sûr ?` | Biotech Keyed `WarningPawnWillDieFromBloodfeeding` (`Se nourrir sur {PAWN_nameDef} ... tuera.\n\nÊtes-vous sûr ?`). Its `{PAWN_gender ? le : la : le/la} tuera` cannot carry `{PAWN_objective}`, which the checker requires, so the objective sits after `pour` as vanilla places it after a preposition (`avec {1_objective}`). |
| combat log | `[INITIATOR_definite] a arraché [recipient_part0_definite] à [RECIPIENT_definite].` and five like it | Core `RulePacks_CombatMelee` (`en visant [recipient_part0_definite]`); the entry supplies `recipient_part0_definite` (`PlayLogEntryUtility.RulesForDamagedParts`, 1.6), and Core's `WordInfo/Gender` lists the part labels (`tête`, `jambe gauche` f., `bras gauche` m.). The owner is a dative (`à`, `lui`), never `de [RECIPIENT_definite]`; `[RECIPIENT_possessive]` is dropped as upstream says. |

## Ported from the siblings

| English | French | From |
|---|---|---|
| Default: {0}. / (default) / enabled / disabled / Off | Par défaut : {0}. / (par défaut) / activé / désactivé / Désactivé | Medic, Overlord, Shipcracker Keyed |
| When enabled, ... When disabled, ... | Si cette option est activée, ... Si elle est désactivée, ... | Medic, Overlord |
| Casting time / Cooldown | Temps de réalisation / Temps de recharge | Medic, Overlord |
| The time the pilot must wait between X | Le temps que le pilote doit attendre entre deux coups. | Medic `MDWC_ShotCooldownDesc` |
| The number of X the shoulders hold when full | Le nombre de coups que contiennent les épaulières quand elles sont pleines. | Medic `MDWC_ShotsDesc` |
| The number of {1} it takes to reload one X | Le nombre de {1} nécessaires pour recharger un coup. | Overlord `OLWC_ReloadCostDesc` |
| A X takes {0} to cast / The next can follow at once | Un coup se lance en {0}. / Le coup suivant peut partir aussitôt. | Medic summaries |
| Enabled work types / the work tooltip | Tâches activées / Overlord's `OLWC_WorkDesc` plus `Par défaut : {0}.` | Overlord |
| settings unavailable | `... car un autre mod a modifié le warcasket hémophage. Ils conserveront donc leurs valeurs d'origine.` | Overlord `OLWC_SettingsUnavailable` |
| Using ability: {0} / on cooldown | utilise la capacité : {0}. / rechargement | upstream main table |
| toggle again | Une nouvelle activation arrête les injecteurs. | Toxin `TXWC_VentTank.description` |
| warcasket trait label and description | Medic's rendering verbatim up to its last clause, ending `peut toujours accomplir des travaux intellectuels.` | Medic, Overlord |
| entombed | soudé dans | upstream |

## Charge nouns

| comp | English | French | Forms | File |
|---|---|---|---|---|
| `CompStrikePool` (shoulders) | strike | coup (m.) | coups by rule (`+s`) | none |
| reload item | component | composant (Core `ComponentIndustrial.label`, m.) | composants by rule | none |

`LanguageWorker_French.Pluralize` is rule-only, never reads
`WordInfo/plural.txt` and ignores its count, so the mod's `Counted` keeps the
bare label under two and calls it from two up; both nouns are single words
whose rule plural is right, so no `WordInfo` file ships. Masculine on purpose:
fr Keyed never switches on `CHARGENOUN_gender`, and vanilla's
`{CHARGENOUN_plural} restants` and `son dernier` frames then agree (`coups
restants`); `chacun` in the summaries agrees with `coup` too. As printed:

- `HPWC_DismemberReload`: `... en les rechargeant avec 1 composant.` / `... avec 3 composants.`
- Summaries: `Les épaulières contiennent 2 coups, et chacun se recharge avec 1 composant.` / `Les épaulières contiennent 1 coup, et chacun se recharge avec 3 composants.`
- `HPWC_DismemberReloadCostDesc`: `Le nombre de composants nécessaires pour recharger un coup. Par défaut : 1.`

The two count formats, `HPWC_CountedStrikes` and `HPWC_CountedReloadItem`, are `{0} {1}` as in English, so the printed examples in this section are unchanged.

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The term concerns it left open are still pending native review. The lore capitalizes `série Hémophage` while the settings write `warcasket hémophage`, as the English does. `HPWC_WorkSummary`'s `travailler comme {0}` relies on the work types' `labelShort` being person nouns, which Core's `art`, `manutention` and `repos` are not, and the siblings ship the same frame.

Every coined row above, and especially: `hémophage` as the set name;
`poussée d'hémogène`; `démembrement` for both the ability and the damage;
`coup` for the strike (against `frappe`, feminine, which would read `frappes
restants` in vanilla's reload frames); `extraire` / `extraction` for the
drain; `warcaskets plus modestes` for VFEP's lesser sets; `points de menace`;
`Ne fera que blesser` / `Pourrait ne faire que blesser`; the two confirm dialogs'
`sera fatal pour {PAWN_objective}` (a genderless target reads `pour ce/cette`,
Core's `ProitObj`); the six combat log lines (`arraché net`, `n'a fait que
l'entailler`, `lui a lacéré`, `rien n'a cédé`). All free prose: the three
descriptions (paragraph 1 per part, the shared paragraphs 2 and 3), both
ability descriptions, the hediff description, and the settings tooltips and
summaries.
