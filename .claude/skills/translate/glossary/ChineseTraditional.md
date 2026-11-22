# Traditional Chinese glossary, Hemophage Warcasket

First pass: machine-assisted generation, 2026-11-22, no native review yet.
Only mod-specific coinages and grounding decisions. The VFEP warcasket
vocabulary (warcasket, the label pattern, the parts, foundry, entomb, VFEP's
set names and research labels, pilot, the spacer-tier opener) lives upstream
in `l10n/languages/ChineseTraditional.md` under "Grounded common vocabulary,
VFE Pirates warcasket domain", with its grounding source and what is pending
native review; the settings and ability words the family settled (Default,
Off, the default suffix, `Using ability: {0}`, the `cooldownGerund`) sit in
that file's main table. Only the spacer opener is the family's here: the
generation paragraph is this mod's own telling, not VFEP's, and with the
series paragraph and what follows the opener in each first line it is this
mod's prose. Nothing here restates an upstream row; a VFEP term is changed
there first. Vanilla terms are grounded against the official Core and
Biotech `ChineseTraditional` data. No term was converted from Simplified
Chinese: zh-Hant's Biotech words differ (血原, 噬血者, 吸血, 血奴 against
zh-Hans 血原质 and its own sanguophage, bloodfeed and thrall words).

## Set-specific rows

