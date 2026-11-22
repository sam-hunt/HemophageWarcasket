# Japanese glossary, Hemophage Warcasket

First pass: machine-assisted generation, 2026-11-22, no native review yet.
Only mod-specific coinages and grounding decisions; engine mechanics,
punctuation (ASCII `.` and `,` with no following space) and style live in
`l10n/languages/Japanese.md`. The VFEP warcasket vocabulary (warcasket, the
parts, the label pattern, foundry, entomb, VFEP's set names and research
labels, pilot, the spacer-tier opener) lives upstream in that file under
"Grounded common vocabulary, VFE Pirates warcasket domain", with its grounding
source and what is pending native review; the settings and ability words the
family settled sit in that file's main table. Only the spacer opener up to its
stat clause is the family's here: the generation paragraph is this mod's own,
not the upstream one, and with the series paragraph and the rest of each first
paragraph it is recorded below. Nothing here restates an upstream row; a VFEP
term is changed there first. Vanilla terms are grounded against the Core and
Biotech `Japanese (日本語).tar`.

## Set-specific rows

| English | Japanese | Grounding |
|---|---|---|
| hemophage (the set name, the bare label prefix) | ヘモファージ | Coined from Biotech's two official renderings: ヘモ, the prefix Biotech's own Hemogenic and Bloodfeeder genes generate names from (`Hemogenic.symbolPack.prefixSymbols.0.symbol`, from ヘモゲン), plus ファージ, the tail of `Sanguophage.label` サングオファージ. A player reads the kinship with both. Rejected: ヘモフェイジ (English pronunciation, breaks the サングオ**ファージ** rhyme), ヘマトファージ (the real biology word, loses the ヘモゲン link), a kanji coinage such as 血食者 (sits beside Biotech's `Bloodfeeder.label` 吸血者 and reads as the feeding gene). Labels ヘモファージ・ウォーキャスケット / ヘモファージ・ウォーキャスケットショルダーアーマー / ヘモファージ・ウォーキャスケットヘルメット on the upstream pattern. |
| the Hemophage series (prose) | ヘモファージ・シリーズ | Overlord's shipped オーバーロード・シリーズ. |
| Workshop title / `HPWC_SettingsCategory` | ヘモファージ・ウォーキャスケット | Equal to the armor label and to each other (title coupling); contains the anchor ウォーキャスケット. The Workshop description itself is still owed. |
| hemogen / hemogenic / non-hemogenic | ヘモゲン / ヘモゲニック / 非ヘモゲニック | Biotech `Hemogenic.resourceLabel`, `Hemogenic.label`, Keyed `AbilityDisabledNoHemogenGene` (ヘモゲニック遺伝子), `Bloodfeed.description` (非ヘモゲニック人間). |
| sanguophage | サングオファージ | Biotech `Sanguophage.label`; one word for the xenotype and the people. |
| spend hemogen | ヘモゲンを消費 | Biotech `HemogenCraving.description` (ヘモゲン備蓄を消費), `Sanguophage` quest info (ヘモゲンを消費しなければならず). |
| hemogen craving | ヘモゲンの渇望 | Biotech `HemogenCraving.label`. |
| hemogen gain | ヘモゲン取得量 | Biotech `HemogenAmplified.description` (ヘモゲン取得量). |
| drain (hemogen, from a wound or a target) / drain time | 抽出 / 抽出時間 | Biotech's neutral word for drawing hemogen out of a person: `ExtractedHemogenPack.label` (メンバーからヘモゲンを抽出), `HemogenFarm.description`, the gene extractor's strings. Used for every "drain": the descriptions (傷口からヘモゲンを抽出), the ability, the settings and `HPWC_DismemberConfirmDrainKill`. Never 吸血 (`Bloodfeed.label`), 摂取 (Bloodfeed's ingest verb), 吸う / 噛む, or 排出 (`HemogenDrain`'s loss out of the body). |
| hemogen rush (ability, hediff, section) | ヘモゲンラッシュ | Coined on Core backstory `KingOfPirates25`'s アドレナリンラッシュ and Biotech's undotted ヘモゲンパック. The `jobReportString` is the family's 能力を使用中: {0}. |
| hemogen reserve (settings row) | ヘモゲンの温存量 | Coined; 温存 is the plain "keep back" word. The tooltip says where the rush stops (ヘモゲンがこの量まで減ると,ラッシュは自動的に止まります). |
| hemogen injectors / the injectors | ヘモゲン注入器 / 注入器 | Coined; 注入 per Biotech `ReimplantXenogerm.description` (注入器官). No vanilla injector item. |
| dismember (ability, section) / dismemberment (damage) | 四肢切断 | Coined. 切断 is vanilla's limb-removal word (Core Keyed `Amputate` 切断手術) and 四肢切断 the usual Japanese name for gore dismemberment. Ability and damage share the noun, as the English shares the stem; the verb "dismember targets" is 四肢切断を使える. |
| tear off (a limb, the head) | 引きちぎる | Core's own verb (`eat_gerund` 引きちぎる); the physical act in every description and log line. Rule pack variants もぎ取る ("rip off") and 力任せに引っ張る ("tore at"). |
| limb / arm or leg / head | 手足 / 腕か脚 / 頭 | Core `GameplayTips.RemoveLimbInfection` (手足), BodyPartDef `Arm.label` 腕, `Leg.label` 脚, `Head.label` 頭. |
| strike (charge noun; the settings' word for one use) | 攻撃 | See "Charge nouns". Counted with 回 in labels (最大攻撃回数, 攻撃1回分の充填コスト). |
| servos | サーボモーター | Core `BionicArm.description`, `BionicLeg.description` (小型サーボモーター). Same word in the description and `HPWC_DismemberReload`. |
| reinforced arms | 補強されたアーム | Coined, after Medic's 補助アーム and Shipcracker's ブリーチングアーム. |
| visor / sealed visor | バイザー / 気密バイザー | Medic's 診断バイザー; 気密 is the upstream "sealed" row. |
| blood pumping (capacity) | 心機能 | Core PawnCapacityDef `BloodPumping.label`. P3's "keep the blood pumping" is 血液を循環させます. |
| improved heat protection / impact dispersion | 熱への防護性能が向上しています / 衝撃分散性能 | Upstream opener's 防護性能 with Core `ArmorRating_Heat.label` (アーマー - 熱系); 衝撃分散性能 is Shipcracker's variant of the opener. |
| melee damage factor / move speed | 近接ダメージ掛け率 / 移動速度 | Core StatDef `MeleeDamageFactor.label`, `MoveSpeed.label`. The row label is 近接ダメージ. |
| blood loss / blood spatter | 失血 / 血しぶき (row), 血のり (the filth) | Core HediffDef `BloodLoss.label`, ThingDef `Filth_Blood.label`. |
| body size / human (target) | 体の大きさ / 人間 | Biotech `Bloodfeed.description` (ターゲットの体の大きさ, 非ヘモゲニック人間). |
| limb HP (damage tooltip) | HP | Core Keyed `HitPoints`, `MaxHitPoints.label` (最大HP). |
| Will only wound / May only wound (cursor) | 負傷させるだけ / 負傷させるだけかもしれない | Shaped like Core Keyed `WillKill` (死亡する): plain form, no period. |
| the two Confirm dialogs | 攻撃で{PAWN_nameDef}の頭を引きちぎると{PAWN_objective}殺すことになります.\n\nよろしいですか? (and 攻撃の後に{PAWN_nameDef}からヘモゲンを抽出すると…) | Biotech Keyed `WarningPawnWillDieFromBloodfeeding` ({PAWN_nameDef}を吸血すると{PAWN_pronoun}は死亡します.\n\nよろしいですか?): its 〜すると frame and closing question. The English carries `{PAWN_objective}`, so the checker holds the key to it; Grammar `ProhimObj` = 彼を carries its own を, hence 殺すことになります rather than vanilla's 死亡します. |
| UV sensitivity gene / sunlight | 紫外線アレルギーの遺伝子 / 日光 | Biotech `UVSensitivity_Mild.label` (軽度の紫外線アレルギー), `UVSensitivity_Intense.label`; 日光 per their descriptions and `SunlightSensitivity_*` thoughts. "slow such a pilot" per the quest info (紫外線で動きが遅くなります). |
| pyrophobia gene / fire terror / flee in terror | 炎恐怖症 / 炎の恐れ / パニックになって逃げ出す | Biotech GeneDef `FireTerror.label`, Core MentalBreakDef `FireTerror.label`, MentalStateDef `PanicFleeFire.label` (炎でパニックになり逃げ出した). |
| mood | 心情 | Core NeedDef `Mood.label`. |
| intellectual work (trait, last clause) | 研究の作業 | Core Keyed `WorkTagIntellectual` (研究); trait text otherwise Medic's verbatim (…ただし,研究の作業だけは行えます.). |
| sanguophage meeting (quest subheading) / leader / guests | サングオファージの会議 / リーダー / 参加者 | Biotech `SanguophageMeetingHost` quest name rules (サングオファージの + 会議) and description (リーダーである[asker_nameDef], 会議を行わせてほしい); 参加者 coined for "guests". |
| crashed shuttle (quest subheading) / master / thrall | 墜落したシャトル / マスター / しもべ | Biotech `SanguophageShip` rules (サングオファージのマスターと…しもべ, 墜落した機体), PawnKindDef `SanguophageThrall.label` (しもべ). |
| lesser warcaskets (VFEP's plain sets) | 下位のウォーキャスケット | Coined; no VFEP Japanese exists. |
| threat points / storyteller / colony wealth / raid | 脅威ポイント / ストーリーテラー / コロニーの資産 / 襲撃 | 脅威ポイント coined from Core's 脅威 (`Difficulty_ThreatScale_Label` 脅威の規模) and ポイント (no vanilla "threat points" string); Core Keyed `Storyteller`, `Difficulty_FixedWealthMode_Info` (入植者の人数,プレーヤーの資産…脅威の規模を調整), `Wealth_Total.label` (総資産). |
| research / required research / none | 研究 / 必要な研究 / なし | Core Keyed `Research`, `None`. |
| angers their faction | その対象の勢力を怒らせます | Biotech Keyed `MessageAbsorbingXenogermWillAngerFaction` ({0}を怒らせます). |
| down and capture | 倒して捕らえる | Medic's 倒れた (Core `Wait_Downed.reportString`). |
| combat log frame | [INITIATOR_definite]は[RECIPIENT_definite]の[recipient_part0_label]を〜た. | Core `RulePacks_CombatMelee` (…は[RECIPIENT_definite]の[recipient_part0_label]を[damaged_past].): plain past, part label bare after の. The `[RECIPIENT_possessive]` line drops the possessive as Core's `[skillDescMelee],そして[recipient_part0_label]を` line does (つかみ,[recipient_part0_label]を引きちぎった). |
| deathMessage | {0}は 引き裂かれて死亡しました. | Core DamageDef `Scratch.deathMessage` shape, vanilla's literal space kept. |
| JobDef / ability job reports | TargetAの手足を引きちぎっている / 手足を引きちぎっている | Core+Biotech reportStrings (TargetAをハッキングしている, TargetAから異種胚を抽出している): progressive, no period. |
| the rumor frame (paragraph 2) | 〜と考える人もいます. / 〜と信じている人もいます. | Biotech `Sanguophage.description` (…と考える人もいます.サングオファージが人類の文明全体を密かに指揮していると信じている人もいます.); 密かに from the same sentence. |
| paragraphs 2 and 3 | 第10世代は政府の計画から生まれました.…… / ヘモファージ・シリーズは,パイロットのヘモゲンで動きます.…… | This mod's prose, byte-identical across the three descriptions; the full text is `1.6/Languages/Japanese/DefInjected/ThingDef/Apparel_Hemophage.xml`. The program sentence reuses Overlord's shipped wording (鹵獲した宙賊の装備は政府のパワーアーマーを核に作り直され,ほかの形では軍務を続けられない退役軍人に支給されました). |

## Ported from the siblings

| English | Japanese | Source |
|---|---|---|
| Default: {0}. / (default) suffix | デフォルト: {0}. / ` (デフォルト)` | Every sibling's Keyed. |
| enabled / disabled / Off | 有効 / 無効 / Off | Medic `MDWC_Enabled`/`MDWC_Disabled`, Shipcracker `SCWC_Off`. |
| When enabled, … When disabled, … | 有効にすると,…. 無効にすると,…. | Medic, Overlord. |
| Casting time / Cooldown | 詠唱時間 / クールダウン | Medic, Overlord; Core Keyed `AbilityCastingTime`. |
| The time the pilot must wait between X | 〜した後,パイロットが次の〜を行えるようになるまでの待ち時間です. | Medic `MDWC_CooldownDesc`. |
| The number of … hold when full | 満タン時に〜に蓄えられる〜の回数です. | Overlord `OLWC_ChargesDesc`, Medic `MDWC_ShotsDesc`. |
| The number of {1} it takes to reload one X | X1回分の充填に必要な{1}の数です. | Overlord `OLWC_ReloadCostDesc`, verbatim but for the noun. |
| reload with {0} | {0}を使用して充填 | Medic `MDWC_ReloadsWith`. |
| … are unavailable. Another mod has changed … | …は使用できません.別のMODが…を変更しているため,能力とヘルメットには本来の値が使われます. | Medic and Overlord `*_SettingsUnavailable`, reworded here to 本来の値が使われます. |
| Enabled work types / work tab / work toggle tooltip | 有効な作業 / 「優先順位」タブ / 有効にすると,…この作業を行えるようになります.ただし,「優先順位」タブで割り当てる必要があります.無効にすると,この作業は行えません. | Overlord `OLWC_WorkDesc`. |
| on cooldown / Using ability: {0} | クールダウン中 / 能力を使用中: {0} | Upstream main table. |
| trait label and description | ウォーキャスケット / {PAWN_nameDef}はウォーキャスケットに封入されています.…ただし,研究の作業だけは行えます. | Medic `Traits.xml`, verbatim to the last clause. |
| Toggle again to stop | もう一度切り替えると〜が止まります. | Toxin `TXWC_VentTank.description`. |
| At zero / at 0% | 0にすると / 0%にすると | Overlord `OLWC_ChargesPerDayDesc`, `OLWC_SparkFrequencyDesc`. |
| entombed | 封入 | Upstream entomb row. |

## Charge nouns

| `chargeNoun` | Japanese | Forms and lookup |
|---|---|---|
| strike (the shoulders' `CompStrikePool`) | 攻撃 | One form. Japanese falls to the base `Pluralize` (lookup, else unchanged) and ja Keyed never switches on `CHARGENOUN_gender`, so no `WordInfo` file is shipped. Reads in every vanilla slot: 攻撃残数, 最大攻撃数, 攻撃回復コスト, 攻撃を1回分回復するために…, 攻撃フル充填コスト, 最後の攻撃を使用すると…, 攻撃が残っていません. Chosen over 一撃 (最大一撃数 and 一撃残数 read badly) and 打撃 (a blunt-damage word). |
| the reload item (Core `ComponentIndustrial`) | コンポーネント | Core `ComponentIndustrial.label`; unchanged by the base worker, so no line is needed. |

`Ability_Dismember.Counted` prints the number, an ASCII space and the label,
unchanged at every count. The sentences take it as a quantity:
`HPWC_DismemberReload` reads …{0}を使用して充填すれば交換できます. ("1 コンポーネントを使用して", "3 コンポーネントを使用して"); the Dismember
summaries open ショルダーアーマーには{5}分を蓄えられ,1回分ごとに{6}で充填します.
("2 攻撃分を蓄えられ,1回分ごとに1 コンポーネントで充填します.", "1 攻撃分",
"3 コンポーネントで"); `HPWC_DismemberReloadCostDesc` gets the bare
コンポーネント (攻撃1回分の充填に必要なコンポーネントの数です.).

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The term concerns it left open are still pending native review, and the counted forms below were the main one. The reader proposed recasting `HPWC_QuestThrallShareDesc` as a plain ratio, and it was declined because that dropped the sense of the strength the quest "should" have.

Every coinage above: ヘモファージ, ヘモゲンラッシュ, ヘモゲンの温存量, ヘモゲン注入器,
四肢切断 for both the ability and the damage (切断 says cutting where the act is
tearing), 攻撃 as the strike noun (it also reads as any attack), 補強されたアーム,
気密バイザー, 下位のウォーキャスケット, 脅威ポイント, 参加者 and the row labels
最大攻撃回数, 攻撃のダメージ, 炎の恐れの発生率, 敵対していない対象にも使用,
マスターのウォーキャスケット脅威ポイント. The counted forms "2 攻撃分" and
"1 コンポーネントで", which the read also flagged, are the engine's number-space-label shape and read as a stat
readout rather than natural counting (コンポーネント1個). The two Confirm dialogs
say {PAWN_objective}殺すことになります (彼を殺す…), which the checker forces where
vanilla would say {PAWN_pronoun}は死亡します. All free prose: the three
descriptions (paragraph 1 after the family opener, paragraphs 2 and 3), the two
ability descriptions, the hediff description, every settings tooltip and
summary, the six combat log lines and the death message. The pass re-read every
sentence against the language file's tells and split the chained clauses and
repeated subjects it found. The VFEP terms'
review is tracked upstream, under the section named above.
