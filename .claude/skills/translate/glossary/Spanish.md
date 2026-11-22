# Spanish (Castellano) glossary, Hemophage Warcasket

First machine-assisted pass 2026-11-22. Only mod-specific coinages and
grounding decisions. The VFEP warcasket vocabulary (warcasket, the parts, the
label pattern, foundry, entomb, VFEP's set names and research labels, the
spacer-tier openers and the 10th-generation paragraph) lives upstream in
`l10n/languages/Spanish.md` under "Grounded common vocabulary, VFE Pirates
warcasket domain", with its grounding source (the community Castilian-leaning
VFE Pirates translation by Cito2310; Castilian `Spanish` only, never
`SpanishLatin`) and what is pending native review; the settings and ability
words the family settled (Off/Default, cooldown, the VEF `jobReportString`
frame and the `cooldownGerund`) sit in that file's main table. The set's
descriptions take the family's spacer-tier opener up to its stat clause; the
generation paragraph is this mod's own, not the upstream one, and with the
series paragraph and what follows the opener in each first line it is this
mod's prose. Nothing here restates the upstream rows; a VFEP term is changed
there first.

## Set-specific rows

| English | Spanish | Grounding |
|---|---|---|
| hemophage warcasket / shoulders / helmet | ataúd de guerra hemófago / hombreras hemófago de ataúd de guerra / casco hemófago de ataúd de guerra | Upstream label pattern, set name invariable in apposition (`hombreras bruto`, Medic's `hombreras médico`). `hemófago` is coined: `hemo-` from Biotech `Hemogen.label` hemógeno (and `Hemogenic.symbolPack` prefix `hemo`), `-fago` from Biotech `Sanguophage.label` sanguífago, so the kinship with both reads at once. Rejected: `hematófago` (a real zoology word for a blood-feeding animal, which sharpens the set into Biotech's bloodfeeding), `hemogenófago` (clumsy), `sanguífago` (the xenotype itself), and keeping English `Hemophage` (the coinage has a transparent Spanish form, unlike VFEP's Siegebreaker). |
| Hemophage (the series, in prose) | la serie Hemófago | Capitalised and invariable as upstream set names are in prose (`La serie Bruto`, `la serie Hazard`). |
| Workshop title / `HPWC_SettingsCategory` | Ataúd de guerra hemófago | Contains `ataúd de guerra`; the two are coupled. |
| warcasket (the trait label) | ataúd de guerra | The bare upstream noun, as Medic and Overlord ship it. |
| hemogen / hemogenic / non-hemogenic | hemógeno (`{0} de hemógeno`) / hemogénico / no hemogénico | Biotech Keyed `Hemogen`, `CannotWakeNotEnoughHemogen` "al menos {0} de hemógeno", `AbilityDisabledNoHemogenGene` "no es hemogénico", `AlertLowHemogenDesc` "un humano no hemogénico". |
| sanguophage | sanguífago | Biotech `Sanguophage.label` (XenotypeDef). |
| hemogen craving | antojo de hemógeno | Biotech `HemogenCraving.stages.hemogen_craving-0.label`, `AlertLowHemogenDesc` "desarrollará un doloroso antojo de hemógeno". |
| base hemogen gain (setting) | Ganancia base del hemógeno | Biotech Keyed `AbilityHemogenGain`, verbatim, the same stat Bloodfeed shows. |
| hemogen reserve | Reserva de hemógeno | Coined from Biotech `Hemogenic.description` "una reserva de fuerza biológica". |
| hemogen rush (ability, hediff, section) | descarga de hemógeno (f.) | Coined. `subidón` is taken (Core `GoJuiceHigh`, Medic's adrenaline stages) and reads as a drug high; `descarga` is the injectors discharging hemogen. The ability's description says `Vuelve a pulsarla` (la habilidad), the summaries `la detiene` (la descarga). |
| injectors / hemogen injectors | inyectores / inyectores de hemógeno | Coined; inyector per Medic's `inyector de adrenalina`. |
| servos / reinforced arms | servos / brazos reforzados | Core `BionicArm.description` "miniservos", `servoarmadura`. |
| burn out (a servo) | fundir (`Cada golpe funde uno de los servos`) | Plain Castilian for a motor or fuse burning out. |
| blood pumping | bombeo de sangre | Core `BloodPumping.label`. |
| dismember (ability, section) / dismemberment (damage) | desmembrar / desmembramiento | Coined; no vanilla hit. The ability label is an infinitive like Toxin's `absorber gas`, `purgar depósito`; the damage label a noun like Core `Mining.label` aplastamiento, `Vaporize.label` vaporización. |
| torn apart (death message) | `{0} ha sido despedazad{0_gender ? o : a}.` | Shape of Core `Vaporize.deathMessage`, `Stun.deathMessage`. |
| tear off (a limb) / limb | arrancar (`arrancarle una extremidad a TargetA`) / extremidad | Core `RulePacks_Maneuvers` `destroyed_past->arrancado`, `Bite.injuryProps.destroyedLabel` Arrancado(a); extremidad per Core prose (6 hits of `extremidades`). |
| strike (charge noun and settings word) | golpe (m.) | See "Charge nouns". |
| drain / a full drain / drain time | extraer (`extraer hemógeno de la herida`) / una extracción completa / Tiempo de extracción | Biotech's verb for drawing hemogen out (`ExtractHemogenPack` extraer pack de hemógeno, `Bloodfeeder.description` "extraer hemógeno de la sangre"). Bloodfeeding's own words are `hemoalimentarse`, `chupar sangre`, `alimentarse de` (Biotech `Bloodfeed.label`, `BloodfeedOn`, `WarningPawnWillDieFromBloodfeeding`), and none of them is used. |
| Will kill / Will only wound / May only wound | Matará / Solo herirá / Puede que solo hiera | Biotech Keyed `WillKill` Matará. |
| the two Confirm dialogs | `El golpe le arrancará la cabeza a {PAWN_nameDef}, lo que {PAWN_objective} matará.\n\n¿Estás seguro?` / `Extraer hemógeno de {PAWN_nameDef} después del golpe {PAWN_objective} matará.\n\n¿Estás seguro?` | Biotech Keyed `WarningPawnWillDieFromBloodfeeding` "Chupar sangre de {PAWN_nameDef} {PAWN_objective} matará.\n\n¿Estás seguro?": the clitic before the verb and the closing question kept. |
| blood loss / bleeding / blood spatter | pérdida de sangre / hemorragia / salpicaduras de sangre | Core `BloodLoss.label`, Keyed `BleedingRate` Hemorragia; `salpicadura` coined (no vanilla hit). |
| melee damage / melee damage factor / move speed | daño cuerpo a cuerpo / factor de daño cuerpo a cuerpo / velocidad de movimiento | Core `MeleeDamageFactor.description` "daño cuerpo a cuerpo" (its label abbreviates CaC), `MoveSpeed.label`. |
| body size | tamaño corporal | Core prose (4 hits). |
| sunlight / UV sensitivity gene / mood | luz solar / un gen de sensibilidad UV / humor | Biotech `UVSensitivity_Mild.description` "sensibles a la luz solar", `UVSensitivity_Mild.label` sensibilidad leve a rayos UV; Core `Mood.label` humor. |
| pyrophobia gene / fire terror / flee from fire | el gen de pirofobia / terror al fuego / huir del fuego (`huirá aterrorizado`) | Biotech `FireTerror.label` pirofobia (GeneDef) and terror al fuego (MentalBreakDef), Core `PanicFleeFire.beginLetter` "está huyendo del fuego". |
| sealed visor | visor hermético | Upstream `sealed` = hermético. |
| hold their nerve | no perder los nervios; the section's tooltip says `calma` | Plain Castilian. |
| intellectual work (trait) | trabajos intelectuales | Core Keyed `WorkTagIntellectual` intelectual (the trait blocks by work tag; there is no Intellectual work type). |
| Sanguophages from quests (section) | Sanguífagos de misiones | Core `Quest` misión. |
| sanguophage meeting / leader / guests | Reunión de sanguífagos / el líder / los demás invitados | Biotech `SanguophageMeetingHost` quest text: "Llegada de sanguífagos", "Reunión finalizada", "Su líder [asker_nameDef] te pide que organices la reunión", "Pasarán varias horas discutiendo". |
| crashed shuttle / master / thralls | Transbordador estrellado / el maestro sanguífago / los sanguiesclavos | Biotech `SanguophageShip` quest text: `sanguophageShuttleCrashedLetterLabel` Transbordador estrellado, "un maestro sanguífago y ... sanguiesclavos", "Puedes usar señales para atraerlo y hacer que se estrelle cerca". The pawn kind `SanguophageThrall.label` is the bare `esclavo`; the quest's own `sanguiesclavo` is kept so the thralls are not read as slaves. |
| down (a pawn) | abatir | Biotech `SanguophageMeetingHost` "Los sanguífagos abatidos". |
| threat points / storyteller / colony wealth | puntos de amenaza / el narrador / la riqueza de la colonia | Coined from Core Keyed `Difficulty_ThreatScale_Label` "Escala de las amenazas" (threat points are never player text in vanilla); Core Keyed `Storyteller` Narrador, `Riqueza de la colonia` (wealth graphs). |
| raid | asalto | Upstream table. |
| lesser warcaskets (VFEP's plain sets) | ataúdes de guerra menores | Coined; no upstream row. |
| the warcasket-chance rows | Ataúd de guerra del líder / de los invitados / del maestro, Ataúdes de guerra de los sanguiesclavos, Puntos de amenaza del maestro | Shortened: the row shows a percentage and the subheading names the quest, and `Probabilidad de ataúd de guerra del líder` stacks four `de`. |
| required research / None | Investigación necesaria / Ninguna (research) and Ninguno (threat points) | Core `None` Ninguno, agreeing with the row's noun. |
| entombed | sepultado en | Upstream. |

## Ported from the siblings

| English | Spanish | From |
|---|---|---|
| (default) / enabled / disabled / Off | ` (por defecto)` / activado / desactivado / Desactivado | Medic, Overlord; upstream Off row. |
| Default: {0}. / When enabled, ... When disabled, ... | `Por defecto: {0}.` / `Si está activado, ... Si está desactivado, ...` | Medic, Overlord. |
| Casting time / Cooldown | Tiempo de lanzamiento / Enfriamiento | Medic, Overlord; Core `AbilityCastingTime`, `CooldownTime`. |
| on cooldown / Using ability: {0} | en enfriamiento / `usando habilidad: {0}.` | Upstream table, Toxin. |
| tooltip openers | `La cantidad de ...`, `Tiempo que ...`, `Número de ...`, `Cantidad de {1} que cuesta recargar ...`, `Frecuencia con la que ...`, `Cuánto ...` | Toxin, Medic, Overlord `OLWC_ReloadCostDesc`, `OLWC_SparkFrequencyDesc`. |
| At zero, ... | `Con 0, ...` | Overlord `OLWC_ChargesPerDayDesc`. |
| capacity row and tooltip | `Capacidad de golpes`, `Número de golpes que llevan las hombreras con la carga completa.` | Medic `MDWC_ShotsDesc`. |
| The next strike can follow at once. | `El siguiente golpe puede darse de inmediato.` | Medic `MDWC_ShotSummaryNoCooldown`. |
| The cooldown after a strike is {1}. | `El enfriamiento tras un golpe es de {1}.` | Medic. |
| Enabled work types / the work tooltip / the work summary | `Tipos de trabajo activados` / Overlord's `OLWC_WorkDesc` verbatim / `El piloto puede hacer trabajos de {0}, ...` | Medic, Overlord. |
| whatever armor it wears | `lleve la armadura que lleve` | Overlord `lleven lo que lleven`. |
| Toggle again to ... | `Vuelve a pulsarla para ...` | Toxin `TXWC_VentTank.description`. |
| the trait description | Medic's rendering verbatim, ending `pero sí puede hacer trabajos intelectuales.` | Medic, Overlord. |
| job report with an animate target | `arrancándole una extremidad a TargetA.` | Core `tratando a TargetA.`, Medic. |
| combat log `{replace:}` contraction | `{replace: de [RECIPIENT_definite]; "de &lt;color=#D09B61FF>el "-"&lt;color=#D09B61FF>del "; "de el "-"del "}` and the `a`/`al` twin | Core `RulePacks_CombatMelee` (the two-pattern form of its `evitó` line), UMW. |

## Combat log

The entry supplies `[recipient_part0_definite]` as well as the bare label
(`PlayLogEntryUtility.RulesForDamagedParts`, decompiled on the local 1.7
install; Core es's own melee lines use it on 1.6), with the article from
`WithDefiniteArticle` and Core's Gender tables, which list every vanilla limb
and the head (`brazo izquierdo` Male, `pierna izquierda`, `cabeza`, `pata
delantera izquierda` Female). The lines are built as Core es builds its
destroyed-part lines (`ha [destroyed_past] [destroyed_targetsDef] de
[RECIPIENT_definite]`) and its dative ones: `le ha arrancado la cabeza al
jabalí`. No line refers back to the part with a pronoun, since `lo`/`la`
would need its gender; the "wounded it" lines say `le ha causado una herida`
(the pawn) or `ha herido al jabalí en la pierna`. `[RECIPIENT_possessive]`
is dropped (Spanish names one's own body part with the article).

## Charge nouns

| English | Spanish | Forms | Lookup files |
|---|---|---|---|
| strike (the shoulders' `CompStrikePool.chargeNoun`) | golpe (m.) | pl. golpes | none: in neither Core table, the worker's rule appends `s` after a vowel, and a gender miss is the masculine it needs |

Read in Core's slots: `golpes restantes`, `El número máximo de golpes`,
`recargar un golpe` (`{CHARGENOUN_indefinite}`), `Coste por golpe`, `No
quedan golpes.` This tar's `Stat_Thing_ReloadDestroyOnEmpty_Desc` does not
switch on the gender; `StatsReport_ReloadRemainingChargesMultipler`
hard-codes `las {CHARGENOUN_plural}` (a vanilla slip for a masculine noun;
nothing to do). Rejected: `ataque` (too general beside the combat log's own
attacks), `uso` (a stat word).

The reload item is Core's `ComponentIndustrial.label`, `componentes`, plural
already: Core's `WordInfo/plural.txt` maps `componentes;componentes`, so
`HPWC_DismemberReloadCostDesc`'s `{1}` reads `Cantidad de componentes que
cuesta recargar un golpe.`, and a count of one prints `1 componentes`. The
sentences that take the count (`HPWC_DismemberReload`, the four
`HPWC_DismemberSummary*`) put it after a colon as a cost readout (`Coste de
recarga: 1 componentes.`, `Coste de recarga por golpe: 1 componentes.`, as
Overlord's `OLWC_GaugeReload` does), where it reads as a value rather than a
broken noun phrase.

The two count formats, `HPWC_CountedStrikes` and `HPWC_CountedReloadItem`, are `{0} {1}` as in English, so the printed examples in this section are unchanged.

## Workshop page

| English | Spanish | Grounding |
|---|---|---|
| off-record (warcasket) | extraoficial | Coined: plain Castilian, the in-game descriptions' `no consta en sus archivos` said in one word. |
| Rumor blames ... some say ..., others that ... | `Todos los rumores señalan a los sanguífagos. Unos dicen que ..., y otros, que ...` | Coined, hearsay frame beside the descriptions' `Algunos creen que`. |
| an inside job all the way to the top | `todo se tramó desde dentro, en las más altas esferas` | Coined: `la cúpula` is the descriptions' word and is not repeated. |
| heat damage (protection against) | protección contra el calor | Core `ArmorRating_Heat.label` Armadura - Calor; the set descriptions' `protección contra el calor mejorada`. |
| beyond the spacer-tier baseline | más ... de lo habitual en la era espacial | Coined; `era espacial` per Core `TechLevel_Spacer`. |
| Mix and match with other warcaskets' pieces | `Combina sus piezas con las de otros ataúdes de guerra como quieras.` | The siblings' `Combina las piezas con ... como quieras`, reshaped for this page's object. |
| hemogen metabolism | metabolismo de hemógeno | Biotech prose `metabolismo` (deathrest, `GeneticallyUnstable` texts). |
| burnout servos / tear clean off / unwilling targets | servos ... que se funden con el uso / arrancar de cuajo / incluso de quien se resiste | `fundir` from the set rows; `de cuajo` plain Castilian; coined. |
| sunlight sensitivity (shields against) | protege a los pilotos sensibles a la luz solar | Biotech `UVSensitivity_Mild.description` "sensibles a la luz solar". |
| filters that steady against pyrophobia | filtros que aplacan la pirofobia | Biotech `FireTerror.label`; `aplacar` coined so the descriptions' `no perder los nervios` is not repeated. |
| getting their brains blown out | que le vuelen la tapa de los sesos | Coined: the Castilian idiom, blunt as the English. |
| can now scale to include / the hemophage itself / accompanying thralls | `Cuando son lo bastante difíciles, ... pueden incluir` / el propio Hemófago / los sanguiesclavos que lo acompañan | Coined; series name capitalized in prose per the row above. |
| lesser marine/warcasket sets | ataúdes de guerra menores | The settled `lesser warcaskets` row; Core es has no `marine` word (`Apparel_PowerArmor.label` servoarmadura), so the marine set is not named. |
| hemogenic gene | el gen hemogénico | Biotech `Hemogenic.label`; `gen chupasangre` (`Bloodfeeders_Reviled` precept) shows the `gen` + label shape. |
| dangerous UV sensitivity / animalfeeder / sanguofeeder (VRE Sanguophage genes, quoted) | "sensibilidad peligrosa a rayos UV" / "chupasangre de animales" / "chupasangre de sanguífagos" | Coined: VRE Sanguophage ships English only. From Biotech `UVSensitivity_Mild.label` sensibilidad leve a rayos UV and `Bloodfeeder.label` chupasangre. |
| built-in support | compatibilidad integrada | Coined after the siblings' `parche integrado`. |
| shield (CE line) | valores de armadura y de escudo | Overlord's CE line with shield added. |
| Dismember respects aim height | `La habilidad desmembrar golpea a la altura que elijas al apuntar.` | Plain Castilian for CE es `CE_TargettedModesMelee.helpText` "la zona de altura seleccionada", `CE_MeleeTargetting_CurHeight` Zona atacada. |
| Research spacer warcaskets (FAQ) | `Investiga el proyecto "ataúdes de guerra de la era espacial" y monta el conjunto en ...` | Medic's line without its `de VFE Pirates`, which this English lacks. |

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The term concerns it left open are still pending native review. `descarga de hemógeno` leans toward an electrical discharge or an unloading, and the reader would have preferred `subidón` or `arrebato`.

- Every coinage: `hemófago`, `descarga de hemógeno`, `desmembrar` and
  `desmembramiento`, `golpe` as the charge noun, `extraer` and `extracción`
  for drain, `fundir` for a servo burning out, `ataúdes de guerra menores`,
  `puntos de amenaza`, `salpicaduras de sangre`, `Reserva de hemógeno`, and
  `hombreras hemófago` (the invariable set name reads like a gender slip next
  to a feminine noun, as Medic's does).
- `1 componentes` in the reload line and the Dismember summaries at the
  default count, and the colon frame chosen for it.
- The shortened warcasket-chance row labels (`Ataúd de guerra del líder`,
  ...), which drop the word for chance.
- The combat log lines, especially `pero no lo ha conseguido` for the sixth line. The reader's `pero la extremidad ha resistido` was replaced by it because the part may be a head.
- `sanguiesclavo` (the quest's word) over the pawn kind's `esclavo`.
- All free prose: the three first paragraphs and the shared paragraphs 2
  and 3 (the rumor frame `Algunos creen que ... Otros creen que ...`, after
  Biotech `Sanguophage.description`'s "Algunos creen que"), both ability
  descriptions, the hediff and trait descriptions, every settings tooltip and
  summary.
- The Workshop page's coinages: `extraoficial`, the rumor frame, `se tramó desde dentro, en las más altas esferas`, `aplacan la pirofobia`, `que le vuelen la tapa de los sesos`, `de quien se resiste`, the three VRE Sanguophage gene labels and `compatibilidad integrada`.