| English | Traditional Chinese | Grounding |
|---|---|---|
| hemogen | 血原 | Biotech `GeneCategoryDef` `Hemogen.label`, `Hemogenic.resourceLabel`, Keyed `Hemogen`. Counted as `{0}點血原` in the summaries, as `Hemogenic.description` counts it (損失2點血原). |
| hemogenic (a pilot) / non-hemogenic | 血原性 (血原性駕駛員, 血原性人士) / 非血原性 | Biotech `Hemogenic.label` 血原性, Keyed `AbilityDisabledNoHemogenGene` ({0}並非血原性人士), `Hemogenic.resourceDescription` (非血原性人類). |
| sanguophage (every slot) | 噬血者 | One word in zh-Hant, unlike zh-Hans: `XenotypeDef` `Sanguophage.label`, `PawnKindDef` `Sanguophage.label`, `FactionDef` `Sanguophages.label` / `pawnSingular` / `pawnsPlural`, both quests' rule packs and Keyed `Sanguophages*` all say 噬血者. Used in every slot. |
| hemophage (the set name, `<set>`型) / Hemophage series | 噬血型 / 噬血型系列 | Coined on the upstream `<set>`型 pattern. The English builds "hemophage" from hemo- (hemogen) and -phage (sanguophage); 噬血 takes 血 from 血原 and 噬 from 噬血者, so the set reads as the sanguophage's own suit. Labels 噬血型戰棺裝甲 / 噬血型戰棺護肩 / 噬血型戰棺頭盔; in prose the set is 噬血型戰棺. Rejected: 血原型 (names only the fuel and reads as a resource tag beside the `Hemogen` gene category 血原), 噬原型 (opaque blend), 嗜血型 (generic "bloodthirsty", loses the tie to 噬血者). |
| Workshop title / `HPWC_SettingsCategory` | 噬血型戰棺 | Set name + 戰棺 (the anchor term); the two are equal (title coupling). |
| trait label `warcasket` | 戰棺 | The upstream bare warcasket noun, as Medic and Overlord ship. |
| shell (the opener's body-piece slot, and "injectors in the shell") | 裝甲 | The upstream opener row's resolved slot, matching the label 噬血型戰棺裝甲. |
| hemogen rush (ability, hediff, section header) | 血原爆發 | Coined. 爆發 for a short burst of speed and strength paid for in hemogen; the hediff label repeats the ability's. |
| hemogen injectors / injectors | 血原注射器 / 注射器 | 注射器 per Biotech and Royalty prose (upstream injector row). |
| blood pumping (the capacity) | 血液循環 | Core `PawnCapacityDef` `BloodPumping.label`. |
| heart | 心臟 | Core `BodyPartDef` `Heart.label`. |
| dismember (ability, section header, verb in prose) / dismemberment (the damage) | 肢解 / 肢解 | Coined (no vanilla hit). Chinese uses one word for verb and noun, so the ability label and the DamageDef label coincide. The damage's death message is {0}被撕碎了。, shaped on Core's `Cut` / `Bite` / `Crush` (`{0}被砍死了。`). |
| tear off (a limb) / rip off / tear at | 扯下 / 撕下, 撕了下來 / 撕扯 | Core `Scratch.injuryProps.destroyedOutLabel` 被扯下, `Maneuver_Scratch` `destroyed_present` 扯下 / 撕開, `AncientWarwalkerFoot.description` 撕下. |
| limb / arm / leg / head | 肢體 / 手臂 / 腿 / 頭 (頭部 in prose) | Core prose (`Prosthetics.description` 缺少的肢體), `BodyPartDef` `Arm`, `Leg`, `Head` labels. |
| servos | 伺服機構 | Core `BionicArm.description` / `BionicLeg.description` (迷你伺服機構). |
| reinforced arms (the pads' mechanism) | 強化機械臂 | Coined; 機械臂 as Medic's 戰地醫療機械臂. |
| strike (charge noun, settings) | 打擊 | See "Charge nouns". Settings labels 打擊容量 (the family's X容量), 打擊傷害, 每次打擊的裝填花費 (Overlord's 每次充能的裝填花費). |
| drain (hemogen, from a wound or a target) | 汲取 (汲取血原, 從傷口汲取血原, 汲取時間) | Coined for this mod, a neutral verb for drawing a resource out (Core uses it for drawing on experience). **Not** 吸血 (Biotech `Bloodfeed.label`, the bloodfeed ability, and the bloodfeeder gene's 吸他的血), and not 提取, which `Bloodfeed.description` and the xenotype text use for extracting hemogen from blood. |
| base hemogen gain | 基礎血原所得 | Biotech Keyed `AbilityHemogenGain`, verbatim. |
| hemogen cost to start / hemogen per second | 啟動血原消耗 / 每秒血原消耗 | On Biotech Keyed `AbilityHemogenCost` 血原消耗. |
| hemogen reserve (the rush's floor) | 血原保留量 | Coined. Vanilla's 血原儲備 is the body's own store (`HemogenPack.description` 體內的血原儲備), so it is not spent on a threshold setting. |
| hemogen craving | 血原渴求 (陷入血原渴求) | Biotech `HemogenCraving.label`, Keyed `AlertLowHemogenDesc` (他們會陷入血原渴求). |
| blood loss / bleeding / blood spatter | 失血 / 出血 / 濺血 (label), 血跡 (prose) | Core `BloodLoss.label`, `BleedingRate` 出血率; 濺血 coined for the row label, 血跡 per Core prose. |
| body size | 體型 | Biotech `Bloodfeed.description` (獲得的血原視乎目標的體型). |
| melee damage / melee damage factor / move speed | 近戰傷害 / 近戰傷害系數 / 移動速度 | Core `MeleeDamageFactor.label` (vanilla writes 系數), `MoveSpeed.label`. |
| Will only wound / May only wound (cursor) | 只會致傷 / 可能只會致傷 | Shaped on Biotech Keyed `WillKill` 會殺死. |
| the two Confirm dialogs | …會把{PAWN_objective}殺死。\n\n你確定嗎? | Biotech Keyed `WarningPawnWillDieFromBloodfeeding` (對{PAWN_nameDef}吸血會把{PAWN_objective}殺死。\n\n你確定嗎?), its ASCII `?` kept as the Biotech dialogs all write it. |
| hostile / not hostile / anger their faction | 敵對 / {0}不是敵對的。 / 激怒對方的派系 | Core Keyed `ConfirmLandOnHostileFactionBase` ({0}是敵對的。), `RefugeePodCrash_NonHostile` (將會激怒{PAWN_possessive}派系). |
| sunlight / UV sensitivity gene | 陽光 / 紫外光敏感基因 | Biotech `UVSensitivity_Mild.label` 紫外光敏感, its description (對陽光異常敏感). |
| pyrophobia gene / fire terror (the break) / flee from fire | 「畏火」基因 / 火焰恐懼 / 逃離火場 | Biotech `GeneDef` `FireTerror.label` 畏火, `MentalBreakDef` `FireTerror.label` 火焰恐懼, `MentalStateDef` `PanicFleeFire.label` 逃離火場. The settings row is 火焰恐懼機率. |
| sealed visor / visor | 密封面罩 / 面罩 | 密封 per the upstream sealed row; 面罩 as Medic's 診斷面罩. |
| heat protection / impact dispersion | 隔熱性能 / 衝擊分散性能 | Upstream coinage 隔熱; Shipcracker's shipped 強化了衝擊分散性能. |
| the rumor frame ("Some think ...", "Some believe ...") | 有些人認為，… / 也有人認為，… | Biotech `XenotypeDef` `Sanguophage.description` (有些人認為他們只是傳說…。一些人認為，噬血者在暗中指揮著整個人類文明。); the shipped second rumor opens 也有人認為 where Biotech says 一些人認為, so the two do not repeat; "directed all along" echoes its 在暗中指揮. |
| sanguophage meeting (quest subheading) / leader / guests | 噬血者會議 / 領袖 / 與會者 | `SanguophageMeetingHost` rule packs (meetNoun 會議, 會議完成); 領袖 per Biotech `Sanguophages.leaderTitle` (the quest text's own 主持人 was not used, since the mod's text has the player host). 與會者 coined for the meeting's guests. |
| crashed shuttle (quest subheading) / master / thrall | 墜落的太空梭 / 首領 / 血奴 | `SanguophageShip` letter label 墜落的太空梭; `PawnKindDef` `SanguophageThrall.label` 血奴. Vanilla zh-Hant never renders "master" (領頭的噬血者 in the quest text); 首領 is Core's `Ancients.leaderTitle`. |
| lesser warcaskets (VFEP's plain sets) | 低階戰棺 | Coined; no upstream row. |
| threat points / colonist count / colony wealth / storyteller / raid | 威脅點數 / 殖民者數量 / 殖民地財富 / 故事敘述者 / 襲擊 | 威脅點數 coined (vanilla shows no player-facing "threat points"; 威脅 per Core difficulty keys); Core `Difficulty_FixedWealthTimeFactor_Info` 殖民者數量, `ThisMapColonyWealthTotal`, Keyed `Storyteller`, quest letters 襲擊. |
| required research / none | 所需研究 / 無 | Core Keyed `Research` 研究, `None` 無. |
| intellectual work (trait's last clause) | 知識工作 | Core Keyed `WorkTagIntellectual`. |
| the work summary's `{0}` (work types' short labels) | 駕駛員可以擔任{0}，… | Core `WorkTypeDef` `labelShort`s are person nouns (研究員, 鍛造師, 醫生), so Overlord's 擔任{0} frame. |
| job report / ability job report | 扯下TargetA的肢體 / 扯下肢體 | Bare verb phrase, no 。, per the upstream JobDef rule. |
| the generation and series paragraphs | 第十代戰棺出自一項政府計畫，… / 噬血型系列以駕駛員的血原為動力。… | This mod's prose, byte-identical across the three descriptions; the full text is `1.6/Languages/ChineseTraditional/DefInjected/ThingDef/Apparel_Hemophage.xml`. The 10th-generation sentence follows Medic's shipped rendering of the same facts. |

## Ported from the siblings

| English | Traditional Chinese | Source |
|---|---|---|
| Default: {0}. / (default) / enabled / disabled / Off | 預設值：{0}。 / (預設) / 啟用 / 停用 / 關閉 | Medic, Overlord and Shipcracker Keyed. |
| Casting time / Cooldown / Enabled work types | 施放時間 / 冷卻時間 / 已啟用的工作類型 | Medic and Overlord Keyed. |
| When enabled, … When disabled, … | 啟用時，… 停用時，… | Shipcracker, Medic, Overlord. |
| on cooldown (`cooldownGerund`) / Using ability: {0} | 冷卻 / 使用能力：{0} | Upstream main table; Medic, Toxin, Shipcracker. |
| Toggle again to … | 再次切換即可… | Toxin `TXWC_VentTank.description`. |
| the work toggle's tooltip | 啟用時，噬血型戰棺駕駛員可以從事這項工作，但需要在「工作」面板中自行分配。停用時則無法從事。 | Overlord `OLWC_WorkDesc`. |
| The number of {1} it takes to reload one X. | 裝填一次X所需的{1}數量。 | Overlord `OLWC_ReloadCostDesc`. |
| … holds when full. | 護肩裝滿時可儲存的…數量。 | Medic `MDWC_ShotsDesc`, Overlord `OLWC_ChargesDesc`. |
| At zero, … / The next X can follow at once. | 設為0時，… / 下一次X可以立即進行。 | Overlord. |
| settings unavailable … built-in values | …目前無法使用。另一個模組修改了噬血型戰棺，因此兩者都會沿用內建數值。 | Medic and Overlord `SettingsUnavailable`. |
| the trait description up to its last clause | {PAWN_nameDef}被封在戰棺內。…{PAWN_nameDef}無法從事大多數工作，但仍可從事 | Medic and Overlord `degreeDatas.warcasket.description`, verbatim; ends 知識工作。 |
| combat log line shape | [INITIATOR_definite]扯下了[RECIPIENT_definite]的[recipient_part0_label]。 | Core `Combat_Melee` (…[destroyed_past]了[RECIPIENT_definite]的[destroyed_targets]。, …把…的…[destroyed_past]…。); the possessive `[RECIPIENT_possessive]` sits solid before the noun (Core Grammar `Prohis` 他的), as Core's `result` rules use it. UMW's rule-pack file format. |

## Charge nouns

| `chargeNoun` | Traditional Chinese | Forms and lookup |
|---|---|---|
| strike (the shoulders' `CompStrikePool`, a `CompApparelReloadable`) | 打擊 | One form. Chinese runs the base worker, whose `Pluralize` returns the label unchanged, and the zh-Hant Keyed never switches on `CHARGENOUN_gender`; the zh-Hant tars ship no `WordInfo` folder at all. So no `WordInfo` file is shipped, for the strike noun or for Core's `ComponentIndustrial` label 零件. Chosen over 擊, which reads well counted (一擊) but not in vanilla's bare slots (沒有擊。, 擊剩餘). Read aloud: 沒有打擊。, 打擊剩餘, 這個物體最多能保持的打擊數量。, 每裝填一打擊需要消耗物品的數量。, 每打擊裝填消耗. |

The counters live in two Keyed strings, `HPWC_CountedStrikes` = `{0}次{1}` and `HPWC_CountedReloadItem` = `{0}個{1}`, where `{0}` is the number and `{1}` the noun. The mod's own sentences take the counted noun after 最多可儲存 or as a direct object. At the defaults (2 strikes, 1 component) and at 1 strike, 3 components:

- `HPWC_DismemberReload`: 每次打擊都會燒壞護肩裡的一個伺服機構，裝填1個零件即可換上新的。 / …裝填3個零件即可換上新的。
- `HPWC_DismemberReloadCostDesc` (`{1}` = 零件): 裝填一次打擊所需的零件數量。預設值：1。
- `HPWC_DismemberSummary*`: 護肩最多可儲存2次打擊，每次打擊需裝填1個零件。 / 護肩最多可儲存1次打擊，每次打擊需裝填3個零件。

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The term concerns it left open are listed here as still pending native review.

The reader judged 噬血型 natural for Taiwan and Hong Kong players, since it ties visibly to the official 噬血者, and judged the blood vocabulary natural. A native check is still owed. 注射器 reads as a syringe for something that keeps blood circulating, but the English says "injectors", so it stays. The counters 次 and 個 in `HPWC_CountedStrikes` and `HPWC_CountedReloadItem` are pending native review. The reader proposed 從事{0} for `HPWC_WorkSummary` and it was declined: `{0}` is the work types' `labelShort`, which are person nouns in this language (研究員), so 擔任{0} stays.

Every coinage in "Set-specific rows": 噬血型 (and that it reads as the
sanguophage's suit rather than a separate coinage), 血原爆發, 肢解 for both
verb and damage, 汲取 for drain, 打擊 as the charge noun, 血原保留量, 強化機械臂, 濺血, 低階戰棺,
與會者, 首領 for the shuttle's master, 威脅點數, 只會致傷 / 可能只會致傷. All
free prose: the three descriptions (paragraph 1 after the family opener,
paragraphs 2 and 3), the two ability descriptions, the hediff description,
the six combat log lines, every settings tooltip and summary. The Confirm
dialogs keep vanilla's ASCII `?` (你確定嗎?) on purpose. The VFEP terms'
review is tracked upstream, under the section named above.
