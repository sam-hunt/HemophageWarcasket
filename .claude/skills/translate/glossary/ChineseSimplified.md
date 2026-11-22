# Simplified Chinese glossary, Hemophage Warcasket

First pass: machine-assisted generation, 2026-11-22, no native review yet.
Only mod-specific coinages and grounding decisions. The VFEP warcasket
vocabulary (warcasket, the parts, the label pattern, foundry, entomb, VFEP's
set names and research labels, pilot, the spacer-tier opener, 改进型冲击力偏转模块
as Shipcracker ships it) lives upstream in `l10n/languages/ChineseSimplified.md`
under "Grounded common vocabulary, VFE Pirates warcasket domain", with its
grounding source and what is pending native review; the settings and ability
words the family settled (Off, default, the settings suffixes, cooldown,
casting time, `Using ability: {0}`, the `cooldownGerund`) sit in that file's
main table. Only the spacer opener is the family's here: what follows it in
each first paragraph, the generation paragraph (this mod's own, not VFEP's)
and the series paragraph are this mod's prose and are recorded below. Nothing
here restates an upstream row; a VFEP term is changed there first. Vanilla
terms are grounded against the Core and Biotech `ChineseSimplified (简体中文).tar`.

## Set-specific rows

| English | Simplified Chinese | Grounding |
|---|---|---|
| sanguophage(s) (every surface of this mod) | 血族 | The official text spends two words, by slot. 赫血种 is the xenotype's own label and its `descriptionShort` (`XenotypeDef Sanguophage.label`), the social interaction `SanguophageChat` and one quest-name adjective. 血族 is sanguophages as people: `PawnKindDef Sanguophage.label`, `FactionDef Sanguophages.pawnSingular`, both quests' text (血族会面, 血族主人), the scenario, the Keyed messages, and the xenotype description itself wherever it speaks of sanguophages in general (当前血族的人数不为人知……). Every sanguophage in this mod is a person in that general sense, so 赫血种 appears nowhere. |
| hemophage (the set name, `<set>`型) | 噬血型 | Coined. 血 is the morpheme Biotech's 血原质 and 血族 / 赫血种 share; 噬 is the standard Chinese rendering of the suffix -phage (噬菌体 bacteriophage, 巨噬细胞 macrophage), so 噬血 is hemo-phage morpheme for morpheme. Labels 噬血型战棺装甲 / 噬血型战棺护肩 / 噬血型战棺头盔. Rejected: 嗜血型 (嗜血 is spent on the bloodfeeder gene's name symbol, `Bloodfeeder.symbolPack`, and both quests' name adjectives), 赫血型 (reads as 赫 + 血型 "blood type"), 血原型 (reads as 血 + 原型 "prototype"), 血族型 (names the sanguophage outright, which the English coinage avoids). |
| Hemophage series (prose) | 噬血型 | As Medic's 医疗型 stands for its series (这一型号 where the series is the subject again). |
| Workshop title / `HPWC_SettingsCategory` | 噬血型战棺 | Set name + 战棺 (the anchor term); the two are equal (title coupling). |
| trait label `warcasket` | 战棺 | The community pack's `VFEP_WarcasketTrait` label, as every sibling ships it. |
| hemogen | 血原质 (noun); 血原 in compounds | Biotech Keyed `Hemogen`, `Hemogen.label`; the short form in compounds as Biotech's own 血原包, 血原耗散, 血原化. |
| hemogenic (the gene; a hemogenic pilot) / non-hemogenic | 血原化基因 / 拥有血原化基因的驾驶员 / 没有血原化基因的驾驶员 | `GeneDef Hemogenic.label` 血原化; the phrasing is Biotech's `BloodTransfusion.description` (拥有“血原化”基因的人) and `Hemogenic.resourceDescription` (没有“血原化”基因的人类), without their quotes, as `AlertLowHemogenDesc` writes 无血原化基因组的人类. The official text also renders "hemogenic" as 血之能力者 (`AbilityDisabledNoHemogenGene`) and 渴血症 (`HemogenPack.description`); neither is used. |
| hemogen rush (ability, hediff, settings section) | 血原奔涌 | Coined on Biotech's 血原 compounds (血原耗散). 奔涌 (surging, rushing like a torrent) carries the speed and the flow of a running toggle; 爆发 was rejected as a momentary burst. |
| hemogen injectors / injectors | 血原质注射器 / 注射器 | 注射器 per the upstream table (Royalty `VenomFangs`). |
| blood pumping (replace / keep the pilot's) | 血液循环 (接管 / 维持) | Core `BloodPumping.label`. |
| heart | 心脏 | Core `Heart.label`. |
| improved heat protection | 热能护甲经过强化 | Core `ArmorRating_Heat.label` 护甲 - 热能 (the stat the devilstrand raises); no VFEP set has the phrase. |
| dismember (ability, verb, settings section) / dismemberment (damage) | 肢解 / 肢解 | Coined plain word; `DamageDef` label shaped like Core's 割伤 / 处决. Death message {0}被撕碎了。 on Core's {0}被咬死了。 shape. |
| tear off / rip off (a limb) | 撕下 / 扯掉 | 撕下 is Core's combat log `destroyed_past` / `destroyed_present`. |
| limb / arm / leg / head | 肢体 / 手臂 / 腿 / 头 | Core `PowerClaw` and prosthetic descriptions (肢体), `Arm.label`, `Leg.label`, `Head.label`. |
| strike (charge noun; one use of Dismember) | 打击 | See "Charge nouns". Counted as 次 in the mod's own sentences (每次打击, 打击次数). |
| servos / reinforced arms | 伺服马达 / 强化机械臂 | Core prose 一套微型伺服马达; 机械臂 as Medic's 战地医疗机械臂. |
| drain (hemogen, from a wound or a target) | 抽取 | Biotech's `ExtractHemogenPack.description` (从目标人物身上抽取血原质). Never 汲血 (Bloodfeed) or 啃咬. Used in the description, the ability, the settings and the confirm dialog alike. |
| drain time / base hemogen gain | 抽取时间 / 基础血原质获取量 | Coined; 获取 per `Bloodfeed.description` (获取到的血原质受目标体型影响). |
| blood loss / bleeding | 失血 / 出血 | Core `BloodLoss.label`, `BloodLoss.description`. |
| blood spatter(s) | 溅血 (label) / 血迹 | Coined; Core `Filth_Blood.label` is 血. |
| hemogen reserve | 血原质保留量 | Coined. |
| hemogen craving | 血原质渴求 | Biotech `HemogenCraving.label` (not the 渴血症 of `AlertLowHemogenDesc`). |
| sunlight / UV sensitivity genes | 日光 / 日光敏感、日光过敏 | Biotech `UVSensitivity_Mild.label`, `UVSensitivity_Intense.label`. |
| pyrophobia gene / fire terror (mental state) | 恐火症 / 畏惧火焰 | Biotech `GeneDef FireTerror.label`, `MentalBreakDef FireTerror.label`; row label 畏惧火焰几率. |
| visor / sealed visor | 面罩 / 密封面罩 | As Medic's 诊断面罩. |
| human (target) | 人类 | `Bloodfeed.description` (人类为目标). |
| intellectual work (trait) | 智识工作 | Core Keyed `WorkTagIntellectual`. |
| Will only wound / May only wound | 只会受伤 / 可能只会受伤 | Shaped on Core Keyed `WillKill` (将会死亡). |
| confirm dialogs | ……会使{PAWN_objective}死亡。\n\n你确定吗？ | Biotech Keyed `WarningPawnWillDieFromBloodfeeding` (对{PAWN_nameDef}汲血会使{PAWN_objective}死亡。\n\n你确定吗？), with 汲血 replaced by the strike or by 抽取血原质. |
| Sanguophage meeting / Crashed shuttle (quest subheadings) | 血族会面 / 坠毁的穿梭机 | `SanguophageMeetingHost` name rules (血族 + 会面); `SanguophageShip.sanguophageShuttleCrashedLetterLabel`. |
| sanguophage master / thrall / leader / guests | 血族主人 / 血仆 / 领袖 / 宾客 | `SanguophageShip` quest text, `PawnKindDef SanguophageThrall.label`, the meeting's questDescription (他们的领袖); 宾客 coined. |
| lesser warcaskets (VFEP's plain sets) | 低阶战棺 | Coined: no VFEP, community or sibling rendering exists. |
| threat points / storyteller / colony wealth / raid | 威胁点数 / 故事叙述者 / 殖民地财富 / 袭击 | 威胁点数 coined (Better Traders Guild's precedent; vanilla's player text says 威胁强度, its `ThreatPoints.label` is a debug graph's); Core `ChangeStoryteller`, `ThisMapColonyWealthTotal`, Keyed `Raid`. |
| research / required research | 研究 / 所需研究 | Core Keyed `Research`. |
| non-hostile / faction | 非敌对 / 派系 | Core Keyed `Hostile` (敌对), `HostileTo`. |
| melee damage factor / move speed | 近战伤害系数 / 移动速度 | Core `MeleeDamageFactor.label`, `MoveSpeed.label`. |
| rumor frame (paragraph 2) | 有些人认为…… 也有人猜测…… | Biotech's own sanguophage xenotype description (有些人认为他们数量少到无关紧要，有些则猜测他们可能已经在暗中掌控了数个人类文明); the shipped second rumor opens 也有人猜测 where Biotech says 有些则猜测. |
| paragraphs 2 and 3 | 第十代战棺出自一项政府项目…… / 噬血型以驾驶员的血原质为动力…… | This mod's prose, byte-identical across the three descriptions; the opening sentences of paragraph 2 follow Medic's shipped telling of the same program. Full text in `1.6/Languages/ChineseSimplified/DefInjected/ThingDef/Apparel_Hemophage.xml`. |

## Ported from the siblings

| English | Simplified Chinese | Source |
|---|---|---|
| Default: {0}. | 默认：{0}。 | Every sibling tooltip. |
| When enabled, … When disabled, … | 启用后，… 禁用后，… | Shipcracker `SCWC_VacuumBurnDesc`, Medic. |
| enabled / disabled / Off / (default) | 启用 / 禁用 / 关 / （默认） | Medic, Shipcracker. |
| Enabled work types | 已启用的工作类型 | Medic, Overlord. |
| When enabled, pilots … can do this work, though you will need to assign it in the work tab. When disabled, they cannot. | 启用后，噬血型驾驶员可以从事这项工作，但需要你在「工作」面板中分配。禁用后则不能从事。 | Overlord `OLWC_WorkDesc`. |
| Casting time / Cooldown / The time the pilot must wait between … | 释放时间 / 冷却时间 / 驾驶员两次……之间需要等待的时间。 | Medic, Overlord. |
| The number of … the shoulders hold when full. | 护肩装满时可容纳的……数量。 | Medic `MDWC_ShotsDesc`. |
| Reload cost per … / The number of {1} it takes to reload one … | 每……的装填消耗 / 装填一……所需的{1}数量。 | Overlord `OLWC_ReloadCost`, `OLWC_ReloadCostDesc`. |
| The next … can follow at once. | 下一次……可以立即进行。 | Medic summaries. |
| At zero, … | 设为0时，…… | Overlord `OLWC_ChargesPerDayDesc`. |
| These settings are unavailable. Another mod has changed … | ……设置不可用。另一个模组修改了噬血型战棺，因此这些部分将沿用内置数值。 | Medic, Overlord. |
| trait description | {PAWN_nameDef}被封在了战棺里……但仍然可以从事智识工作。 | Medic and Overlord verbatim up to the last clause. |
| Using ability: {0} / on cooldown | 使用{0}。 / 冷却中 | Upstream table. |
| entombed (in a warcasket) | 封在……战棺里 | Upstream row, Medic. |

## Charge nouns

| `chargeNoun` | Simplified Chinese | Forms and lookup |
|---|---|---|
| strike (the shoulders' `CompStrikePool`) | 打击 | One form. Chinese falls to the base `Pluralize` (lookup, else unchanged) and the Chinese Keyed never switches on `CHARGENOUN_gender`, so no `WordInfo` file is shipped. A noun rather than a classifier so vanilla's slots read: 剩余打击 (2), 打击剩余, 此物体上剩余的打击数量, 这个物体最多能保持的打击数量, 打击装填消耗, 没有打击。 The two per-charge slots (每打击装填消耗, 每装填一打击) are as stiff as vanilla's own 充能 in them. |

What the counted-noun slots print (`Counted` is the number, an ASCII space and the label; Core's `ComponentIndustrial.label` 零部件 comes back unchanged at any count):

- `HPWC_DismemberReload`, 1 component: 每次打击都会烧毁护肩中的一个伺服马达，装填1 零部件即可更换。 At 3: 装填3 零部件即可更换。
- `HPWC_DismemberSummary*`, defaults: 护肩容量为2 打击，每次打击需用1 零部件装填。 At 1 strike and 3 components: 护肩容量为1 打击，每次打击需用3 零部件装填。 The count sits where a capacity or a cost value goes, so it reads as a value with its unit, as Overlord's shipped 每点充能可用1 零部件装填 does.
- `HPWC_DismemberReloadCostDesc`: 装填一次打击所需的零部件数量。默认：1。

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The term concerns it left open are listed here as still pending native review.

The reader judged 噬血型 natural for mainland players, since 噬血 is stock game vocabulary (Code Vein's Chinese title is 噬血代码), and judged the plain blood words (肢解, 失血, 溅血, 抽取血原质) natural and unlikely to offend. A native check of both judgements is still owed, beside the 嗜血 concern below. The counted nouns print without a measure word ("2 打击", "1 零部件") and read as value readouts, where a native writer would say 2次打击 and 1个零部件.

Every coinage in "Set-specific rows": 噬血型 above all (噬 shares a sound with 嗜血, the bloodfeeder gene's word, and a reader may hear feeding in it as the English "hemophage" itself allows), 血原奔涌, 肢解 for both the ability and the damage, 打击 as the charge noun, 抽取 for drain, 伺服马达, 强化机械臂, 热能护甲经过强化, 面罩, 溅血, 血原质保留量, 基础血原质获取量, 抽取时间, 宾客, 低阶战棺, 威胁点数, 只会受伤 / 可能只会受伤, and the death message {0}被撕碎了。 All free prose: the three descriptions (paragraph 1 after the family opener, paragraphs 2 and 3, including 差强人意 for "somewhat lackluster"), the two ability descriptions, the hediff description, the six combat log lines, every settings tooltip and summary. The VFEP terms' review is tracked upstream, under the section named above.
