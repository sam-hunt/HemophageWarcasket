# Brazilian Portuguese glossary: Hemophage Warcasket

First machine-assisted pass 2026-11-22, pending native review. Only
mod-specific coinages and grounding decisions. The VFEP warcasket vocabulary
(warcasket, the parts, the label pattern, foundry, entomb, VFEP's set names
and research labels, the spacer-tier openers and the 10th-generation
paragraph) lives upstream in `l10n/languages/PortugueseBrazilian.md` under
"Grounded common vocabulary, VFE Pirates warcasket domain", with its
grounding source and what is pending native review; the settings and ability
words the family settled sit in that file's main table. The set's
descriptions take the family's spacer-tier opener from there; the generation
paragraph is this mod's own, not the upstream one, and with the series
paragraph and what follows the opener in each first line it is this mod's
prose, shared byte-identically across the three parts. Nothing here restates
the upstream rows; a VFEP term is changed there first.

## Set-specific rows

| English | Brazilian Portuguese | Grounding |
|---|---|---|
| hemophage (set name) | hemófaga, adjective agreeing with carcaça (f.): `carcaça de guerra hemófaga`, `ombreiras de carcaça de guerra hemófaga`, `capacete de carcaça de guerra hemófaga` | Coined from Biotech's `Hemogen.label` "hemogênio" (hemo-) and `Sanguophage.label` "sanguófago" (-fago), so the kinship with both shows. Adjectival like Medic's `médica` and the pack's `catafrática`. Rejected: `hematófaga` (the real zoology word for blood-feeding animals, which reads as feeding, the bloodfeeder gene's), `sanguófaga` (collides with the xenotype), `hemógena` (means "blood-producing", not the series), the bare English `Hemophage`. |
| Hemophage series (prose) | a série Hemófaga | Capitalized as the siblings' `a série Médica`, `a série Suserano`. |
| Workshop title / `HPWC_SettingsCategory` | Carcaça de Guerra Hemófaga | Contains the family's "carcaça de guerra"; title coupling. |
| hemogen / hemogenic / non-hemogenic | hemogênio / hemogênico / não hemogênico | Biotech `Hemogen.label`, `Hemogenic.label`, Keyed `AbilityDisabledNoHemogenGene` "não é hemogênico", `Bloodfeed.description` "humanos não hemogênicos". |
| hemogen amounts in prose | `{0} de hemogênio` (invariable) | Natural mass-noun count; avoids Biotech's "2 hemogênios", which a mod-side number cannot agree with. |
| sanguophage | sanguófago | Biotech `Sanguophage.label` (XenotypeDef). |
| Some think ... / Some believe ... (rumor frame) | Alguns pensam que ... / Outros acreditam que ... | Biotech `Sanguophage.description` (XenotypeDef) paragraph 4 ("Alguns pensam ... Alguns acreditam que ..."); the shipped second rumor opens `Outros acreditam que` so the two do not repeat `Alguns`, and says "dirigida por sanguófagos", echoing its "dirigem secretamente". |
| the top of the program | a cúpula do programa | Coined. |
| hemogen rush (ability, hediff, section) | ímpeto de hemogênio (m.) | Coined. `surto` is Core's mental-break word (`MessageBondedAnimalMentalBreak` "surto mental"), `impulso` its mood-boost word, `descarga` reads as a toilet flush; `ímpeto` is unspent in Core and Biotech. |
| dismember (ability, section) | desmembrar | Coined; infinitive like the siblings' ability labels (`absorver gás`, `ressuscitar mecanoide`). |
| dismemberment (DamageDef label) | desmembramento | Noun like Core's damage labels (`Cut.label` "corte", `Crush` "esmagamento"). |
| has been torn apart (deathMessage) | `{0} foi despedaçad{PAWN_gender ? o : a}.` | Core `ExecutionCut.deathMessage` shape; Core `ShatteredBodyPart` "Despedaçado". |
| tear off (a limb) | arrancar | Core `Scratch.injuryProps.destroyedLabel` "Arrancado", melee `destroyed_present` "arrancando". |
| limb / arm / leg / head | membro / braço / perna / cabeça | Core `GameplayTips.RemoveLimbInfection` "membro do corpo"; `Arm`, `Leg`, `Head` `.label`. |
| strike (charge noun, settings) | golpe (m.) | See "Charge nouns". |
| drain (hemogen, from a wound or target) | drenar / drenagem | Core Keyed `CommandAutoRepairDesc` "drena sua energia" and Biotech `HemogenDrain.labelShortAdj` "drenagem": the neutral resource verb. Never `extrair`, which Biotech spends on `Bloodfeed.description` ("extraia o hemogênio diretamente do sangue") and the hemogen pack surgery, nor `alimentar-se`/`vampiro`, the bloodfeed words. |
| injectors | injetores (de hemogênio) | Coined; Medic's `injetor de adrenalina`. |
| servos | servomotores | Core `BionicJaw.description` "micro servomotores". Never `servos`: Biotech's `SanguophageThrall.label` is "servo(a)", and this mod's quest rows say "servos" for thralls. |
| reinforced arms | braços reforçados | Coined. |
| blood pumping | bombeio de sangue | Core `BloodPumping.label`. |
| blood loss / blood spatter | perda de sangue / respingos de sangue | Core `BloodLoss.label`; `Filth_Blood.label` "sangue". |
| heat protection / impact dispersion | proteção contra calor aprimorada / dispersão de impacto e proteção contra calor aprimoradas | Core `ArmorRating_Heat` "Calor"; Shipcracker's "dispersão de impacto aprimorada". |
| visor | viseira | Medic's `viseira de diagnóstico`. |
| sunlight / UV sensitivity gene | luz solar / gene de sensibilidade UV | Biotech `UVSensitivity_Mild.label` "sensibilidade UV leve", its description "luz solar". |
| pyrophobia gene / fire terror (mental state) | gene pirofobia / terror ao fogo | Biotech GeneDef `FireTerror.label` "pirofobia", MentalBreakDef `FireTerror.label` "terror ao fogo". |
| intellectual work (trait's last clause) | trabalhos intelectuais | Core Keyed `WorkTagIntellectual` "intelectual". |
| Will only wound / May only wound | irá apenas ferir / pode apenas ferir | Core Keyed `WillKill` "irá matar", lowercase as it is. |
| Confirm dialogs | `O golpe matará {PAWN_nameDef}, pois arrancará a cabeça {PAWN_objective}.` / `Drenar hemogênio de {PAWN_nameDef} depois do golpe causará a morte {PAWN_objective}.`, both closing `\n\nTem certeza?` | Biotech `WarningPawnWillDieFromBloodfeeding` ("... matará {PAWN_pronoun}.\n\nTem certeza?"). `{PAWN_objective}` is dele/dela (Core Grammar `ProhimObj`), so it follows a noun (`a cabeça dele`, `a morte dela`). The first keeps `{PAWN_nameDef}` as an object, since an unnamed animal's nameDef carries an article that `de` would not fuse. |
| hostile / non-hostile / faction | hostil / não hostis / facção | Core Keyed `Hostile`, `Faction`. |
| hit points (of a limb) | pontos de vida | Core prose (7 uses). |
| quest / research / storyteller / raid / colony wealth | missão / pesquisa / narrador / invasão / riqueza da colônia | Core `Quests.label` "Missões", `Research.labelShort`, Keyed `Storyteller`, `Raid.label`, `ThisMapColonyWealthTotal`. |
| threat points | pontos de ameaça | Coined from Core Keyed `DifficultyThreatSection` "Ameaças" and `ThreatPoints.valueFormat` "{0} pontos"; Core's `ThreatPoints.label` ("pontos de diversão /10") is a history-graph label, not reusable. |
| Sanguophage meeting (quest) | Reunião de sanguófagos | Biotech `SanguophageMeetingHost` quest text: "Reunião concluída", "começaram sua reunião", "pedindo para você organizar a reunião". |
| Crashed shuttle (quest) | Transporte acidentado | Biotech `SanguophageShip` letter label "transporte acidentado". The quest's own word wins over the upstream "ônibus espacial" row here. |
| master / thralls / leader / guests | mestre sanguófago / servos / líder / convidados | Biotech `SanguophageShip` "um mestre sanguófago e [thrallCount] servo(s)", `SanguophageThrall.label` "servo(a)"; `SanguophageMeetingHost` "O líder deles". |
| entombed (quest pilots) | integrado em uma carcaça de guerra | Upstream `integrar` row. |
| lesser warcaskets | carcaças de guerra comuns | Coined: VFEP's basic sets. `menores` reads "smaller", `inferiores` judges them. |
| down (a pawn) | incapacitar | Overlord's `fica incapacitado`; Core's `abatido` also means slaughtered in a verb. |
| Required research / None | Pesquisa necessária / Nenhuma (agreeing with pesquisa); points floor None = Nenhum | Core None "Nenhum". |

## Ported from the siblings

| English | Brazilian Portuguese | Source |
|---|---|---|
| (default) / enabled / disabled / Off | ` (padrão)` / ativado / desativado / Desligado | Medic, Overlord, Shipcracker Keyed. |
| Default: {0}. | Padrão: {0}. | All siblings. |
| When enabled, ... When disabled, ... | Quando ativado, ... Quando desativado, ... | Medic. |
| Casting time / Cooldown | Tempo de invocação / Tempo de recarga | Medic, Overlord. |
| on cooldown (`cooldownGerund`) | em recarga | Toxin, Shipcracker. |
| Using ability: {0} | usando habilidade: {0}. | Toxin. |
| Enabled work types | Tipos de trabalho ativados | Medic, Overlord. |
| WorkDesc | Overlord's `OLWC_WorkDesc` verbatim with the set name and `Padrão: {0}.` | Overlord. |
| WorkSummary colon frame | `Trabalhos que o piloto pode fazer: {0}, ...` | Medic. |
| holds when full / number of {1} it takes to reload one | `comportam quando cheias` / `A quantidade de {1} necessária para recarregar um golpe.` | Medic `MDWC_ShotsDesc`, Overlord `OLWC_ReloadCostDesc`. |
| SettingsUnavailable frame | `... estão indisponíveis. Outro mod alterou a carcaça de guerra hemófaga, então elas manterão os valores originais.` | Overlord. |
| trait label / description | carcaça de guerra / Medic's text verbatim up to `mas ainda pode fazer trabalhos intelectuais.` | Medic, Overlord. |
| next can follow at once | `O próximo golpe pode ser desferido imediatamente.` | Medic's `A próxima injeção pode ser aplicada imediatamente.` |
| the 10th-generation program clause | `reconstruiu equipamentos piratas capturados em torno das próprias armaduras energizadas e os entregou a veteranos que não podiam mais servir de outra forma` | Overlord's paragraph 2, with the pack's "armaduras energizadas". |

## Charge nouns

Vanilla pt-BR prints a `chargeNoun` only through `{CHARGENOUN}` and
`{CHARGENOUN_plural}` (no `{lookup: ...}`, no `CHARGENOUN_gender`), and its
frames hard-code the masculine (`recarregar um {CHARGENOUN}`, `seu último
{CHARGENOUN}`). Portuguese runs the base `Pluralize` (lookup, else the word
unchanged) and Core ships no plural table, so both counted nouns ship a line
in `1.6/Languages/PortugueseBrazilian/WordInfo/plural.txt`. No
`WordInfo/Gender` file: nothing reads it here.

| English | Singular / plural | Gender | Read-aloud |
|---|---|---|---|
| strike (shoulders, `CompStrikePool.chargeNoun`; `{5}` of the summaries) | golpe / golpes (`golpe;golpes`) | m. | `golpes restantes`, `Custo de recarga por golpe`, `recarregar um golpe`, `seu último golpe`, `As ombreiras comportam 2 golpes` / `1 golpe` |
| component (Core `ComponentIndustrial.label`; `{0}` of `HPWC_DismemberReload`, `{6}` of the summaries, `{1}` of `HPWC_DismemberReloadCostDesc`) | componente / componentes (`componente;componentes`) | m. | `uma recarga de 1 componente` / `3 componentes`, `recarregado com 1 componente`, `A quantidade de componentes necessária` |

Every counted slot takes the bare noun after `com`, `de` or as a direct
object, never declined.

The two count formats, `HPWC_CountedStrikes` and `HPWC_CountedReloadItem`, are `{0} {1}` as in English, so the printed examples in this section are unchanged.

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The term concerns it left open are still pending native review. `recarga` / `recarregar` carries both the reload paid in components and the cooldown (`Tempo de recarga`), sometimes in one summary sentence. `Um golpe leva {0} de invocação` borrows vanilla's `Tempo de invocação` and reads oddly for a physical strike. `irá apenas ferir` was kept against the reader's `apenas ferirá` because Core's `WillKill` beside it is `irá matar`.

Every coinage above: `hemófaga` and `a série Hemófaga`, `ímpeto de
hemogênio`, `desmembrar` / `desmembramento`, `golpe` as the charge noun,
`drenar` / `drenagem`, `servomotores`, `braços reforçados`, `a cúpula do
programa`, `carcaças de guerra comuns`, `pontos de ameaça`, `incapacitar`.
The combat log lines (the `arrancou-lhe` / `lhe arrancou` dative possessive,
the hedged `o(a)`, `no(a)`, `pelo(a)` before a part label and `feri-lo(a)`,
`arrancá-lo(a)`), the two Confirm dialogs (with a genderless target
`{PAWN_objective}` prints `ele(a)`, as in every pt-BR use of the token), and
all free prose: the three descriptions, the two ability descriptions, the
hediff description and the settings tooltips and summaries.
