# TODOs

## Features

- **A first look at the set in game** Nobody has welded a pawn in yet: check that the foundry lists the three pieces after spacer warcaskets research, that the picked color paints the plating while the red parts stay red, on the worn pieces and the icons alike, and how the pads draw against the helmet's horns from each facing (Overlord's pads needed a south offset under its helmet).
- **Torso**
  - **Hemogen rush: first look in game** Built, and so far only compiled and unit tested. Check:
    - The toggle shows on a pilot drafted or not, and is disabled with the game's own reason for a pilot who is not hemogenic, has no hemogen or is asleep.
    - The drain (2 per second), hemogen craving at zero, the rush carrying on through drafting and undrafting, and ending when the pilot is downed, falls asleep, starts a deathrest, has a mental break or leaves the map.
    - The glow on all three pieces from each facing, with the back of the helmet dark and nothing drawn over by the next piece.
    - The look: the tint and shader (`TransparentPostLight`, `(1, 0.2, 0.15)`), the heartbeat at speeds 1 to 3, and the steady glow zoomed out and in the colonist bar portrait.
    - The buff's numbers in play, and whether melee cooldown should join them (the hediff def's header).
  - **Work trait and its settings: first look in game** Built, and so far compiled, unit tested and booted to the menu without errors. Check:
    - An entombed pilot's Bio tab shows the warcasket trait once, with VFEP's pain and rest effects, and the work tab leaves research (and dark study with Anomaly) assignable beside what every warcasket pilot can do.
    - The pilot can hack and gains Intellectual skill, and removal surgery leaves VFEP's shellcasket trait as on any other set.
    - The settings window: the "Enabled work types" rows, their tooltips and the summary line, and that ticking a row (smithing, say) opens it for a pilot already entombed while the other skilled labor stays blocked.
    - With "Warcaskets can work!" or Hardworking Warcaskets active, the section reads that nothing is blocked.
- **Shoulders**
  - **Dismember: first look in game** Built, and so far only compiled and unit tested. Check:
    - The gizmo shows on a pilot drafted or not, a click on a far target walks the pilot over, Shift-click queues, and the cursor shows the cross over an invalid target and "Will kill" over one the strike would behead.
    - The strike: a random arm or leg reads "Torn off" in the health tab whatever the target wears (a mechanoid's "Shattered"), the target is held for the 1 second windup, the pilot's weapon is holstered, being hit does not interrupt, and the cooldown is one hour.
    - A target with no limbs left is beheaded with the "torn apart" death message, and a body built without limbs (a centipede) is refused with "has no limbs to tear off".
    - The drain, for a hemogenic pilot on a human: the 4 second bar with Bloodfeed's sound and splash, the target held throughout, about 20 hemogen gained and 0.45 blood loss on the target, and an early end when the pilot is full. A pilot without the gene, an animal or a mechanoid gets the strike alone.
    - A non-hostile target: the goodwill cost on a visitor's faction (vanilla's maximum for one hit, 130), guilt for a prisoner or colonist.
    - The numbers in play: the cooldown, the windup, and about 5 seconds of holding one enemy still.
  - **Beheading a body built without limbs** Off (`decapitateBodiesWithoutLimbs` on the ability def), so centipedes, diaboli, war queens and most Anomaly entities are invalid targets; on, one use kills them outright. Decide whether that is wanted at all.
  - **Combat log** The strike writes no line: `TakeDamage` logs nothing by itself, and vanilla's `BattleLogEntry_DamageTaken` cannot name the attacker. Wants a log entry and a rule pack of its own.
  - **Raiders never use it** VEF fires no apparel ability for a non-player pawn. The Medic repo's `CompMedicAI` is the model if raider pilots should, though the Tuning item may drop the raider tags first.
  - **Combat Extended aim height** CE keeps a per-pawn melee aim height (`CompMeleeTargettingGizmo.heightInt`, public). With CE active the strike could take a leg for low and an arm for middle instead of a random limb. Belongs with the CE compat scan.
