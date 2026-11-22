# Korean glossary, Hemophage Warcasket

First pass: machine-assisted generation, 2026-11-22, no native review yet.
Only mod-specific coinages and grounding decisions; family-wide mechanics, josa
markers and style live in `l10n/languages/Korean.md`. The VFEP warcasket
vocabulary (warcasket, the parts, the label pattern, foundry, entomb, VFEP's
set names and research labels, pilot, the spacer-tier opener) lives upstream
in that file under "Grounded common vocabulary, VFE Pirates warcasket domain",
with its grounding source (the RimWorldKorea `RMK` pack) and what is pending
native review; the settings and ability words the family settled sit in that
file's main table. The descriptions open on the family's spacer-tier opener;
paragraph 2 (this set's telling of the 10th generation, not VFEP's) and the
series paragraph are this mod's prose and are recorded below. Nothing here
restates an upstream row; a VFEP term is changed there first. Vanilla terms
are grounded against the Core and Biotech `Korean (한국어).tar`.

## Set-specific rows

| English | Korean | Grounding |
|---|---|---|
| hemophage (the set name, the bare label prefix) | 헤모파지 | Coined from two Biotech renderings: 헤모 is how Biotech ko renders the "hemo" prefix (`MemeDef` `Bloodfeeding.generalRules`, `memeHyphenPrefix->헤모`), 파지 is the tail of the official `Sanguophage.label` 생귀오파지, which is itself a transliteration. So the set reads as the sanguophage's kin the way the English does. Labels 헤모파지 워캐스킷 / 헤모파지 워캐스킷 어깨 / 헤모파지 워캐스킷 헬멧 on the upstream pattern. Rejected: 혈액정수 워캐스킷 (names the resource, not a series), 혈액파지 (mixes a native noun with a transliterated tail, no vanilla precedent), 흡혈 워캐스킷 (흡혈 is Biotech's bloodfeeding word, which no surface may name). |
| Hemophage series | 헤모파지 시리즈 | Upstream "series" as the siblings ship it (독소 시리즈, 함선 침투 시리즈). |
| Workshop title / `HPWC_SettingsCategory` | 헤모파지 워캐스킷 | Set name + 워캐스킷 (the anchor term); the two are equal (title coupling). |
| trait label `warcasket` | 워캐스킷 | The bare upstream noun, as Medic and Overlord ship it. |
| hemogen | 혈액정수 | Biotech Keyed `Hemogen`, `Hemogenic.resourceLabel`. |
| hemogen level | 혈액정수 수치 | Biotech Keyed `AlertLowHemogenDesc` (혈액정수 수치가 낮습니다). |
| hemogen craving | 혈액정수 갈망 | Biotech `HemogenCraving.label`. |
| hemogenic (of a pawn) / non-hemogenic | 흡혈성 / 흡혈성이 아닌 | Biotech `Hemogenic.label` (흡혈성) and Keyed `AbilityDisabledNoHemogenGene` ({0_labelShort}(은)는 흡혈성이 아닙니다). The official word carries 흡혈; it is the gene's own label, so it is kept, and the drain verb below stays clear of it. |
| sanguophage | 생귀오파지 | Biotech `Sanguophage.label` (xenotype) and every quest text; one word serves the xenotype and sanguophages in general. |
| the rumor frame ("Some think ...", "Some believe ...") | …다고 생각하는 사람도 있습니다. / …다고 믿는 사람도 있습니다. | Biotech `Sanguophage.description`, last paragraph (어떤 사람들은 … 생각합니다. 어떤 사람들은 사실 인류 전체를 생귀오파지가 조종하고 있다고 믿기도 합니다.); "directed" is that sentence's 조종하다. The shipped rumors keep its 생각 / 믿다 verbs but close with 사람도 있습니다 instead of opening with 어떤 사람들은. |
| hemogen rush (ability, hediff, settings section) | 혈액정수 쇄도 | Core `GlucosoidRush.label` (글루코소이드 쇄도) and `PsychofluidRush.label` (사이코플루이드 쇄도): vanilla's "rush" hediffs. |
| dismember (ability, verb label, section) / dismemberment (the damage) | 사지 절단 / 사지 절단 | Coined. 사지 per Core prose ("잘린 사지", "사지의 심각한 감염"); 절단 per Core `SurgicalCut.label` (외과적 절단). Korean names the ability and the damage with one noun, so both labels read alike; the prose verb is 뜯어내다 (tear off). |
| tear off (a limb) / tear at / rip off | 뜯어내다 / 잡아 뜯다 / 뽑아 버리다 | Coined for the descriptions, the job report and the combat log. |
| limb / arm / leg / head / heart | 사지 / 팔 / 다리 / 머리 / 심장 | Core prose for 사지; `BodyPartDef` `Arm`, `Leg`, `Head`, `Heart` labels. |
| strike (the charge noun; one use of Dismember) | 일격 | Coined; see "Charge nouns". |
| drain (hemogen, from a wound or a target) / drain time | 추출하다 / 추출 시간 | Biotech's neutral verb for drawing hemogen out: `ExtractedHemogenPack.label` (구성원에게서 혈액정수 추출), `Bloodfeeder.description` (피에서 혈액정수를 추출), the 혈액팩 추출 surgery. Never 흡혈 (Biotech's bloodfeeding, `WarningPawnWillDieFromBloodfeeding` 흡혈하면) and never 빨아들이다. |
| the injectors / hemogen injectors | 주입기 / 혈액정수 주입기 | Medic's 아드레날린 주입기, after Core's fuelling port 연료 주입기. |
| the servos | 서보 모터 | Coined; vanilla ko has no servo. |
| reinforced arms | 강화 팔 | Shipcracker's shipped 돌파용 강화 팔. |
| visor / sealed visor | 바이저 / 밀폐형 바이저 | Medic's 진단 바이저; upstream 밀폐형 row. |
| heat protection | 열기 방어력 | Core `ArmorRating_Heat.label` (방어도 - 열기), with the family's 방어력. |
| improved impact dispersion | 개선된 충격 분산 능력 | Upstream (Shipcracker's variant of the spacer opener). |
| blood pumping | 혈액 순환 | Core `BloodPumping.label`. |
| blood loss / bleeding / blood (spatter) | 혈액 손실 / 출혈 / 피, 핏자국 | Core `BloodLoss.label`, Keyed `BleedingRate` (출혈), `Filth_Blood.label` (피). Blood is plain throughout (피를 잃습니다). |
| hemogen reserve (settings row) | 혈액정수 비축량 | Coined on Biotech `Hemogenic.resourceDescription` (비축분). |
| base hemogen gain | 기본 혈액정수 획득량 | Coined. |
| body size | 신체 크기 | Core Keyed `BodySize`. |
| melee damage / melee damage factor / move speed | 근접 피해 / 근접 피해 계수 / 이동 속도 | Core `MeleeDamageFactor.label`, `MoveSpeed.label`. |
| human (target) | 인간 | Biotech quest and xenotype text (인간 혈액, 인간 사회). |
| sunlight / UV sensitivity gene | 햇빛 / 자외선 민감성 유전자 | Biotech Keyed `InSunlight` (햇빛에 노출), `UVSensitivity_Mild.label` (자외선 민감성). |
| pyrophobia gene / flee in terror from fire | 불 공포증 유전자 / 불을 보고 겁에 질려 달아나다, 불에서 도망치다 | Biotech `FireTerror` GeneDef label (불 공포증); the mental state `PanicFleeFire` (불로부터 도망, 불에서 도망치는중) and Core `PanicFlee.beginLetter` (겁에 질려 달아나고). The mental break's own label, Biotech `MentalBreakDef` `FireTerror` = 화재 테러, reads as "arson attack", so the row label 불에서 도망칠 확률 names the state instead. |
| deathrest | 죽음안식 | Biotech `Deathrest.label` (not used in this mod's text today). |
| Dismember's cursor labels (where Core's `WillKill` 사망 goes) | 부상에 그침 / 부상에 그칠 수 있음 | Core Keyed `WillKill` (사망): short, no period. |
| the two Confirm dialogs | 일격으로 {PAWN_nameDef}의 머리를 뜯어내면 … / 일격 후 {PAWN_nameDef}에게서 혈액정수를 추출하면 {PAWN_objective} 죽이게 됩니다.\n\n계속하시겠습니까? | Biotech Keyed `WarningPawnWillDieFromBloodfeeding` ({PAWN_nameDef}(을)를 흡혈하면 {PAWN_objective} 죽일 수 있습니다.\n\n계속하시겠습니까?), with "will kill" as 죽이게 됩니다. `{PAWN_objective}` is 그를 / 그녀를 and takes no particle. |
| the combat log lines | `[INITIATOR_definite](이)가 [RECIPIENT_definite]의 [recipient_part0_label](을)를 뜯어냄.` and kin | Core `RulePacks_CombatMelee` (`…[RECIPIENT_definite]의 [recipient_part0_label](을)를 [damaged_past].`, `…버리려고 했지만 …`), nominal `-함` endings; the possessive symbol is dropped as Core does. |
| reportString / jobReportString | TargetA의 사지 뜯어내는 중 / 사지 뜯어내는 중 | Core `JobDef` shape: invariant 의 only, `~ 중`, no period. |
| damage label / death message | 사지 절단 / {0}(이)가 갈기갈기 찢겨 죽었습니다. | Core `Scratch.deathMessage` ({0}(이)가 찢겨 죽었습니다.). |
| hediff description | 이 사람의 워캐스킷이 … | Biotech `XenogermLossShock.description` (이 사람의 …). |
| intellectual work (trait's last clause) | 지적노동 | Core Keyed `WorkTagIntellectual` (지적노동), the tag the trait keeps. Already a noun of work, so the clause is 다만 지적노동은 여전히 할 수 있습니다. without 작업. |
| work tab | 작업 탭 | Medic and Overlord. |
| government program (lore) / veterans / power armor | 정부 사업단 / 숙련병 / 동력 갑옷 | Medic's shipped paragraph 2, whose first two sentences this set reuses verbatim; 동력 갑옷 per Core `Apparel_PowerArmor.description`. |
| record (the program has no record) | 기록 | Plain word; Core backstories (복무에 부적합한 것으로 기록되었습니다). |
| sanguophage meeting (quest) / leader / guests | 생귀오파지 회의 / 지도자 / 손님 | Biotech `SanguophageMeetingHost` quest text (회의, 그들의 지도자인). |
| crashed shuttle (quest) / master / thrall | 추락한 왕복선 / 생귀오파지 주인 / 흡혈종복 | Biotech `SanguophageShip` letters (추락한 왕복선, 생귀오파지 주인과 … 흡혈종복); `SanguophageThrall.label` (흡혈종복). |
| down and capture | 쓰러뜨려 생포하다 | Core `Downed.label` (쓰러짐); Biotech `SanguophageShip.questDescription` (생포하면). |
| quest / raid / storyteller | 임무 / 습격 / 이야기꾼 | Core Keyed `Raid`, `Storyteller`; upstream 임무 row. |
| threat points | 위협 점수 | Coined on Core `Difficulty_ThreatScale_Label` (위협 규모); vanilla ko has no player-facing "threat points" string. |
| colony wealth / colonist count | 정착지 재산 / 정착민 수 | Core history graphs (정착지 재산). |
| lesser warcaskets (VFEP's two plain sets) | 하급 워캐스킷 | Coined; RMK has no collective word for the plain sets. |
| faction anger | 대상이 속한 세력의 분노를 삽니다 | Core Keyed `Faction` (세력). |
| hold their nerve near fire | 불 가까이에서도 평정을 잃지 않다 | Coined. |
| the set's paragraph 2 and series paragraph | 10세대 워캐스킷은 노획한 해적 장비를 정부 사업단이…… / 헤모파지 시리즈는 조종사의 혈액정수로 움직입니다…… | This mod's prose, byte-identical across the three descriptions; the full text is `1.6/Languages/Korean/DefInjected/ThingDef/Apparel_Hemophage.xml`. |

## Ported from the siblings

| English | Korean | Source |
|---|---|---|
| Default: {0}. | 기본값: {0}. | Every sibling tooltip. |
| (default) suffix | ` (기본값)` | Sibling `*_DefaultSuffix`, leading space kept. |
| enabled / disabled | 켜짐 / 꺼짐 | Medic `MDWC_Enabled` / `MDWC_Disabled`, Overlord `OLWC_WorkOpen` / `OLWC_WorkBlocked`. |
| Off (slider value) | 없음 | Shipcracker `SCWC_Off`, upstream row. |
| When enabled, … When disabled, … | 켜면 …. 끄면 …. | Medic, Overlord, Shipcracker. |
| Casting time / Cooldown | 시전 시간 / 재사용 대기 | Medic `MDWC_CastTime`, `MDWC_Cooldown`. |
| on cooldown | 재사용 대기 중 | Upstream row; Toxin and Shipcracker `cooldownGerund`. |
| Using ability: {0} | {0} 능력 사용 중 | Upstream row; Toxin. |
| Enabled work types | 활성화된 작업 유형 | Medic and Overlord `*_SectionWork`. |
| When enabled, pilots … can do this work, though you will need to assign it in the work tab. When disabled, they cannot. | 켜면 … 조종사가 이 작업을 할 수 있지만, 작업 탭에서 따로 할당해야 합니다. 끄면 할 수 없습니다. | Overlord `OLWC_WorkDesc`. |
| The number of … the shoulders hold when full. | 어깨에 최대로 장전할 수 있는 …의 수입니다. | Medic `MDWC_ShotsDesc`. |
| The number of {1} it takes to reload one … | … 하나를 재장전하는 데 필요한 {1} 개수입니다. | Overlord `OLWC_ReloadCostDesc`. |
| The time the pilot must wait between … | 조종사가 … 후 다음 …까지 기다려야 하는 시간입니다. | Medic `MDWC_ShotCooldownDesc`. |
| The next … can follow at once. | 다음 …은 바로 이어갈 수 있습니다. | Medic `MDWC_ShotSummaryNoCooldown`. |
| At zero / At 0% | 0으로 설정하면 / 0%로 설정하면 | Overlord `OLWC_ChargesPerDayDesc`, `OLWC_SparkFrequencyDesc`. |
| settings unavailable | … 설정을 사용할 수 없습니다. 다른 모드가 헤모파지 워캐스킷을 변경했기 때문에 … | Overlord `OLWC_SettingsUnavailable`. |
| trait description | Medic's rendering verbatim up to its last clause | Medic and Overlord `degreeDatas.warcasket.description`. |
| Toggle again to … | 다시 누르면 … | Toxin `TXWC_VentTank.description`. |

## Charge nouns

| `chargeNoun` | Korean | Forms and lookup |
|---|---|---|
| strike (the shoulders' `CompStrikePool`) | 일격 | One form. Korean falls to the base `Pluralize` (lookup, else unchanged), Core and Biotech ko ship no `WordInfo` at all, and ko Keyed never switches on `CHARGENOUN_gender`, so no `WordInfo` file is shipped. Reads in every vanilla slot: 잔여 일격, 일격, 이 개체의 최대 일격 수입니다., 일격 재장전 비용, 일격당 재장전 비용, 하나의 일격을 재장전하는 데…, 마지막 일격을 사용하면…, 남은 일격이 없습니다. Consonant-final (격), so markers resolve to 을 / 이. |
| reload item (Core `ComponentIndustrial`, not this mod's noun) | 부품 | Core `ComponentIndustrial.label`; returned unchanged by `Pluralize`, so `HPWC_DismemberReloadCostDesc`'s `{1}` is 부품. |

The counters live in two Keyed strings, `HPWC_CountedStrikes` = `{1} {0}회` and `HPWC_CountedReloadItem` = `{1} {0}개`, where `{0}` is the number and `{1}` the noun. They reach `{0}` of `HPWC_DismemberReload` and `{5}` / `{6}` of the summaries as "일격 2회", "부품 1개", "부품 3개". The summaries' opening frame is 어깨에는 {5}분(이)가 들어가며, so the josa resolver looks at the counter's last syllable, never the digit: 회분 takes 이 and 개 takes 로. As printed at the defaults: 어깨에는 일격 2회분이 들어가며, 일격 하나는 부품 1개로 재장전합니다. At 1 strike and 3 components: 어깨에는 일격 1회분이 들어가며, 일격 하나는 부품 3개로 재장전합니다. The reload line: …{0}(으)로 재장전하면 교체할 수 있습니다. → 부품 1개로 / 부품 3개로.

## Workshop page

| English | Korean | Grounding |
|---|---|---|
| Biotech (DLC name) | 바이오테크 | Core `ExpansionDef` `Biotech.label`; Toxin's Workshop page. |
| dangerous UV sensitivity / animalfeeder / sanguofeeder (VRE Sanguophage genes) | 위험한 자외선 민감도 / 동물 흡혈인자 / 생귀오파지 흡혈인자 | RMK pack, VRE Sanguophage `GeneDef` (`VRE_Sensitivity_Dangerous.label`, `VRE_AnimalFeeder.label`, `VRE_SanguoFeeder.label`); the mod ships no Korean. |
| marine warcasket (VFEP plain set) | 해병대 워캐스킷 | RMK VFEP `VFEP_Warcasket_Marine.label`. Not used: the page says only 하급 워캐스킷, as the in-game tooltip does. |
| hemogen metabolism | 혈액정수 대사 | Biotech gene descriptions (대사 에너지, 신진대사). |
| filters (helmet) | 필터 | Biotech `DetoxifierKidney.description`, `ToxResist_Partial.description`. |
| brain | 뇌 | Core `Brain.label`; Biotech `Deathless.customEffectDescriptions` (뇌의 파괴로만 죽을 수 있습니다). |
| rumor | 소문 | Core book rulepacks (…에 대한 소문). |
| off-record | 어디에도 기록이 남지 않은 | Coined, after the descriptions' 사업단의 기록에는 … 없습니다. |
| an inside job all the way to the top | 맨 윗선의 생귀오파지가 꾸민 내부 소행 | Coined; told as hearsay (…다는 말도 있습니다). |
| burnout servos | 쓰고 나면 타 버리는 강력한 서보 모터 | Coined on the glossary's 서보 모터 and the shoulders' 강력한 서보 모터. |
| aim height (CE) | 조준 높이 | Coined on CE ko `CE_ToggleAimModeDesc` (조준 방식) and `CE_TargetHeight` (목표물 높이); CE ko has no melee-targeting keys. |
| shield (CE line) | 보호막 | Upstream "shield bubble" row. |
| accompanying thralls | 함께 온 흡혈종복 | Glossary 흡혈종복. |
| the helmet joke | 두꺼운 장갑이 생귀오파지 최대의 약점을 막아 줍니다. 바로 뇌가 날아가는 거죠. | Coined. |

## Pending native review

A fresh-reader register read ran on 2026-11-22 after generation, and its rewrites are in the shipped files. The term concerns it left open are still pending native review. 하급 워캐스킷 reads as "low-grade". 시전 is the spell-casting verb and sounds magical for a servo strike, though vanilla's abilities use it.

Every coinage in "Set-specific rows": 헤모파지 as the set name, 혈액정수 쇄도,
사지 절단 for both the ability and the damage, 뜯어내다 / 잡아 뜯다 / 뽑아 버리다,
일격 as the charge noun, the counters 회 and 개 in `HPWC_CountedStrikes` and `HPWC_CountedReloadItem` and the frame 회분, 추출하다 for drain, 서보 모터, 열기 방어력, 혈액정수 비축량, 기본 혈액정수
획득량, 위협 점수, 하급 워캐스킷, 불에서 도망칠 확률 against the official mental break
label 화재 테러, 부상에 그침 / 부상에 그칠 수 있음, 맥동하는 빛, 피 튀김. All free
prose: the three descriptions (paragraph 1 after the family opener,
paragraphs 2 and 3), both ability descriptions, the hediff description, every
settings tooltip and summary, and the six combat log lines. Also the vanilla
quirk that `LetterHour` is 시, so a cooldown in hours prints as "12시 동안",
which can read as a clock time; the summary says 동안 to keep it a duration.
The family frames still open upstream (켜면 / 끄면, 켜짐 / 꺼짐) are left as the
siblings ship them. The VFEP terms' review is tracked upstream.

Workshop page coinages: 어디에도 기록이 남지 않은, 맨 윗선의 생귀오파지가 꾸민 내부 소행, 쓰고 나면 타 버리는 강력한 서보 모터, 조준 높이, the helmet joke, and all of the page's free prose outside the lines taken from the siblings.
