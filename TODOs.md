# TODOs

## Features

- **Torso**
  - **grants Hemogenic Rush ability**
    - likely needs a more vanilla-idiomatic/better name.
    - For a short time, gain increased conciousness, movement speed, pain tolerance, carry capacity, melee damage factor etc, while draining the casters hemogen at a greatly increased rate. requires hemogenic gene.
    - Should be a toggleable ability, similar to Toxin's torso gas vent but a bit slower.
    - While active, draws an extra render node overlay (like Shipcracker's breach jump effect, but tinted bright red, and if possible pulsing like a heartbeat??) over any currently worn hemophage warcasket apparel (how does Shipcracker do it? what's best here? ability hooks? mod def extension? ticking comp?)
    - The glow sprites are in (`WarcasketHemophage{,Shoulders,Helmet}Glow_<facing>.png`, white over the red details that light up) and no def references them yet. The helmet has none for its north facing, and `Graphic_Multi` fills a missing north from the south, so its node must skip that facing or get an empty sprite (the helmet def's header).
  - **Allows Intellectual work** default enabled, Medic/Overlord precedent (mod settings for other work types but disabled default)
- **Shoulders**
  - **grants Dismember ability** Destroy a target's limb (could follow CE's melee height target preference? not sure if there's a vanilla equivalent?), if the caster has the hemogenic gene and the target is a humanlike, drain hemogen over 4 seconds.
- **Helmet**
  - grants resistance/immunity to sunlight sensitivity
  - has maxed out sharp/blunt/heat armor (2.0?)
  - maybe speeds up deathrest a little bit?
- **Mod settings for each ability**
  - The first Keyed string brings `1.6/Languages/English/Keyed/HemophageWarcasket.xml`, `HPWC_SettingsCategory` as the Workshop title key and the two flips in `Scripts/check-translations.py`.
- **Tuning** The three pieces are the 10th generation's baseline (VFEP's Sarcophagus row less its medicine and bionics; the def headers argue each number). Settle the set's own stats and costs as the mechanics land, then mirror them into the CE patch.
  - Raider gear: the pieces carry the tier's `WarcasketVeteran` / `WarcasketAll` tags, so VFEP's veteran raiders can roll them. Weigh that again once the pieces do something only a hemogenic pilot can use (Overlord dropped its tags for a similar reason).
- **Ability icons** `Textures/UI/Abilities/WarcasketHemophage/Dismember.png` and `HemogenRush.png` are on disk and untracked until the abilities are named. They deploy with a local build but are in no CI release until committed.
- **VRE Sanguophage Integration**
  - Dismember hemogen drain gating widens when the caster has AnimalFeeder/CorpseFeeder/Sanguofeeder and the target matches, and any associated buffs are applied
  - Hemohunter could allow autocasting dismember?
  - Does VRES add extra sunlight sensitivity gene levels, and does our helmet respect them?
  - Does VRES add any research our set might be better gated behind? Vanilla's own Deathrest doesn't quite fit, because the genes are orthoganal
  - Blood moon event could trigger hemogenic rush for free? investigate balance/flavor before committing?

## Pre-release polish

- **CE compat scan for any relevant changes they make to sanguophage/hemogenic mechanics**
- **CE/VGE/SOS2 compat** Check each of the family patches are in good order here
- **Transcode** `Preview.mp4` to `Preview.gif` and < 1MiB
- **Workshop gallery** Add images, Medic/Overlord as precendent
- **Optimize images** Run the `../MOD_DOWNLOAD_SIZE_TRIM.md` pass before the first release.
- **A first look at the set in game** Nobody has welded a pawn in yet: check that the foundry lists the three pieces after spacer warcaskets research, that the picked color paints the plating while the red parts stay red, on the worn pieces and the icons alike, and how the pads draw against the helmet's horns from each facing (Overlord's pads needed a south offset under its helmet).
- **Smoke test on 1.6**
- **Full review of player-facing text**
  - The three descriptions, `About.xml`, the README's features and the Workshop page are first cuts written to the design ahead of the code, so today they promise what no def grants. Correct them to what is built as each mechanic lands. No release, candidates included, before the two agree.
- **Initial translation pass** of all 9 planned languages
- **Create the GitHub repo** (`sam-hunt/HemophageWarcasket`; the `origin` remote already points there) and push. The `l10n` submodule's relative URL resolves against it, and the release workflow runs there.
- **First Workshop publish.** Commit `About/PublishedFileId.txt`, add the Workshop link and the Steam badges to the README