- **Helmet**
  - grants resistance/immunity to sunlight sensitivity
  - has maxed out sharp/blunt/heat armor (2.0?)
  - maybe speeds up deathrest a little bit?
- **Mod settings for each ability**
  - The settings window is in with its work section; each ability gets a section of its own (`Source/1.6/CLAUDE.md` has the shape and where the slider class comes from).
  - **Hemogen rush drain rate** A slider over the hemogen spent per second: 0.25 steps from 0 to 5, then 0.5 steps from 5 to 10. Default is the def's 2 per second.
  - **Hemogen rush reserve** A slider over the hemogen level at which the rush stops on its own, so a pilot can keep some back and avoid hemogen craving. Default 0, where the rush runs the pilot dry.
  - **Dismember cooldown** A slider over the ability def's `cooldownTime`, in game hours. Default 1 hour. The strongest lever: the strike cannot fail, so this is its only limit.
  - **Dismember hemogen gain** A slider over `hemogenGain`, the hemogen a full drain gives. Default 20, Bloodfeed's. At 0 there is no drain.
  - **Dismember drain time** A slider over `drainTicks`, in seconds. Default 4. It is also how long the target is held.
  - **Dismember blood loss** A slider over `targetBloodLoss`, the blood loss a full drain adds to the target. Default 45%, Bloodfeed's. At 0 a drained prisoner only bleeds from the stump.
  - **Dismember beheads bodies without limbs** A switch over `decapitateBodiesWithoutLimbs`, if the item under Shoulders decides it should exist. Default off.
  - Later, if asked for: the windup (`castTime`), and a switch to refuse non-hostile targets.
- **Tuning** The three pieces are the 10th generation's baseline (VFEP's Sarcophagus row less its medicine and bionics; the def headers argue each number). Settle the set's own stats and costs as the mechanics land, then mirror them into the CE patch.
  - Raider gear: the pieces carry the tier's `WarcasketVeteran` / `WarcasketAll` tags, so VFEP's veteran raiders can roll them. Probably drop them, and patch/finalize VFEP's fill-in, similar to Overlord?
- **VRE Sanguophage Integration**
  - Dismember hemogen drain gating widens when the caster has AnimalFeeder/CorpseFeeder/Sanguofeeder and the target matches, and any associated buffs are applied
  - Hemohunter could allow autocasting dismember?
  - Does VRES add extra sunlight sensitivity gene levels, and does our helmet respect them?
  - Does VRES add any research our set might be better gated behind? Vanilla's own Deathrest doesn't quite fit, because the genes are orthoganal
  - Blood moon event could trigger hemogenic rush for free? investigate balance/flavor before committing?
  - Add a compatibility note to the workshop description
- How does hemogen sit in Chinese localizations in vanilla?

## Pre-release polish

- Family-wide leftovers found during scaffolding:
  - `.steamworkshop/CLAUDE.md` is byte-identical across the siblings, still uses Medic examples, and contradicts itself on bolded bullet leads.
  - `Docs/Research/` is the Shipcracker-era copy, pinned to an older VFEP commit than v4.03.
  - I changed CONTRIBUTING's "Build instructions are in README.md" to point at `CLAUDE.md` here; the siblings still carry the wrong line.
- **CE compat scan for any relevant changes they make to sanguophage/hemogenic mechanics**
- **CE/VGE/SOS2 compat** Check each of the family patches are in good order here
- **Transcode** `Preview.mp4` to `Preview.gif` and < 1MiB
- **Workshop gallery** Add images, Medic/Overlord as precendent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **Smoke test on 1.6**
- **Full review of player-facing text**
  - The three descriptions, `About.xml`, the README's features and the Workshop page are first cuts written to the design ahead of the code, so today they promise a helmet feature no def grants. Correct them to what is built as each mechanic lands. No release, candidates included, before the two agree.
- **Initial translation pass** of all 9 planned languages
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README
