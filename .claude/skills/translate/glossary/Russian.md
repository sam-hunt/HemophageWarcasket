# Russian glossary, Hemophage Warcasket

First machine-assisted pass 2026-11-22. Only mod-specific coinages and
grounding decisions. The VFEP warcasket vocabulary (warcasket, the parts, the
label pattern, foundry, entomb, VFEP's set names and research labels, the
spacer-tier openers and the 10th-generation paragraph) lives upstream in
`l10n/languages/Russian.md` under "Grounded common vocabulary, VFE Pirates
warcasket domain", with its grounding source (the community pack
OneCodeUnit/VanillaRussianExpanded) and what is pending native review; the
settings and ability words the family settled sit in that file's main table.
The set's descriptions open on Shipcracker's form of the spacer-tier opener
(`... космического уровня с <protection clause>.`); the generation paragraph
is this mod's own, not the upstream one, and with the series paragraph and
what follows the opener in each first line it is this mod's prose. Nothing
here restates the upstream rows; a VFEP term is changed there first.

## Set-specific rows

| English | Russian | Grounding |
|---|---|---|
| hemophage warcasket / shoulders / helmet | гемогенная броня мертвеца / наплечники гемогенной брони / шлем гемогенной брони | Upstream label pattern with the set adjective `гемогенная`, from Biotech `Hemogen.label` `гемоген` and its adjective as Biotech forms it (`HemogenCraving.label` `гемогенное голодание`, `HemogenDrain.label` `гемогенная нестабильность`). The obvious derivation is unavailable: Biotech's Russian already renders "sanguophage" as `гемофаг` (`Sanguophage.label`), so `гемофажья броня` would name the set after the xenotype, which the English avoids on purpose (the mechanics key on the gene). Rejected: `гемофажья` / `гемофагская` (the xenotype's own word), `кровавая` (generic, and Biotech's quest namers spend it), `гематофаговая` (a real word for blood-feeding animals, so it says bloodfeeding). The kinship with `гемофаг` survives through the shared `гемо-` root. |
| Hemophage (the series, in prose) | гемогенная серия | Medic's `медицинская серия` and Toxin's `Токсичная серия` frame. |
| Workshop title / `HPWC_SettingsCategory` | Гемогенная броня мертвеца | Contains `броня мертвеца`; the two are coupled. |
| hemogen / hemogenic / non-hemogenic | гемоген / гемогеничный (short form `гемогеничен`) / негемогеничный | Biotech `Hemogen.label`, Keyed `AbilityDisabledNoHemogenGene` `{0_labelShort} не {0_gender ? гемогеничен : гемогенична}`, `HemogenPack.description` `Гемогеничные люди`, `AlertLowHemogenDesc` `негемогеничным человеком`. Keep `гемогенный` (the set, the hemogen-adjective) and `гемогеничный` (the pilot) apart. |
| sanguophage / master / thrall | гемофаг / гемофаг-повелитель, повелитель / трэлл | Biotech `Sanguophage.label`, the crashed shuttle quest's letters (`гемофаг-повелитель и [thrallCount] трэллов`), `SanguophageThrall.label`. |
| hemogen stores in the body | запас гемогена в организме | Biotech `HemogenPack.description`, `AlertLowHemogenDesc`. The text never says hemogen is "in the blood". |
| hemogen craving | гемогенное голодание | Biotech `HemogenCraving.label`. |
| Hemogen cost / Base hemogen gain | Затраты гемогена (`Затраты гемогена на запуск`) / Базовый прирост гемогена | Biotech Keyed `AbilityHemogenCost`, `AbilityHemogenGain`, verbatim. |
| affected by the target's body size | Количество получаемого гемогена зависит от размера тела цели. | Biotech `Bloodfeed.description`, verbatim; it states a fact, not the feeding. |
| hemogen rush (ability, hediff, section) | гемогенный форсаж, `Гемогенный форсаж` as the header, `форсаж` alone after first mention | Coined. `форсаж` (an engine's boost mode) says fuel burned fast for a burst, which is the mechanic. Rejected: `прилив гемогена` (reads as hemogen coming in, the opposite), `гемогенное усиление` (Biotech spends it on `HemogenAmplified`), `гемогенный рывок` (a dash only, no strength). |
| the injectors / hemogen injectors | инъекторы / инъекторы гемогена | Biotech `XenogermReimplanter.label` `генный инъектор`. |
| dismember (ability, section) | отрыв конечности, `Отрыв конечности` as the header | Coined. A noun phrase, as vanilla ability labels are (`выброс пламени`). `расчленение` was rejected: it is cutting a body into pieces, while the ability tears off one limb (or a head). |
| dismember (verb) | отрывать конечность / конечности | Core `Scratch.injuryProps.destroyedLabel` `оторвано`, the label the strike's own injury gives a lost part. |
| dismemberment (the damage label) | отрыв | Shaped like Core's one-word damage labels (`порез`, `укус`, `разрыв`); `разрыв` itself is spent on `TornadoScratch`. |
| torn apart (death message) | `{0} {PAWN_gender ? разорван : разорвана} на части.` | Core damage death messages gender the participle with `{PAWN_gender ? ... }`; the `{0}` stays as Shipcracker's `SCWC_Breach.deathMessage` keeps it. |
| strike (one use of Dismember, the charge noun) | удар | See "Charge nouns". |
| drain (hemogen, from the wound or a target) / a full drain | вытягивать (вытянуть) гемоген из раны / полное вытягивание, `Время вытягивания` | Coined, to stay off every word Biotech spends: `извлечь гемоген` is Bloodfeed's own (`Bloodfeed.description` `Укусить цель и извлечь гемоген прямо из её крови`), `откачать` is the extraction operation (`ExtractHemogenPack.label`) and implies a pump the pads do not carry, `кормление`/`укус`/`корм` are the bloodfeed ability. `вытягивать` names drawing a resource out and nothing else. |
| servos / reinforced arms | сервоприводы / усиленные манипуляторы | Core `BionicArm.description` `минисервоприводы`; `манипуляторы` as Shipcracker's `пробивные манипуляторы`. |
| burn out (a servo) | сгорает | Coined. |
| blood pumping | кровообращение | Core `BloodPumping.label`. |
| blood loss / bleeding / blood spatter | потеря крови / кровотечение / брызги крови, `пятна крови` | Core `BloodLoss.label`, Keyed `BleedingRate`; `Filth_Blood.label` `кровь`. |
| heat protection / impact dispersion | защита от нагрева / рассеивание энергии ударов | Core `ArmorRating_Heat.label` `Броня: Нагрев`; Shipcracker's `с улучшенным рассеиванием энергии ударов`. |
| visor / sealed visor | визор / герметичный визор | Medic's `диагностический визор`; `герметичный` as Shipcracker's helmet. |
| sunlight / UV sensitivity gene | солнечный свет, на солнечном свету / ген чувствительности к солнцу | Biotech `Sanguophage.description` `на солнечном свету они становятся медлительными`, `UVSensitivity_Mild.label` `чувствительность к солнцу`. |
| pyrophobia gene / fire terror (the mental state) | ген пирофобии / страх перед огнём, `Шанс страха перед огнём` | Biotech `FireTerror.label` (gene) `пирофобия`, `FireTerror.label` (mental state) `страх перед огнём`. |
| hold their nerve / steadies | не терять самообладания / успокаивает | Coined. |
| Will only wound / May only wound | Только ранит / Может только ранить | Core Keyed `WillKill` `Убьёт`. |
| {0} is not hostile / has no limbs to tear off | Цель не враждебна: {0}. / Не осталось конечностей, которые можно оторвать: {0}. | Medic's `Цель враждебна: {0}.`, so no predicate agrees with a name. |
| the two Confirm dialogs | `{PAWN_nameDef} лишится головы от удара, и это будет стоить {PAWN_objective} жизни.\n\nВы уверены?` / `Если после удара вытянуть гемоген из {PAWN_nameDef}, это будет стоить {PAWN_objective} жизни.\n\nВы уверены?` | Biotech `WarningPawnWillDieFromBloodfeeding` `Кормление от {PAWN_nameDef} убьёт {PAWN_possessive}.\n\nВы уверены?`: the closing question verbatim, the name undeclined after a preposition as vanilla leaves it. The English token is `{PAWN_objective}` (Russian dative `ему`/`ей`), which the checker holds, so the clause is built on the dative `стоить ему жизни` instead of vanilla's accusative. |
| quest / colony / colonist count / colony wealth | задание / поселение / число поселенцев / богатство поселения | Core Keyed (`Задание провалено`, `ThisMapColonyWealthTotal` `Общее богатство поселения`); vanilla's Russian never writes `колония`. |
| threat points / storyteller / raid | очки угрозы / рассказчик / налёт | `очки угрозы` coined from Core `ThreatPoints.valueFormat` `{0} очков` and the difficulty page's `угрозы`; Core Keyed `Storyteller`, `Raid`. |
| Sanguophage meeting / leader / guests | Встреча гемофагов / лидер / гости | Biotech `SanguophageMeetingHost` quest (`meetNoun` `встреча`, `Их лидер, [asker_nameDef], просит вас организовать встречу`, `Их встреча продлится несколько часов`). |
| Crashed shuttle (subheading) | Разбившийся челнок | Biotech `sanguophageShuttleCrashedLetterLabel`, verbatim; the quest text `выберутся из корабля и будут готовы к бою`. |
| lesser warcaskets (VFEP's plain sets) | более простая броня мертвеца, `Броня мертвеца у трэллов` as the row label | Coined; the upstream table has no row for it yet. |
| Sanguophages from quests (section) | Гемофаги из заданий | Core `задание`. |
| Required research | Требуемое исследование | Core `требуется`. |
| intellectual work (trait's last clause) | заниматься умственной работой | Core Keyed `WorkTagIntellectual` `Умственная работа`. |
| warcasket (the trait label) | броня мертвеца | Upstream trait row. |
| reportString / jobReportString | `отрывает конечность у TargetA` / `отрывает конечность`; the rush `применяет способность «{0}»` | Core `JobDef` report strings: third person present, lowercase, no period; `у TargetA` as the combat packs' `у [RECIPIENT_definite]`. |
| the rumor frame (paragraph 2) | Одни считают, что ... Другие же верят в то, что ... | Biotech `Sanguophage.description` `Многие считают их легендой ... Другие же верят в то, что гемофаги тайно правят ...`; `управляли` echoes its `правят`. |

## The combat log

`HPWC_Combat_Dismember` puts the part label through
`{lookup: [recipient_part0_label]; Case; 3}`, the accusative. Core's
`Case.txt` lists the part labels the strike takes (`левая рука`, `правая
нога`, `голова`, `рука`, `нога`), so they read `отрывает у Ивана левую руку`;
a label the table lacks (a modded body) degrades to the nominative, which is
what Core's own combat packs print for every label (they place the bare
`[recipient_part0_label]` after the verb). The full rule-pack resolver runs
`lookup` after symbol substitution (`GrammarResolver.ResolveAllFunctions`,
decompiled on 1.7), Core's own book namers use the same construct on a rule
symbol, and the checker strips `{lookup: ...}` before comparing symbols. The
verbs are present tense, as Core's combat packs write them, so nothing agrees
with a pawn's gender; `[RECIPIENT_objective]` (`ему`/`ей`) carries the
possessor in `отрывает ему левую руку` and `[RECIPIENT_possessive]` the
accusative pronoun in `только ранит его`, as Core's `сбивая
[RECIPIENT_possessive] с ног` does. No line names the part again by pronoun,
since a limb and a head differ in gender.

## Ported from the siblings

| English | Russian | From |
|---|---|---|
| (default) / enabled / disabled / Off | ` (по умолчанию)` / `включено` / `выключено` / `Выкл` | Medic, Overlord, Shipcracker. |
| Default: {0}. / When enabled / When disabled | `По умолчанию: {0}.` / `Если включено, ...` / `Если отключено, ...` | Medic, Overlord. |
| Casting time / Cooldown | Время для подготовки / Перезарядка | Core `AbilityCastingTime`, `CooldownTime`, as Medic. |
| Strike capacity, its tooltip | Запас ударов, `Сколько ударов вмещают полностью заряженные наплечники.` | Medic `MDWC_Shots` / `MDWC_ShotsDesc`. |
| Reload cost per strike | Цена перезарядки одного удара | Overlord `OLWC_ReloadCost`. |
| The time the pilot must wait between strikes | Пауза между ударами. | Medic `MDWC_ShotCooldownDesc`. |
| The cooldown after a strike is {1}. / The next strike can follow at once. | Следующий удар можно нанести через {1}. / Следующий удар можно нанести сразу. | Medic's shot summaries; the cooldown sentence is reworded to name when the next strike can follow. |
| The settings are unavailable ... | Настройки способностей и шлема недоступны. Другой мод изменил гемогенную броню мертвеца, поэтому у них останутся исходные значения. | Medic and Overlord `..._SettingsUnavailable`, narrowed to the two sections. |
| Enabled work types / the work toggle | Разрешённые работы / `Если включено, пилоты гемогенной брони смогут выполнять эту работу, но назначить её нужно будет во вкладке «Работа». Если отключено, не смогут.` | Overlord `OLWC_SectionWork`, `OLWC_WorkDesc`. |
| The pilot can do {0} work | Пилоту доступны работы: {0}. | Medic and Overlord; `{0}` is the work types' `labelShort` list, so it sits after a colon. |
| the trait description | `{PAWN_nameDef} находится внутри брони мертвеца. ... но по-прежнему может` | Medic and Overlord, verbatim to the last clause, which ends on `заниматься умственной работой`. |
| on cooldown | перезаряжается | Upstream main table. |
| Using ability: {0} | применяет способность «{0}» | Upstream main table. |
| spacer-tier opener with a protection clause | `Специализированная броня мертвеца / Специализированные наплечники брони мертвеца / Специализированный шлем брони мертвеца космического уровня с ...` | Shipcracker's shortDescriptions. |
| veterans who could no longer serve any other way | ветеранам, которые без неё уже не могли бы служить | Medic's generation paragraph. |

## Charge nouns

| English | Russian | Forms | Lookup files |
|---|---|---|---|
| strike (the shoulders' `CompStrikePool.chargeNoun`, and the settings' word for one use) | удар (m.) | pl. удары; gen. sg. удара; gen. pl. ударов | `1.6/Languages/Russian/WordInfo/plural.txt` (`удар;удары`) and `case.txt` rows for both numbers. Core's `Case.txt` has the singular row and `Gender/Male.txt` the word, so the worker's fallback already gives `удары`; only the plural's case row is missing from Core. |
| component (Core `ComponentIndustrial.label`, the reload item) | компонент | pl. компоненты; gen. pl. компонентов | Core `Plural.txt` (`компонент; компоненты`) and the singular `Case.txt` row; the mod ships the plural's row (`компоненты;компонентов;...`) in `case.txt`, which Core lacks. |

`LanguageWorker_Russian.Pluralize(str, count)` returns the singular below
two, otherwise the singular's genitive for 2 to 4 and the plural's genitive
for 5 and up, each read from the `case` table; a missing plural row returns
the nominative plural (`5 компоненты`), which is why the rows ship. The
counted strings take the noun in an agreement-free slot: `вмещают {5}`
(inanimate accusative equals nominative), `уходит {6}` / `уходит {0}`, and
`Сколько {lookup: {1}; Case; 1}` for the uncounted plural. As printed:

- defaults (2 strikes, 1 component): `Наплечники вмещают 2 удара, и на
  перезарядку каждого уходит 1 компонент.`; `На его замену при перезарядке
  уходит 1 компонент.`
- 1 strike, 3 components: `Наплечники вмещают 1 удар, и на перезарядку
  каждого уходит 3 компонента.`
- 5 strikes, 5 components: `вмещают 5 ударов`, `уходит 5 компонентов`.
- `HPWC_DismemberReloadCostDesc`: `Сколько компонентов уходит на перезарядку
  одного удара.`
- Core's five slots: `Осталось ударов`, `удары` (max charges), `Стоимость 1
  удара`, `удары закончились`, `как только закончатся удары`.

No Gender file: Russian's Keyed never switches on `CHARGENOUN_gender`.

The two count formats, `HPWC_CountedStrikes` and `HPWC_CountedReloadItem`, are `{0} {1}` as in English, so the printed examples in this section are unchanged.

## Workshop page

| English | Russian | Grounding |
|---|---|---|
| hemogenic gene | ген гемогеничности | Biotech `Hemogenic.label` `гемогеничность`. |
| marine (VFEP's marine warcasket) | not named; `более простая броня мертвеца` covers both plain sets | Core `Apparel_PowerArmor.label` `пехотная броня` would give `пехотная броня мертвеца`; the page says only "lesser", as the game's own tooltip does. |
| heat damage (the set's extra protection) | защита от нагрева | Core `ArmorRating_Heat.label` `Броня: Нагрев`, as the shipped descriptions. |
| plating (the helmet's) | бронеплиты | Coined, to keep `броня` for the set. |
| off-record | нигде официально не числится | Coined, from the descriptions' `не значится`. |
| rumor / inside job all the way to the top | слухи / серию тайно одобрили гемофаги, стоявшие во главе программы | Coined, on the descriptions' `Одни считают ... Другие же верят` frame. |
| burnout servos | сервоприводы работают на износ | Coined, the idiom for running a machine until it burns out. |
| from unwilling targets | даже из тех, кто вовсе не собирался им делиться | Coined; `вытянуть` per the drain row. |
| dangerous UV sensitivity / animalfeeder / sanguofeeder (VRE Sanguophage genes) | опасная чувствительность к солнцу / кровоедство за счёт животных / кровоедство за счёт гемофагов | **Coined**: VRE Sanguophage ships English only. From Biotech `UVSensitivity_Mild.label` `чувствительность к солнцу` and `Bloodfeeder.label` `кровоедство`; gene names only, never the drain verb. |
| shield (CE line) | силовой щит | Upstream VFEP table's shield bubble row. |
| aim height (CE) | высота прицеливания | Coined: CE ru has `CE_TargetHeight` `Высота цели` and `режим прицеливания`, no label for the aimed height. |
| intro, headings, compat, FAQ and Links lines shared with the siblings | as Overlord's page; the FAQ answer as Medic's | Sibling pages, verbatim. |

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The read raised no term concerns.

- The set adjective `гемогенная` and the series name `гемогенная серия`, and
  whether a reader takes them for the set's name rather than a description.
- `гемогенный форсаж`, `отрыв конечности`, `отрыв` (the damage label) and
  the drain verb `вытягивать` / `вытягивание`.
- The two Confirm dialogs' `это будет стоить {PAWN_objective} жизни`, and
  `{PAWN_nameDef} лишится головы от удара`.
- The combat log's `{lookup: ...; Case; 3}` on the part label (a step past
  Core's own packs) and its six verbs (`выдирает`, `дёргает за` especially).
- `очки угрозы`, `более простая броня мертвеца` for VFEP's lesser sets, and
  the row labels `Шанс брони у лидера` / `у гостей` / `у повелителя`,
  `Очки угрозы для брони повелителя`, `Броня мертвеца у трэллов`.
- The descriptions' own sentences: `Визор заслоняет пилота от солнечного
  света, а рядом с огнём пилот в таком шлеме реже обращается в бегство`,
  `Негемогеничному пилоту эта серия покажется довольно заурядной`, and the
  generation paragraph's `В документах программы гемогенная серия не
  значится`.
- `HPWC_HelmetPyrophobiaDesc` says what 100% means (`При 100% это случается
  так же часто, как обычно`), which the English carries as "compared to
  normal".
- Workshop page coinages: `бронеплиты`, `нигде официально не числится`, `серию тайно одобрили гемофаги, стоявшие во главе программы`, `работают на износ`, `кто вовсе не собирался им делиться`, the three VRE gene names, `высота прицеливания`.
